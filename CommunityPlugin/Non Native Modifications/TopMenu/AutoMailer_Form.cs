using CommunityPlugin.Objects.Enums;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Models;
using EllieMae.EMLite.Common;
using EllieMae.EMLite.Common.UI;
using EllieMae.EMLite.RemotingServices;
using EllieMae.EMLite.Reporting;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public partial class AutoMailer_Form : Form
    {
        private string TriggerName;
        AutoMailerCDO CDO = CustomDataObject.Get<AutoMailerCDO>();
        public AutoMailer_Form()
        {
            InitializeComponent();
            SetupControls();
        }

        private void SetupControls()
        {
            ClearControls();

            cmbTriggers.Items.Clear();
            cmbTriggers.Items.AddRange(CDO.Triggers.Select(x => x.Name).ToArray());

            Sessions.Session defaultInstance = Session.DefaultInstance;
            FSExplorer rptExplorer = new FSExplorer(defaultInstance);
            string folder = "\\AutoMailer\\";
            FileSystemEntry fileSystemEntry = new FileSystemEntry(folder, FileSystemEntry.Types.Folder, (string)null);
            ReportMainControl r = new ReportMainControl(defaultInstance, false);
            ReportIFSExplorer ifsExplorer = new ReportIFSExplorer(r, defaultInstance);
            FileSystemEntry[] entries = ifsExplorer.GetFileSystemEntries(fileSystemEntry);
            cmbReports.Items.Clear();
            cmbReports.Items.AddRange(entries.Select(x => x.Name).ToArray());

            cmbFrequency.Items.Clear();
            cmbFrequency.Items.AddRange(Enum.GetNames(typeof(FrequencyType)));

            dgvFields.Rows.Clear();
            List<FieldDescriptor> emailFields = new List<FieldDescriptor>();
            emailFields.AddRange(EncompassApplication.Session.Loans.FieldDescriptors.StandardFields.Cast<FieldDescriptor>().Where(x => x.Description.ToLower().Contains("email")));
            emailFields.AddRange(EncompassApplication.Session.Loans.FieldDescriptors.VirtualFields.Cast<FieldDescriptor>().Where(x => x.Description.ToLower().Contains("email")));
            emailFields.AddRange(EncompassApplication.Session.Loans.FieldDescriptors.CustomFields.Cast<FieldDescriptor>().Where(x => x.Description.ToLower().Contains("email")));
            foreach (FieldDescriptor field in emailFields)
                dgvFields.Rows.Add(new string[] { field.FieldID, field.Description });
        }



        private void ClearControls()
        {
            txtName.Text = string.Empty;
            txtBcc.Text = string.Empty;
            txtCC.Text = string.Empty;
            txtSubject.Text = string.Empty;
            txtTo.Text = string.Empty;
            txtHtml.Text = string.Empty;
            chkTriggerActive.Checked = false;
            chkDays.ClearSelected();
            cmbReports.Text = string.Empty;
            cmbFrequency.Text = string.Empty;
            cmbTriggers.Text = string.Empty;
            dtpTime.Value = Convert.ToDateTime("12:01 PM");
            dtpDate.Value = DateTime.Today;
        }


        private void BtnClear_Click(object sender, EventArgs e)
        {
            TriggerName = string.Empty;
            ClearControls();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            bool update = !string.IsNullOrEmpty(TriggerName);
            if (CDO.Triggers.Any(x => x.Name.Equals(txtName.Text)) && !update)
            {
                MessageBox.Show("There is already a Trigger with this name.");
                return;
            }

            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Name cannot be blank.");
                return;
            }

            MailTrigger trigger = update ? CDO.Triggers.Where(x => x.Name.Equals(TriggerName)).FirstOrDefault() : new MailTrigger();
            trigger.Name = txtName.Text;
            trigger.ReportFilter = cmbReports.SelectedItem.ToString();
            trigger.Frequency = (FrequencyType)Enum.Parse(typeof(FrequencyType), cmbFrequency.SelectedItem.ToString());
            trigger.Time = dtpTime.Value;
            trigger.Date = dtpDate.Value;
            trigger.Days = chkDays.CheckedIndices.Cast<int>().ToArray();
            trigger.To = txtTo.Text;
            trigger.CC = txtCC.Text;
            trigger.BCC = txtBcc.Text;
            trigger.Subject = txtSubject.Text;
            trigger.Body = txtHtml.Text;
            trigger.Active = chkTriggerActive.Checked;

            if (!update)
                CDO.Triggers.Add(trigger);

            CustomDataObject.Save<AutoMailerCDO>(CDO);

            SetupControls();
        }

        private void BtnDuplicate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TriggerName))
            {
                MailTrigger trigger = CDO.Triggers.Where(x => x.Name.Equals(TriggerName)).FirstOrDefault();
                MailTrigger newTrigger = trigger.Clone(trigger);
                CDO.Triggers.Add(newTrigger);

                CustomDataObject.Save<AutoMailerCDO>(CDO);
                SetupControls();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            bool update = !string.IsNullOrEmpty(TriggerName);
            if (update)
            {
                MailTrigger trigger = CDO.Triggers.Where(x => x.Name.Equals(TriggerName)).FirstOrDefault();
                CDO.Triggers.Remove(trigger);

                CustomDataObject.Save<AutoMailerCDO>(CDO);
                SetupControls();
            }
        }

        private void CmbTriggers_SelectedIndexChanged(object sender, EventArgs e)
        {
            MailTrigger trigger = CDO.Triggers.Where(x => x.Name.Equals(cmbTriggers.SelectedItem.ToString())).FirstOrDefault();
            TriggerName = trigger.Name;
            txtName.Text = trigger.Name;
            cmbReports.Text = trigger.ReportFilter;
            cmbFrequency.Text = trigger.Frequency.ToString();
            dtpTime.Value = trigger.Time;
            chkTriggerActive.Checked = trigger.Active;

            if (trigger.Date != DateTime.MinValue)
                dtpDate.Value = trigger.Date;

            for (int day = 0; day < 7; day++)
                chkDays.SetItemChecked(day, trigger.Days.Contains(day));

            txtTo.Text = trigger.To;
            txtCC.Text = trigger.CC;
            txtBcc.Text = trigger.BCC;
            txtSubject.Text = trigger.Subject;
            txtHtml.Text = trigger.Body;

            lblPreviewCC.Text = trigger.CC;
            lblPreviewFrom.Text = "AutoMailer@amecinc.com";
            lblPreviewFrom2.Text = "AutoMailer@amecinc.com";
            lblPreviewSubject.Text = trigger.Subject;
            lblPreviewSubject2.Text = trigger.Subject;
            lblPreviewTo.Text = trigger.To;
        }

        private void CmbReports_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            pnlSearch.Visible = true;
        }

        private void TxtHtml_TextChanged(object sender, EventArgs e)
        {
            string[] split = txtHtml.Text.Split('[', ']');
            string finalHtml = String.Join(" ", split);
            List<string> mergeFields = txtHtml.Text.Split().Where(x => x.StartsWith("[") && x.EndsWith("]")).Select(x => x.Replace("[", "").Replace("]", "")).ToList();
            foreach (string field in mergeFields)
            {
                string value = EncompassHelper.Val(field);
                finalHtml = finalHtml.Replace($"{field}", value);
            }

            browser.DocumentText = "0";
            browser.Document.OpenNew(true);
            browser.Document.Write(finalHtml);
            browser.Refresh();
        }

        private void BtnCloseSearch_Click(object sender, EventArgs e)
        {
            pnlSearch.Visible = false;
        }
    }
}
