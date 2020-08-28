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
        private static readonly Dictionary<string, PluginSettings> Plugins = CDOHelper.CDO.CommunitySettings.Plugins;

        
        public static bool CheckAccess(string pluginName, bool menu = false, bool loan = false)
        {
            if (EncompassHelper.IsTest() || CDOHelper.CDO.CommunitySettings.SuperAdminRun)
                return true;

            
            if (Plugins != null) return CheckAccessInPlugins(pluginName, menu, loan);


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

        private static bool CheckAccessInPlugins(string pluginName, bool menu, bool loan)
        {
            PluginSettings pluginSettings = null;
            if (menu)
            {
                if ((Plugins.TryGetValue("SideMenu", out pluginSettings) == true) && (Plugins.TryGetValue(pluginName, out pluginSettings) == true))
                    return true;
            }
            else
            {
                if (Plugins.TryGetValue(pluginName, out pluginSettings) == false) return false;
            }

            if (pluginSettings == null) return false;

            if (pluginSettings.Permissions == null) return false;

            if (pluginSettings.Permissions.Everyone) return true;

            var isAllowedToRun = !loan && pluginSettings.Permissions.Everyone;

            if (!isAllowedToRun && pluginSettings.Permissions.Personas != null)
                isAllowedToRun = EncompassHelper.ContainsPersona(pluginSettings.Permissions.Personas);

            if (!isAllowedToRun && pluginSettings.Permissions.UserIDs != null)
                isAllowedToRun = pluginSettings.Permissions.UserIDs.Contains(EncompassHelper.User.ID);

            return isAllowedToRun;
        }
    }
}
