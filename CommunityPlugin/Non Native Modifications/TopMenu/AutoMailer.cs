using CommunityPlugin.Objects;
using System;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public class AutoMailer : MenuItemBase
    {
        protected override void menuItem_Click(object sender, EventArgs e)
        {
            AutoMailer_Form f = new AutoMailer_Form();
            f.Show();
        }
    }
}
