using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.RemotingServices;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Config
{
    public partial class eFolderTransfer_Config : Form
    {
        private eFolderTransferCDO CDO;
        public eFolderTransfer_Config(eFolderTransferCDO CDO)
        {
            InitializeComponent();
            this.CDO = CDO != null ? CDO : CustomDataObject.Get<eFolderTransferCDO>(eFolderTransferCDO.Key);
            cmbForms.Items.AddRange(Session.FormManager.GetFormInfos(InputFormType.Custom).OrderBy(x => x.Name).Select(x => x.Name).ToArray());
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            eFolderTransferRule rule = CDO.Rules.FirstOrDefault(x => x.FormName.Equals(cmbForms.Text));
            bool isNew = rule == null;
            if (isNew)
            {
                rule = new eFolderTransferRule();
                CDO.Rules.Add(rule);
            }

            rule.ControlID = txtControlID.Text;
            rule.FormName = cmbForms.Text;
            CustomDataObject.Save<eFolderTransferCDO>(eFolderTransferCDO.Key, CDO);

            MessageBox.Show("Setting Saved");
        }

        private void cmbForms_SelectedIndexChanged(object sender, EventArgs e)
        {
            eFolderTransferRule rule = CDO.Rules.FirstOrDefault(x => x.FormName.Equals(cmbForms.Text));
            if(rule != null)
                txtControlID.Text = rule.ControlID;
        }
    }
}
