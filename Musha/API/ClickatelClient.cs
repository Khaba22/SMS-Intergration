using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Windows.Forms;
using Twilio.Http;



namespace Musha.API
{
    

        public class ClickatelClient : SeverClient
        {
        private const string URL = "https://platform.clickatell.com/messages";
        private const string CONTENT_TYPE_JSON = "application/json";

        private readonly string m_auth;
       

        public bool CanCall { get { return false; } }

        public bool CanSendSms { get { return true; } }

        public bool FromNumberRequired { get { return false; } }

        public bool IsInitialized { get; set; }

        public ClickatelClient(string api)
        {
            m_auth = api;
        }

        public void Init()
        {
            IsInitialized = true;

            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072;
        }

        public override string ToString()
        {
            return "Clickatell API";
        }

        public Task<IResponse> CallAsync(string from, string to, string msg)
        {
            throw new NotImplementedException();
        }

        public async Task<IResponse> SendSmsAsync(string form, string to, string msg)
        {
            var req = WebRequest.CreateHttp(URL);

            req.ContentType = CONTENT_TYPE_JSON;
            req.Accept = CONTENT_TYPE_JSON;
            req.Method = "POST";

            req.PreAuthenticate = true;
            req.Headers.Add(HttpRequestHeader.Authorization, m_auth);

            using (var writer = new StreamWriter(await req.GetRequestStreamAsync()))
            {
                await writer.WriteAsync(new Request(to, msg).ToJson());
                await writer.FlushAsync();
            }
           //WebClient client = new WebClient();
            //Stream s = client.OpenRead(string.Format("https://api.clickatell.com/http/sendmsg?user=itrelated&password=EvCqTl=s&api_id=3433648&to=+27&text=1", txtTo.Text, txtBody.Text));
            //StreamReader reader = new StreamReader(s);
            //string result = reader.ReadToEnd();
            //MessageBox.Show(result, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

         
            using (var reader = new StreamReader((await req.GetResponseAsync()).GetResponseStream()))
            {
                string UserName = "itrelated";
                string PassWord = "EvCqTl=s";
                req.Credentials = new NetworkCredential(UserName, PassWord);
                req.Credentials = CredentialCache.DefaultCredentials;
                var json = await reader.ReadToEndAsync();

                return JsonConvert.DeserializeObject<Response>(json);


            }
           
        }

        private class Request
        {
            [JsonProperty("content")]
            public string Content { get; set; }

            [JsonProperty("to")]
            public List<string> To { get; set; } = new List<string>();

            public Request(string to, string msg)
            {
                Content = msg;
                To.Add(to);
            }

            public string ToJson()
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        public class Message
        {
            [JsonProperty("apiMessageId")]
            public string ApiMessageId { get; set; }

            [JsonProperty("accepted")]
            public bool Accepted { get; set; }

            [JsonProperty("to")]
            public string To { get; set; }

            [JsonProperty("error")]
            public object Error { get; set; }
        }

        public class Response : IResponse
        {
            [JsonProperty("messages")]
            public List<Message> Messages { get; set; }

            [JsonProperty("error")]
            public object Error { get; set; }

            public string Status
            {
                get
                {
                    if (Error != null)
                        return Error.ToString();
                    else if (Messages[0].Error != null)
                        return Messages[0].Error.ToString();
                    else
                        return (Messages[0].Accepted ? "Delivered" : "Failed");
                }
                set { }
            }

            public bool CanUpdate { get { return false; } }

            public Task UpdateAsync()
            {
                throw new NotImplementedException();
            }
        }

    }
}