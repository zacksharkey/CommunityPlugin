using CommunityPlugin.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public class Impersonate : MenuItemBase
    {

        protected override void menuItem_Click(object sender, EventArgs e)
        {
            Impersonate_Form form = new Impersonate_Form();
            form.Show();
        }
    }
}
