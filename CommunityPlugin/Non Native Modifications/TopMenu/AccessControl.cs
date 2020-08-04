using CommunityPlugin.Non_Native_Modifications.TopMenu;
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
        public AccessControl()
        {
            InitializeComponent();
            InterfaceHelper ih = new InterfaceHelper();
            comboBox1.Items.Add(ih.GetAll(typeof(Plugin)).Select(x => x.Name));
            comboBox1.Items.Add(ih.GetAll(typeof(MenuItemBase)).Select(x => x.Name));
            comboBox1.SelectedIndex = 0;
            comboBox1.TextChanged += ComboBox1_TextChanged;
            cbPersonas.Items.AddRange(EncompassApplication.Session.Users.Personas.Cast<EllieMae.Encompass.BusinessObjects.Users.Persona>().Select(x => x.Name).ToArray());
            cbUsers.Items.AddRange(EncompassApplication.Session.Users.GetAllUsers().Cast<EllieMae.Encompass.BusinessObjects.Users.User>().Select(x => x.ID).ToArray());
        }

        private void ComboBox1_TextChanged(object sender, EventArgs e)
        {
            string Name = comboBox1.Text;
            CDO cdo = CDOHelper.CDO;
            Dictionary<string, PluginSettings> plugins = cdo.CommunitySettings.Plugins;
            bool flag = plugins.ContainsKey(Name);
            PluginSettings settings = flag ? plugins[Name] : null;
            chkAllAccess.Checked = flag ? settings.Permissions.Everyone : false;
            for (int i = 0; i < cbPersonas.Items.Count; i++)
            {
                if(flag)
                    cbPersonas.SetItemChecked(i, settings.Permissions.Personas.Contains(cbPersonas.Items[i]));
                else
                    cbPersonas.SetItemChecked(i, false);
            }
            for (int i = 0; i < cbUsers.Items.Count; i++)
            {
                if (flag)
                    cbUsers.SetItemChecked(i, settings.Permissions.UserIDs.Contains(cbUsers.Items[i]));
                else
                    cbUsers.SetItemChecked(i, false);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string Name = comboBox1.Text;
            CDO cdo = CDOHelper.CDO;
            Dictionary<string, PluginSettings> plugins = cdo.CommunitySettings.Plugins;

            bool flag = plugins.ContainsKey(Name);
            PluginSettings settings = flag ? plugins[Name] : new PluginSettings();
            settings.Permissions.Everyone = chkAllAccess.Checked;
            settings.Permissions.Personas = cbPersonas.CheckedItems.OfType<string>().ToList();
            settings.Permissions.UserIDs = cbUsers.CheckedItems.OfType<string>().ToList();
            if (!flag)
                cdo.CommunitySettings.Plugins.Add(Name, settings);

            CDOHelper.UpdateCDO(cdo);
            CDOHelper.UploadCDO();
            MessageBox.Show($"{settings.PluginName} Saved");

        }
    }
}
