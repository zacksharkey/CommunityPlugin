using CommunityPlugin.Non_Native_Modifications.TopMenu;
using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Models;
using System.Collections.Generic;
using System.Linq;

namespace CommunityPlugin.Objects
{
    public class PluginAccess
    {
        public static bool CheckAccess(string pluginName)
        {
            CommunitySettings cdo = CustomDataObject.Get<CommunitySettings>(CommunitySettings.Key);
            List<PluginAccessRight> rights = cdo.Rights;
            if (rights.Count.Equals(0))
            {
                rights.Add(new PluginAccessRight() { PluginName = nameof(TopMenuBase), AllAccess = true });
                rights.Add(new PluginAccessRight() { PluginName = nameof(PluginManagement), AllAccess = true });
                CustomDataObject.Save<CommunitySettings>(CommunitySettings.Key, cdo);
            }

            PluginAccessRight right = rights.Where(x => x.PluginName.Equals(pluginName)).FirstOrDefault();
            if (right == null)
                return false;

            bool isAllowedToRun = right.AllAccess;

            if (!isAllowedToRun && right.Personas != null)
                isAllowedToRun = EncompassHelper.ContainsPersona(right.Personas);

            if (!isAllowedToRun && right.UserIDs != null)
                isAllowedToRun = right.UserIDs.Contains(EncompassHelper.User.ID);

            return isAllowedToRun;
        }
    }
}
