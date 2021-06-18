using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Models;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommunityPlugin.Standard_Plugins
{
    public partial class LoanFolderMove_Simulate : Form
    {
        public LoanFolderMove_Simulate()
        {
            InitializeComponent();
            SimulateAsync();
        }

        private async void SimulateAsync()
        {
            dgvProgress.DataSource = null;

            Loan current = EncompassHelper.CurrentLoan;
            if(current == null)
            {
                MessageBox.Show("Please Open a Loan.");
                this.Close();
            }

            string folder = current.LoanFolder;
            LoanFolderRules cdo = CustomDataObject.Get<LoanFolderRules>();
            List<LoanFolderRule> rules = cdo.Rules.OrderBy(x => x.Order).ToList();
            dgvProgress.DataSource = rules;

            await Task.Run(() =>
            {
                Run(rules);
            });
        }

        private void Run(List<LoanFolderRule> Rules)
        {
            int index = 0;
            foreach(LoanFolderRule rule in Rules)
            {
                if(rule.Active && rule.Calculate())
                {
                    dgvProgress.Rows[index].DefaultCellStyle.BackColor = Color.Green;
                    LoanFolder moveToFolder = EncompassApplication.Session.Loans.Folders[rule.FolderName];
                    MessageBox.Show($"Loan would be moved to {moveToFolder.DisplayName}");
                    return;
                }
                else
                {
                    dgvProgress.Rows[index].DefaultCellStyle.BackColor = Color.Red;
                }
            }

            MessageBox.Show($"No Loan Folder Rule was found that match loan conditions");
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
