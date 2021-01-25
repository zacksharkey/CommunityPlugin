using CommunityPlugin.Objects.Helpers;
using EllieMae.EMLite.ClientServer.Query;
using EllieMae.EMLite.ClientServer.Reporting;
using EllieMae.EMLite.Common;
using EllieMae.EMLite.Common.UI;
using EllieMae.EMLite.RemotingServices;
using EllieMae.EMLite.Reporting;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public partial class BatchTool_Form : Form
    {
        public BatchTool_Form()
        {
            InitializeComponent();
            //This is coming next :)
        }
        private void ResetGrid()
        {
            if (dgvData.DataSource != null)
                dgvData.DataSource = null;
            else
            {
                dgvData.Rows.Clear();
                dgvData.Columns.Clear();
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            ResetGrid();
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "CSV files (*.csv)|*.csv|Excel Files|*.xls;*.xlsx";
            if(o.ShowDialog() == DialogResult.OK)
            {
                string filename = o.FileName;
                string ext = Path.GetExtension(filename);
                DataTable dt = new DataTable();
                switch(ext)
                {
                    case ".csv":
                        dt = FileParser.DataTableFromCSV(filename);
                        break;
                    case ".xls":
                        dt = FileParser.DataTableFromXlsx(filename);
                        break;
                    case "xlsx":
                        dt = FileParser.DataTableFromXlsx(filename);
                        break;
                }

                dgvData.DataSource = dt;
            }
            dgvData.CellValueChanged -= dgvData_CellValueChanged;
            dgvData.CellValueChanged += dgvData_CellValueChanged;
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            ResetGrid();
            gpReportControls.Visible = true;
            ReportMainControl r = (ReportMainControl)FormWrapper.Find("ReportMainControl");
            if (r == null)
            {
                MessageBox.Show("Please Open the Report tab and select a report.");
                return;
            }

            FileSystemEntry f = (FileSystemEntry)r.GetType().GetField("currentFile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(r);
            if (f == null)
            {
                MessageBox.Show("Issue finding selected Report.");
                return;
            }

            Sessions.Session sesh = Session.DefaultInstance;
            FSExplorer rptExplorer = new FSExplorer(sesh);
            r = new ReportMainControl(sesh, false);
            ReportIFSExplorer ifs = new ReportIFSExplorer(r, sesh);
            ReportSettings settings = sesh.ReportManager.GetReportSettings(f);
            if(settings.Columns.Count() < 2)
            {
                MessageBox.Show("Please enter two fields and save your report.");
                return;
            }

            string firstColID = settings.Columns.FirstOrDefault().FieldID;
            if(!firstColID.Equals("GUID") && !firstColID.Equals("364"))
            {
                MessageBox.Show("The first field of your report must be Loan Number or GUID");
                return;
            }

            LoanReportParameters parameters = new LoanReportParameters();
            parameters.Fields.AddRange(settings.Columns);
            parameters.FieldFilters.AddRange(settings.Filters);
            parameters.UseDBField = settings.UseFieldInDB;
            parameters.UseDBFilter = settings.UseFilterFieldInDB;

            parameters.UseExternalOrganization = settings.forTPO;
            parameters.CustomFilter = CreateLoanCustomFilter(settings);
            ReportResults results = sesh.ReportManager.QueryLoansForReport(parameters, null);

            List<string[]> data = results.GetAllResults();
            dgvData.Columns.AddRange(parameters.Fields.Select(x => new DataGridViewTextBoxColumn() { HeaderText = x.FieldID }).ToArray());
            dgvData.Rows.Add(data.Count);
            for(int i = 0; i < data.Count;i++)
            {
                for(int ii = 0; ii < data[i].Count();ii++)
                {
                    dgvData.Rows[i].Cells[ii].Value = data[i][ii];
                }
            }
            dgvData.CellValueChanged -= dgvData_CellValueChanged;
            dgvData.CellValueChanged += dgvData_CellValueChanged;
        }

        private QueryCriterion CreateLoanCustomFilter(ReportSettings Settings)
        {
            QueryCriterion queryCriterion = Settings.ToQueryCriterion();
            switch (Settings.LoanFilterType)
            {
                case ReportLoanFilterType.Role:
                    QueryCriterion criterion1 = (QueryCriterion)new BinaryOperation(BinaryOperator.And, (QueryCriterion)new OrdinalValueCriterion("LoanAssociateUser.RoleID", (object)Settings.LoanFilterRoleId), (QueryCriterion)new StringValueCriterion("LoanAssociateUser.UserID", Settings.LoanFilterUserInRole));
                    queryCriterion = queryCriterion != null ? queryCriterion.And(criterion1) : criterion1;
                    break;
                case ReportLoanFilterType.Organization:
                    QueryCriterion criterion2 = (QueryCriterion)new OrdinalValueCriterion("AssociateUser.org_id", (object)Settings.LoanFilterOrganizationId);
                    if (Settings.LoanFilterIncludeChildren)
                        criterion2 = criterion2.Or((QueryCriterion)new XRefValueCriterion("Associateuser.org_id", "org_descendents.descendent", (QueryCriterion)new OrdinalValueCriterion("org_descendents.oid", (object)Settings.LoanFilterOrganizationId)));
                    queryCriterion = queryCriterion != null ? queryCriterion.And(criterion2) : criterion2;
                    break;
                case ReportLoanFilterType.UserGroup:
                    QueryCriterion criterion3 = (QueryCriterion)new OrdinalValueCriterion("AssociateGroup.GroupID", (object)Settings.LoanFilterUserGroupId);
                    queryCriterion = queryCriterion != null ? queryCriterion.And(criterion3) : criterion3;
                    break;
            }
            if (Settings.DynamicQueryCriterion != null)
                queryCriterion = queryCriterion != null ? queryCriterion.And(Settings.DynamicQueryCriterion) : Settings.DynamicQueryCriterion;
            return queryCriterion;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                bool guidReport = dgvData.Columns[0].HeaderText.Equals("guid", StringComparison.OrdinalIgnoreCase);
                foreach (DataGridViewRow r in dgvData.Rows)
                {
                    Color c = r.DefaultCellStyle.BackColor;
                    if (chkOnlyChanged.Checked && c != Color.Yellow)
                        continue;

                    if (c == Color.Red || c == Color.Green)
                        continue;

                    string guid = r.Cells[0].Value.ToString();
                    if (!guidReport)
                        guid = EncompassHelper.LoanNumberToGuid(guid);

                    if (string.IsNullOrEmpty(guid))
                    {
                        r.DefaultCellStyle.BackColor = Color.Red;
                        continue;
                    }
                    BatchUpdate b = new BatchUpdate(guid);
                    for (int i = 1; i < dgvData.Columns.Count; i++)
                    {
                        string value = r.Cells[i].Value.ToString();
                        string header = dgvData.Columns[i].HeaderText;
                        b.Fields.Add(header, value);
                    }

                    EncompassHelper.SubmitBatch(b);
                    r.DefaultCellStyle.BackColor = Color.Green;
                }
            });
        }

        private void btnUpdateColumn_Click(object sender, EventArgs e)
        {
            string val = txtCopy.Text;
            int index = dgvData.SelectedCells[0].ColumnIndex;
            foreach (DataGridViewRow r in dgvData.Rows)
                r.Cells[index].Value = val;
        }

        private void dgvData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow r = dgvData.Rows[e.RowIndex];
            r.DefaultCellStyle.BackColor = Color.Yellow;
        }
    }
}
