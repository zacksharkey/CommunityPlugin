using CommunityPlugin.Non_Native_Modifications;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Models;
using CommunityPlugin.Standard_Plugins;
using CommunityPlugin.Standard_Plugins.Forms.CodebaseAssemblies.SettingsExtract;
using System.Collections.Generic;
using System.Linq;

namespace CommunityPlugin.Objects
{
    public class PluginAccess
    {

        private static readonly List<PluginAccessRight> Rights = CDOHelper.CDO.CommunitySettings.Permission["AllAccess"].Select(x => new PluginAccessRight() { AllAccess = true, PluginName = x.ToString() }).ToList();

        public static bool CheckAccess(string pluginName, bool menu = false, bool loan = false)
        {
            if (EncompassHelper.IsTest() || CDOHelper.CDO.CommunitySettings.SuperAdminRun)
                return true;

            PluginAccessRight right = Rights.Where(x => x.PluginName.Equals(pluginName)).FirstOrDefault();
            if (right == null)
                return false;

            bool isAllowedToRun = loan ? false : right.AllAccess;

            if (!isAllowedToRun && right.Personas != null)
                isAllowedToRun = EncompassHelper.ContainsPersona(right.Personas);

            if (!isAllowedToRun && right.UserIDs != null)
                isAllowedToRun = right.UserIDs.Contains(EncompassHelper.User.ID);

            return isAllowedToRun;
        }
    }
}
