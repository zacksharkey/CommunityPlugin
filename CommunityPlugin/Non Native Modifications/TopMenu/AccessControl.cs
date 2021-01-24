using CommunityPlugin.Non_Native_Modifications.TopMenu;
using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models;
using Elli.Common.Extensions;
using EllieMae.Encompass.Automation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Objects
{
    public partial class AccessControl : UserControl
    {
        private CommunitySettings CDO = CustomDataObject.Get<CommunitySettings>(CommunitySettings.Key);
        private PluginAccessRight right;
        public AccessControl()
        {
            InitializeComponent();
            InterfaceHelper ih = new InterfaceHelper();
            comboBox1.Items.AddRange(ih.GetAll(typeof(Plugin)).Select(x => x.Name).OrderBy(x=>x).ToArray());
            comboBox1.Items.AddRange(ih.GetAll(typeof(MenuItemBase)).Select(x => x.Name).OrderBy(x => x).ToArray());
            comboBox1.SelectedIndex = 0;
            comboBox1.TextChanged += ComboBox1_TextChanged;
            cbPersonas.Items.AddRange(EncompassApplication.Session.Users.Personas.Cast<EllieMae.Encompass.BusinessObjects.Users.Persona>().Select(x => x.Name).ToArray());
            cbUsers.Items.AddRange(EncompassApplication.Session.Users.GetAllUsers().Cast<EllieMae.Encompass.BusinessObjects.Users.User>().Select(x => x.ID).ToArray());
        }

        private void ComboBox1_TextChanged(object sender, EventArgs e)
        {
            string Name = comboBox1.Text;
            List<PluginAccessRight> plugins = CDO.Rights;
            right = CDO.Rights.FirstOrDefault(x => x.PluginName.Equals(Name));
            bool newRight = right == null;
            chkAllAccess.Checked = newRight ? false : right.AllAccess;
            for (int i = 0; i < cbPersonas.Items.Count; i++)
                cbPersonas.SetItemChecked(i, newRight ? false : right.Personas.Contains(cbPersonas.Items[i]));

            for (int i = 0; i < cbUsers.Items.Count; i++)
                cbUsers.SetItemChecked(i, newRight ? false : right.UserIDs.Contains(cbUsers.Items[i]));
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string Name = comboBox1.Text;
            PluginAccessRight right = CDO.Rights.FirstOrDefault(x => x.PluginName.Equals(Name));
            if(right == null)
            {
                right = new PluginAccessRight();
                right.PluginName = Name;
                CDO.Rights.Add(right);
            }

            right.AllAccess = chkAllAccess.Checked;
            right.Personas = cbPersonas.CheckedItems.OfType<string>().ToList();
            right.UserIDs = cbUsers.CheckedItems.OfType<string>().ToList();

            CustomDataObject.Save<CommunitySettings>(CommunitySettings.Key, CDO);
            MessageBox.Show($"{CommunitySettings.Key} Saved");
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            Plugin p = Global.ActivePlugins.Where(x => x.GetType().Name.Equals(comboBox1.Text)).FirstOrDefault();
            if(p != null)
            {
                p.Configure();
            }
            else
            {
                MessageBox.Show("Did not find configuration for this plugin.");
            }
        }
    }
}
