using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Models;
using CommunityPlugin.Standard_Plugins;
using System;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public partial class LoanFolderMove_Form : Form
    {
        private LoanFolderRules CDO;
        public LoanFolderMove_Form()
        {
            InitializeComponent();
            InitControls();
        }
         
        private void InitControls()
        {
            CDO = CustomDataObject.Get<LoanFolderRules>();
            dgvFolders.Columns.Add("Loan Folder", "Loan Folder");
            foreach (string folder in EncompassHelper.GetFolders())
                dgvFolders.Rows.Add(folder);

            cmbMilestone.Items.AddRange(EncompassHelper.GetAllMilestones());
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if(EncompassHelper.CurrentLoan != null)
            {
                LoanFolderRule currentRule = new LoanFolderRule();
                currentRule.Expression = txtCalculation.Text;
                MessageBox.Show($"Result was {currentRule.Calculate()}");
            }
            else
            {
                MessageBox.Show("Please Open a Loan to test.");
            }
        }

        private void btnSimulate_Click(object sender, EventArgs e)
        {
            LoanFolderMove_Simulate s = new LoanFolderMove_Simulate();
            s.ShowDialog();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dgvFolders.SelectedRows.Count.Equals(0))
                return;

            string folder = dgvFolders.SelectedRows[0].Cells[0].Value.ToString();
            if(!string.IsNullOrEmpty(folder))
            {
                LoanFolderRule current = CDO.Rules.Where(x => x.FolderName.Equals(folder)).FirstOrDefault();
                if(current == null)
                {
                    current = new LoanFolderRule();
                    current.FolderName = folder;
                    CDO.Rules.Add(current);
                }
                current.Expression = txtCalculation.Text;
                current.Milestone = cmbMilestone.Text;
                current.Order = Convert.ToInt32(txtOrder.Value);
                current.Active = chkActive.Checked;
            }

            CustomDataObject.Save<LoanFolderRules>(CDO);
            ClearFields();
        }

        private void ClearFields()
        {
            txtCalculation.Text = string.Empty;
            cmbMilestone.Text = string.Empty;
            txtOrder.Value = 0;
            chkActive.Checked = false;
        }

        private void dgvFolders_SelectionChanged(object sender, EventArgs e)
        {
            ClearFields();
            LoanFolderRule current = GetSelectedRule();
            if (current == null)
                return;

            txtCalculation.Text = current.Expression;
            cmbMilestone.Text = current.Milestone;
            txtOrder.Value = current.Order;
            chkActive.Checked = current.Active;
        } 

        private LoanFolderRule GetSelectedRule()
        {
            if (CDO == null)
                return null;

            string folder = dgvFolders.SelectedRows[0].Cells[0].Value.ToString();
            if (string.IsNullOrEmpty(folder))
                return null;

            return CDO.Rules.FirstOrDefault(x => x.FolderName.Equals(folder));
        }
    }
}
