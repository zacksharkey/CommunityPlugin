using CommunityPlugin.Objects.Models;
using System.Collections.Generic;

namespace CommunityPlugin.Objects.CustomDataObjects 
{ 
    public class CommunitySettings
    {
        public static string Key =  $"{nameof(CommunitySettings)}.json";

        public List<PluginAccessRight> Rights { get; set; }

        public CommunitySettings()
        {
            Rights = new List<PluginAccessRight>();
        }
    }
}
