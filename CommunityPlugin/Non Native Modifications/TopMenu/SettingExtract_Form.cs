using CommunityPlugin.Objects.Helpers;
using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.DataEngine.eFolder;
using EllieMae.EMLite.RemotingServices;
using EllieMae.Encompass.Automation;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public partial class SettingExtract_Form : Form
    {
        private string fileName;
        public SettingExtract_Form()
        {
            InitializeComponent();
            using (SaveFileDialog o = new SaveFileDialog())
            {
                o.Filter = "Zip Files | *.zip;";
                o.ShowDialog();
                fileName = o.FileName;
            }

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            backgroundWorker1.DoWork += BackgroundWorker1_DoWork;

            backgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            bool workNotDone = true;
            while (workNotDone)
            {
                var zipContent = new MemoryStream();
                var archive = new ZipArchive(zipContent, ZipArchiveMode.Create);

                //Add("Downloading Custom Data Objects", Session.ConfigurationManager.GetCustomDataObjectNames().ToDictionary(x => x, x => Session.ConfigurationManager.GetCustomDataObject(x)), archive, 10, worker);
                //Add("Downloading Plugins", Session.ConfigurationManager.GetCustomDataObjectNames().ToDictionary(x => x, x => Session.ConfigurationManager.GetCustomDataObject(x)), archive, 20, worker);
                //Add("Downloading Forms", Session.FormManager.GetFormInfos(InputFormType.Custom).ToDictionary(x => x.Name, x => Session.FormManager.GetCustomForm(x.FormID)), archive, 30, worker, ".emfrm");


                lblStatus.Text = "Downloading Settings";
                DocumentTrackingSetup dts = EncompassHelper.SessionObjects.ConfigurationManager.GetDocumentTrackingSetup();

                AddSingle("Downloading CompanyInfo Settings", "CompanyInfo.json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(EncompassHelper.SessionObjects.ConfigurationManager.GetCompanyInfo())), archive, 35, worker);
                AddSingle("Downloading EPass Credential Settings", "EPassCredentials.json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(EncompassHelper.SessionObjects.ConfigurationManager.GetAllePassCredentialSettings())), archive, 40, worker);
                AddSingle("Downloading Loan Folder Settings", "LoanFolders.json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(EncompassHelper.SessionObjects.LoanManager.GetAllLoanFolderInfos(true))), archive, 45, worker);
                AddSingle("Downloading Tasks Settings", "Tasks.json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(EncompassApplication.Session.Loans.Templates.Tasks)), archive, 50, worker);
                AddSingle("Downloading Documents Settings", "Documents.json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(EncompassApplication.Session.Loans.Templates.Documents)), archive, 55, worker);
                AddSingle("Downloading Custom Field Settings", "CustomFields.json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(EncompassHelper.SessionObjects.ConfigurationManager.GetLoanCustomFields())), archive, 60, worker);
                AddSingle("Downloading Fee Management Settings", "FeeManagement.json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(EncompassHelper.SessionObjects.ConfigurationManager.GetFeeManagement())), archive, 65, worker);
                AddSingle("Downloading CompPlans Settings", "CompPlans.json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(EncompassHelper.SessionObjects.ConfigurationManager.GetAllCompPlans(false, 0))), archive, 70, worker);
                AddSingle("Downloading LoanFolderAccess Settings", "LoanFolderAccess.json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(EncompassHelper.SessionObjects.BpmManager.GetLoanFolderAccessRules())), archive, 75, worker);
                AddSingle("Downloading MilestoneTasks Settings", "MilestoneTasks.json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(EncompassHelper.SessionObjects.ConfigurationManager.GetMilestoneTasks(dts.ToArray().Select(x => x.Guid).ToArray()))), archive, 80, worker);
                AddSingle("Downloading Triggers Settings", "Triggers.json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(EncompassHelper.SessionObjects.BpmManager.GetRules(BizRuleType.Triggers))), archive, 85, worker);
                AddSingle("Downloading AutomatedConditions", "AutomatedConditions.json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(((BpmManager)(AutomatedConditionBpmManager)Session.BPM.GetBpmManager(BpmCategory.AutomatedConditions)).GetAllRulesFromDatabase())), archive, 90, worker);

                archive.Dispose();

                lblStatus.Text = "Writing to Zip File.";
                File.WriteAllBytes(fileName, zipContent.ToArray());

                worker.ReportProgress(100);
                lblStatus.Text = "Done";
                btnClose.Enabled = true;
                workNotDone = false;
            }
        }

        private void Add(string Title, Dictionary<string, BinaryObject> data,  ZipArchive Archive, int Progress, BackgroundWorker worker, string suffix = null)
        {
            lblStatus.Text = Title;
            foreach (KeyValuePair<string, BinaryObject> kvp in data)
                AddEntry($"{kvp.Key}{suffix}", kvp.Value.GetBytes(), Archive);
            worker.ReportProgress(Progress);
        }

        private void AddSingle(string Title, string FileName, byte[] data, ZipArchive Archive, int Progress, BackgroundWorker worker)
        {
            lblStatus.Text = Title;
            AddEntry($"{FileName}", data, Archive);
            worker.ReportProgress(Progress);
        }

        private void BackgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }

        private void AddEntry(string FileName, byte[] fileContent, ZipArchive archive)
        {
            var entry = archive.CreateEntry(FileName);
            using (var stream = entry.Open())
                stream.Write(fileContent, 0, fileContent.Length);
        }

        private void BtnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
