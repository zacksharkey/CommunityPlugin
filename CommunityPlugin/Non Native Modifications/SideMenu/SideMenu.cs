using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Interface;
using System;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.SideMenu
{
    public class SideMenu : Plugin, ITabChanged, ILoanTabChanged
    {
        public override void LoanTabChanged(object sender, EventArgs e)
        {
            SideMenuUI.CreateMenu(PluginAccess.CheckAccess(nameof(SideMenu)));
        }

        public override void TabChanged(object sender, EventArgs e)
        {
            try
            {
                TabControl tabControl = sender as TabControl;
                if (tabControl.SelectedIndex < 0)
                    return;
                TabPage tabPage = tabControl.TabPages[tabControl.SelectedIndex];
                if (tabPage == null || !tabPage.Name.Contains("loanTabPage"))
                    SideMenuUI.Closing();
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(SideMenu));
            }
        }
    }
}
