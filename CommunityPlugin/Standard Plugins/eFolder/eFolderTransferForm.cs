using CommunityPlugin.Objects.Helpers;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.BusinessObjects.Loans.Logging;
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

            List<TrackedDocument> documents = loan.Log.TrackedDocuments.Cast<TrackedDocument>().ToList();
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
            for (int i = 0; i < clbBuckets.Items.Count; i++)
                clbBuckets.SetItemCheckState(i, CheckState.Checked);

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
            lblStatus.Text = $"Transferring {Docs.Keys.Count} documents";
            foreach (TrackedDocument doc in Docs.Keys)
            {
                lblStatus.Text = $"Transferring {doc.Title} ";
                if (EncompassApplication.CurrentLoan.Log.TrackedDocuments.Cast<TrackedDocument>().Any(x => x.Title.Equals(doc.Title)))
                {
                    if (MessageBox.Show($"This placeholder already exists {doc.Title}. Do you want to create an additional placeholder?", "Duplicate Placeholder", MessageBoxButtons.OKCancel) != DialogResult.OK)
                        continue;
                }
                TrackedDocument currentDoc = null;
                try
                {
                    currentDoc = EncompassApplication.CurrentLoan.Log.TrackedDocuments.Add(doc.Title, doc.MilestoneName);
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
    }
}
