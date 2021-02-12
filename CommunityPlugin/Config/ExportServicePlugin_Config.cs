using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using System;
using System.Windows.Forms;

namespace CommunityPlugin.Config
{
    public partial class ExportServicePlugin_Config : Form
    {
        private ExportServiceConfig Config;
        public ExportServicePlugin_Config()
        {
            InitializeComponent();
            Config = CustomDataObject.Get<ExportServiceConfig>(ExportServiceConfig.Key);
            txtFieldID.Text = Config.ExportFieldID;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Config.ExportFieldID = txtFieldID.Text;
            CustomDataObject.Save<ExportServiceConfig>(ExportServiceConfig.Key, Config);
        }
    }
}
