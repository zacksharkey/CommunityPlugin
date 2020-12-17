using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Interface;
using EllieMae.Encompass.Automation;

namespace CommunityPlugin.Standard_Plugins.Forms.CodebaseAssemblies.SettingsExtract
{
    public class Attach_Sample : Plugin, ILogin
    {

        public override void Run()
        {
            LoansScreen loans = (LoansScreen)EncompassApplication.Screens[EncompassScreen.Loans];
            if (loans == null)
                return;
            loans.AttachCodebaseToForm("Name Of The Form", typeof(Form_Example));
        }
    }
}
