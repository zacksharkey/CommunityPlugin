using CommunityPlugin.Objects;
using System;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public class PluginManagement : MenuItemBase
    {

        public override bool CanRun()
        {
            return PluginAccess.CheckAccess(nameof(PluginManagement));
        }

        protected override void menuItem_Click(object sender, EventArgs e)
        {
            PluginManagement_Form f = new PluginManagement_Form();
            f.Show();
        }
    }
}
