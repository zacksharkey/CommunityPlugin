using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public class TopMenuBase : Plugin, ILogin
    {
        List<ToolStripItem> active;

        public override void Login(object sender, EventArgs e)
        {
            GradientMenuStrip menu = (GradientMenuStrip)FormWrapper.Find("mainMenu");
            ToolStripMenuItem communityMenu = new ToolStripMenuItem("Community Menu");
            ToolStripMenuItem item = menu.Items[0] as ToolStripMenuItem;
            item.DropDownItems.Insert(0, communityMenu);
            communityMenu.DropDownItems.AddRange(GetDropDownItems());
        }

        private ToolStripItem[] GetDropDownItems()
        {
            active = new List<ToolStripItem>();
            foreach (Type type in ((IEnumerable<Type>)this.GetType().Assembly.GetTypes()).Where<Type>((Func<Type, bool>)(type => type.IsSubclassOf(typeof(MenuItemBase)))).ToList<Type>())
            {
                try
                {
                    MenuItemBase menuItemBaseClass = Activator.CreateInstance(type) as MenuItemBase;
                    if (menuItemBaseClass != null && PluginAccess.CheckAccess(type.Name) && this.active.FirstOrDefault<ToolStripItem>(x => x.GetType() == menuItemBaseClass.GetType()) == null)
                        this.active.Add(menuItemBaseClass.CreateToolStripMenu((Image)null, menuItemBaseClass.GetType().Name));
                }
                catch (Exception ex)
                {
                    Logger.HandleError(ex, nameof(TopMenuBase), (object)null);
                }
            }

            return active.ToArray();
        }
    }
}
