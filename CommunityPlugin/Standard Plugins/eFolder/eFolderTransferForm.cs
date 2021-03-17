using CommunityPlugin.Objects.Extension;
using CommunityPlugin.Objects.Helpers;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.BusinessObjects.Loans.Logging;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Reporting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CommunityPlugin.Standard_Plugins.eFolder
{
    public partial class eFolderTransferForm : Form
    {
        private Dictionary<TrackedDocument, List<Attachment>> Docs;
        public eFolderTransferForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ln = txtLoanNumber.Text;
            if (string.IsNullOrEmpty(ln))
                return;

            Loan loan = EncompassApplication.Session.Loans.Open(EncompassHelper.LoanNumberToGuid(ln));
            if (loan == null || loan.Locked)
                return;

            Docs = new Dictionary<TrackedDocument, List<Attachment>>();
            loan.Lock();
            loan.BusinessRulesEnabled = false;
            loan.CalculationsEnabled = false;

            List<TrackedDocument> documents = loan.Log.TrackedDocuments.Cast<TrackedDocument>().OrderBy(x=>x.Title).ToList();
            foreach(TrackedDocument doc in documents)
            {
                List<Attachment> attachments = new List<Attachment>();
                foreach(Attachment att in doc.GetAttachments())
                {
                    attachments.Add(att);
                }
                Docs.Add(doc, attachments);
            }

            clbBuckets.Items.AddRange(documents.Select(x => x.Title).ToArray());
            clbBuckets.CheckAll(true);

            loan.BusinessRulesEnabled = true;
            loan.CalculationsEnabled = true;
            loan.Unlock();
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            new Thread(() => TransferFiles()) { IsBackground = true }.Start();
        }

        private void TransferFiles()
        {
            List<string> errors = new List<string>();
            lblStatus.Text = $"Transferring documents";
            foreach (TrackedDocument doc in Docs.Keys)
            {
                if (clbBuckets.GetItemCheckState(clbBuckets.Items.IndexOf(doc.Title)) != CheckState.Checked)
                    continue;

                lblStatus.Text = $"Transferring {doc.Title} ";
                TrackedDocument existingDoc = EncompassApplication.CurrentLoan.Log.TrackedDocuments.Cast<TrackedDocument>().FirstOrDefault(x => x.Title.Equals(doc.Title));
                bool exists = existingDoc != null;
                TrackedDocument currentDoc = null;
                try
                {
                    currentDoc = exists ? existingDoc : EncompassApplication.CurrentLoan.Log.TrackedDocuments.Add(doc.Title, doc.MilestoneName);
                }
                catch (Exception)
                {
                    errors.Add(doc.Title);
                    continue;
                }
                foreach (Attachment a in Docs[doc])
                {
                    string ext = Path.GetExtension(a.Title);
                    string filename = Path.GetFileName(a.Title);
                    string path = $"{Environment.CurrentDirectory}\\{filename}{(string.IsNullOrEmpty(ext) ? ".pdf" : ext)}";
                    try
                    {
                        a.SaveToDiskOriginal(path);
                    }
                    catch (Exception)
                    {
                        path = $"{Environment.CurrentDirectory}\\{Path.GetFileName(a.Title)}.pdf";
                        a.SaveToDisk(path);
                    }
                    Attachment att = EncompassApplication.CurrentLoan.Attachments.Add(path);
                    lblStatus.Text = $"Attaching {att.Title} to {doc.Title} ";
                    currentDoc.Attach(att);
                    if (File.Exists(path))
                        File.Delete(path);
                }
            }

            if (errors.Count() > 0)
                MessageBox.Show($"The following documents were not transfered {errors.Select(x => $"{x}{Environment.NewLine}")}");
            else
                lblStatus.Text = $"All Files Transferred Successfully";
        }

        private void btnNone_Click(object sender, EventArgs e)
        {
            clbBuckets.CheckAll(false);
        }

        private void txtLoanNumber_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLoanNumber.Text))
                return;

            string guid = EncompassHelper.LoanNumberToGuid(txtLoanNumber.Text);
            if (string.IsNullOrEmpty(guid))
                return;
            StringList sl = new StringList();
            sl.Add("Fields.4000");
            sl.Add("Fields.4002");
            LoanReportData data = EncompassApplication.Session.Reports.SelectReportingFieldsForLoan(guid, sl);
            if (data == null)
                return;

            lblBorrower.Text = $"Borrower: {data["Fields.4000"]} {data["Fields.4002"]}";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
