using CommunityPlugin.Objects.Interface;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public abstract class MenuItemBase : IMenuItemBase
    {
        internal ToolStripMenuItem menuItem;

        public ToolStripItem CreateToolStripMenu(Image image, string Name)
        {
            menuItem = new ToolStripMenuItem(Name);
            menuItem.Image = image;
            menuItem.Click += new EventHandler(menuItem_Clicked);
            return (ToolStripItem)menuItem;
        }

        private void menuItem_Clicked(object sender, EventArgs e)
        {
            bool flag = FormWrapper.OpenForms.Any(x => x.Name.Equals($"{menuItem.Text}_Form"));
            if (!flag)
                menuItem_Click(sender, e);
        }

        protected abstract void menuItem_Click(object sender, EventArgs e);
    }
}