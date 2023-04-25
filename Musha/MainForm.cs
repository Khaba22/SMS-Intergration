using Musha.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Musha.API.LDetails;

namespace Musha
{
    public partial class MainForm : Form
    {
        private SeverClient m_currentApi;
        private IResponse m_lastResponse;
        public MainForm()
        {

            InitializeComponent();
            LoadAPIs();
        }
        private void LoadAPIs()
        {
            cbApis.Items.Add(new ClickatelClient(LDetails.CLICKATELL_API_KEY));
            cbApis.Items.Add(new Twillo(LDetails.ClickatelUsername, LDetails.ClickatelPassword));
        }
        private void cbApis_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentApi = cbApis.SelectedItem as SeverClient;

            if (m_currentApi == null)
                return;

            if (!m_currentApi.IsInitialized)
                m_currentApi.Init();

            cbCall.Checked = m_currentApi.CanCall;
            cbSms.Checked = m_currentApi.CanSendSms;

            btnCall.Enabled = m_currentApi.CanCall;
            btnText.Enabled = m_currentApi.CanSendSms;

            txtFrom.Enabled = m_currentApi.FromNumberRequired;

        }

        private async void btnText_Click(object sender, EventArgs e)
        {
            btnText.Enabled = false;

            string from = txtFrom.Text;
            string to = txtTo.Text;
            string body = txtBody.Text;

            SetStatus("Sending...");

            m_lastResponse = await m_currentApi.SendSmsAsync(from, to, body);


            btnText.Enabled = true;
            SetStatus();
        }

        private async void btnCall_Click(object sender, EventArgs e)
        {
            btnCall.Enabled = false;

            string from = txtFrom.Text;
            string to = txtTo.Text;
            string body = txtBody.Text;

            SetStatus("Sending...");

            m_lastResponse = await m_currentApi.CallAsync(from, to, body);

            btnCall.Enabled = true;
            SetStatus();
        }
        private void SetStatus()
        {
            if (m_lastResponse == null)
                return;

            SetStatus(m_lastResponse.Status);
        }

        private void SetStatus(string value)
        {
            lblStatus.Text = $"Status: {value}";
        }

        private async void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            if (m_lastResponse == null || !m_lastResponse.CanUpdate)
                return;

            await m_lastResponse.UpdateAsync();

            SetStatus();
        }
    }
}
