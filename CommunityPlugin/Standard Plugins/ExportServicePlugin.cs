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
using System.Collections.Generic;
using System.Linq;

namespace CommunityPlugin.Standard_Plugins
{
    public class ExportServicePlugin:Plugin, ILogin, IFormLoaded
    {
        private ExportServiceConfigs Config;
        private List<ExportServiceConfig> CurrentConfigs;
        public override void Configure()
        {
            ExportServicePlugin_Config form = new ExportServicePlugin_Config();
            form.ShowDialog();
        }

        public override void Login(object sender, EventArgs e)
        {
            Config = CustomDataObject.Get<ExportServiceConfigs>();
        } 

        public override void FormLoaded(object sender, FormChangeEventArgs e)
        {
            string form = e.Form.Name;
            CurrentConfigs = Config.Configs.Where(x => x.Forms.Contains(form)).ToList();
            foreach (ExportServiceConfig config in CurrentConfigs)
            {
                foreach (EllieMae.Encompass.Forms.Control c in e.Form.FindControlsByType(typeof(EllieMae.Encompass.Forms.Button)))
                {
                    if (c.ControlID.Equals(config.ExportControlID))
                    {
                        EllieMae.Encompass.Forms.Button b = (EllieMae.Encompass.Forms.Button)c;
                        b.Click += B_Click;
                    }
                }
            }
        }

        private void B_Click(object sender, EventArgs e)
        {
            string btnID = ((EllieMae.Encompass.Forms.Button)sender).ControlID;
            ExportServiceConfig config = CurrentConfigs.FirstOrDefault(x => x.ExportControlID.Equals(btnID));
            if (config == null || string.IsNullOrEmpty(config.Service))
                return;

            ServiceSetting setting = ServicesMapping.GetServiceSettingFromID(config.Service);
            new ExportService(Session.LoanDataMgr, setting).ProcessLoans(new string[] { EncompassApplication.CurrentLoan.Guid });
        }
    }
}
