using CommunityPlugin.Non_Native_Modifications;
using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.UI;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class AppraiserDialog:Plugin, ILogin
    {
        private Form AppraiserDialogForm;
        public override void Login(object sender, EventArgs e)
        {
            FormWrapper.FormOpened += FormWrapper_FormOpened; 
        }

        private void FormWrapper_FormOpened(object sender, Objects.Args.FormOpenedArgs e)
        {
            AppraiserDialogForm = e.OpenForm;
            if (!AppraiserDialogForm.Name.Equals(nameof(AppraiserDialog), StringComparison.OrdinalIgnoreCase))
                return;

            TabControl appraisersTab = AppraiserDialogForm.Controls.Find("appraisersTab", true).FirstOrDefault() as TabControl;
            if (appraisersTab == null)
                return;

            appraisersTab.SelectedIndexChanged += AppraisersTab_SelectedIndexChanged;
            RemoveAppraisalCompanies(appraisersTab);
        }

        private void AppraisersTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl appraisersTab = sender as TabControl;
            RemoveAppraisalCompanies(appraisersTab);
        }

        private void RemoveAppraisalCompanies(TabControl appraisersTab)
        {
            bool shouldRun = EncompassApplication.Session.Loans.FieldDescriptors.CustomFields.Cast<FieldDescriptor>().Any(x => x.FieldID.StartsWith("CX.AMC.DISABLE"));
            if (!shouldRun)
                return;

            string controlID = appraisersTab.SelectedIndex.Equals(0) ? "lvwMyAppraisers" : "lvwAllAppraisers";
            GridView grid = AppraiserDialogForm.Controls.Find(controlID, true).FirstOrDefault() as GridView;
            if (grid == null)
                return;

            grid.Items.Change += Items_Change;
            Remove(grid);
        }

        private static void Remove(GridView grid)
        {
            string companyList = EncompassApplication.CurrentLoan?.Fields["CX.AMC.DISABLE"].FormattedValue.ToLower().Replace(" ", "");
            List<GVItem> companiesToRemove = new List<GVItem>();
            foreach (GVItem item in grid.Items)
                if (companyList.Contains(item.Text.ToLower().Replace(" ", "")))
                    companiesToRemove.Add(item);

            foreach (GVItem remove in companiesToRemove)
                grid.Items.Remove(remove);
        }

        private void Items_Change(object sender, EventArgs e)
        {
            GridView grid = ((EllieMae.EMLite.UI.GVItemCollection)sender).GridView;
            if (grid != null)
                Remove(grid);
        }
    }
}
