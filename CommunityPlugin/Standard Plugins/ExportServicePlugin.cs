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


namespace CommunityPlugin.Standard_Plugins
{
    /// <summary>
    /// This is using LoanServices.dll also you must change the alias for  Export.llad to anything other than global.
    /// All of these services are available in your 'AppData\LocalLow\Apps\Ellie Mae\xIxxxx\EncompassData\Settings\Services.xml'
    /// UCD
    /// FrdMacCAC
    /// FrdMacLPA
    /// FnmaUcdTransfer
    /// UladDu
    /// Fannie
    /// Freddie
    /// CitieScore
    /// Ilad
    /// GinnieMaePdd12
    /// GinnieMae12.161
    /// WellsFargo
    /// </summary>
    public class ExportServicePlugin:Plugin, ILoanOpened, IFieldChange
    {
        private ExportServiceConfig Config;
        public override void Configure()
        {
            ExportServicePlugin_Config form = new ExportServicePlugin_Config();
            form.ShowDialog();
        }

        public override void LoanOpened(object sender, EventArgs e)
        {
            Config = CustomDataObject.Get<ExportServiceConfig>(ExportServiceConfig.Key);
        }

        public override void FieldChanged(object sender, FieldChangeEventArgs e)
        {
            if(e.FieldID.Equals(Config.ExportFieldID) && e.NewValue.Equals("X"))
            {
                ServiceSetting setting = ServicesMapping.GetServiceSettingFromID("Ilad");
                new ExportService(Session.LoanDataMgr, setting).ProcessLoans(new string[] { EncompassApplication.CurrentLoan.Guid });
            }
        }
    }
}
