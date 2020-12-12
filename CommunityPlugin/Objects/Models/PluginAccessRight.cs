using System.Collections.Generic;

namespace CommunityPlugin.Objects.Models
{
    public class PluginAccessRight
    {
        public string PluginName { get; set; }
        public string Description { get; set; }
        public bool AllAccess { get; set; }
        public List<string> Personas { get; set; }
        public List<string> UserIDs { get; set; }
        public PluginAccessRight()
        {
            Personas = new List<string>();
            UserIDs = new List<string>();
            Description = string.Empty;
            PluginName = string.Empty;
        }
    }
}
