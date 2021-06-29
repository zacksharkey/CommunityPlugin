using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Interface;

namespace CommunityPlugin.Standard_Plugins
{
    public class WelcomePageForm : Plugin, INativeFormLoaded
    {
        public override void NativeFormLoaded(object sender, FormOpenedArgs e)
        {
            if (e.OpenForm.Name.Equals(nameof(WelcomePageForm)))
            {
                e.OpenForm.Close();
            }
        }
    }
}
