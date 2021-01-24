using System;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public class BatchTool : MenuItemBase
    {
        protected override void menuItem_Click(object sender, EventArgs e)
        {
            BatchTool_Form f = new BatchTool_Form();
            f.ShowDialog();
        }
    }
}
