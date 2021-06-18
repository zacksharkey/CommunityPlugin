using CommunityPlugin.Config;
using CommunityPlugin.Objects;
using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using EllieMae.Encompass.Automation;
using System;
using System.Linq;

namespace CommunityPlugin.Standard_Plugins.eFolder
{
    public class eFolderTransfer: Plugin, ILogin, IFormLoaded
    {
        private eFolderTransferCDO CDO;
        public override void Configure()
        {
            eFolderTransfer_Config form = new eFolderTransfer_Config(CDO);
            form.ShowDialog();
        }

        public override void Login(object sender, EventArgs e)
        {
            CDO = CustomDataObject.Get<eFolderTransferCDO>();
        }

        public override void FormLoaded(object sender, FormChangeEventArgs e)
        {
            string form = e.Form.Name;
            eFolderTransferRule formRule = CDO.Rules.FirstOrDefault(x => x.FormName.Equals(form));
            if (formRule == null)
                return;

            foreach (EllieMae.Encompass.Forms.Control c in e.Form.FindControlsByType(typeof(EllieMae.Encompass.Forms.Button)))
            {
                if (c.ControlID.Equals(formRule.ControlID))
                {
                    EllieMae.Encompass.Forms.Button b = (EllieMae.Encompass.Forms.Button)c;
                    b.Click += B_Click;
                }
            }
        }

        private void B_Click(object sender, EventArgs e)
        {
            eFolderTransferForm form = new eFolderTransferForm();
            form.ShowDialog();
        }
    }
}
