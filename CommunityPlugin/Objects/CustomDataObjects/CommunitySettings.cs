using CommunityPlugin.Objects.Models;
using System.Collections.Generic;

namespace CommunityPlugin.Objects.CustomDataObjects 
{ 
    public class CommunitySettings
    {
        public List<PluginAccessRight> Rights { get; set; }

        public CommunitySettings()
        {
            Rights = new List<PluginAccessRight>();
        }
    }
}
