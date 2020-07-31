using CommunityPlugin.Objects;
using System;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public class AnalysisTool : MenuItemBase
    {
        public override bool CanRun()
        {
            return PluginAccess.CheckAccess(nameof(AnalysisTool));
        }

        protected override void menuItem_Click(object sender, EventArgs e)
        {
            AnalysisTool_Form f = new AnalysisTool_Form();
            f.Show();
        }
    }
}
