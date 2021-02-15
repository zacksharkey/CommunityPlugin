using CommunityPlugin.Config;
using CommunityPlugin.Objects;
using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.InternalEM;
using EllieMae.EMLite.Common;
using EllieMae.EMLite.RemotingServices;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Linq;

namespace CommunityPlugin.Standard_Plugins
{
    public class ExportServicePlugin:Plugin, ILoanOpened, IFormLoaded
    {
        private ExportServiceConfigs Config;
        private ExportServiceConfig CurrentConfig;
        public override void Configure()
        {
            ExportServicePlugin_Config form = new ExportServicePlugin_Config();
            form.ShowDialog();
        }

        public override void LoanOpened(object sender, EventArgs e)
        {
            Config = CustomDataObject.Get<ExportServiceConfigs>(ExportServiceConfigs.Key);
        }

        public override void FormLoaded(object sender, FormChangeEventArgs e)
        {
            string form = e.Form.Name;
            CurrentConfig = Config.Configs.FirstOrDefault(x => x.Forms.Contains(form));
            if(CurrentConfig != null)
            {
                foreach(EllieMae.Encompass.Forms.Control c in e.Form.FindControlsByType(typeof(EllieMae.Encompass.Forms.Button)))
                {
                    if(c.ControlID.Equals(CurrentConfig.ExportFieldID))
                    {
                        EllieMae.Encompass.Forms.Button b = (EllieMae.Encompass.Forms.Button)c;
                        b.Click += B_Click;
                    }
                }
            }
        }

        private void B_Click(object sender, EventArgs e)
        {
            ServiceSetting setting = ServicesMapping.GetServiceSettingFromID(CurrentConfig.Service);
            new ExportService(Session.LoanDataMgr, setting).ProcessLoans(new string[] { EncompassApplication.CurrentLoan.Guid });
        }

    }
}
