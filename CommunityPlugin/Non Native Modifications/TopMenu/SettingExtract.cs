using CommunityPlugin.Objects;
using System;
using System.Collections.Generic;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public class SettingExtract : MenuItemBase
    {
        protected override void menuItem_Click(object sender, EventArgs e)
        {
            SettingExtract_Form form = new SettingExtract_Form();
            form.Show();
        }
    }
}
