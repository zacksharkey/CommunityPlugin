using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using System;
using System.Windows.Forms;

namespace CommunityPlugin.Config
{
    public partial class RetrieveDocuments_Config : Form
    {
        private RetrieveDocumentsCDO Config;
        public RetrieveDocuments_Config()
        {
            InitializeComponent();
            Config = CustomDataObject.Get<RetrieveDocumentsCDO>(RetrieveDocumentsCDO.Key);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Config.FieldID = txtFieldID.Text;
            CustomDataObject.Save<RetrieveDocumentsCDO>(RetrieveDocumentsCDO.Key, Config);
            this.Close();
        }
    }
}
