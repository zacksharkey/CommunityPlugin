using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Enums;
using CommunityPlugin.Objects.Helpers;
using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.RemotingServices;
using System;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Config
{
    public partial class ExportServicePlugin_Config : Form
    {
        private ExportServiceConfigs Config;
        private InputFormInfo[] Forms;
        public ExportServicePlugin_Config()
        {
            InitializeComponent();
            Config = CustomDataObject.Get<ExportServiceConfigs>(ExportServiceConfigs.Key);
            cmbService.Items.AddRange(Enum.GetNames(typeof(GSEServiceType)));
            Forms = Session.FormManager.GetFormInfos(InputFormType.Custom);
            chkForms.Items.AddRange(Forms.OrderBy(x => x.Name).Select(x => x.Name).ToArray());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ExportServiceConfig config = Config.Configs.FirstOrDefault(x => x.Service.Equals(cmbService.Text));
            bool add = config == null;
            if(add)
            {
                config = new ExportServiceConfig();
                Config.Configs.Add(config);
            }

            config.Forms = chkForms.CheckedItems.Cast<string>().ToList();
            config.ExportControlID = txtFieldID.Text;
            config.Service = cmbService.Text;
            CustomDataObject.Save<ExportServiceConfigs>(ExportServiceConfigs.Key, Config);
            MessageBox.Show("Changes Saved.");
            this.Close();
        }

        private void cmbService_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Forms.Length; i++)
                chkForms.SetItemChecked(i, false);

            txtFieldID.Text = string.Empty;
            ExportServiceConfig config = Config.Configs.FirstOrDefault(x => x.Service.Equals(cmbService.Text));
            if(config != null)
            {
                foreach (var item in config.Forms)
                    chkForms.SetItemChecked(chkForms.Items.IndexOf(item), true);

                txtFieldID.Text = config.ExportControlID;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
