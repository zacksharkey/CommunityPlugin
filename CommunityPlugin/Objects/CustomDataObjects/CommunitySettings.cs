using CommunityPlugin.Objects.Models;
using System.Collections.Generic;

namespace CommunityPlugin.Objects.CustomDataObjects 
{ 
    public class CommunitySettings
    {
        public static string Key =  $"{nameof(CommunitySettings)}.json";

        public string SideMenuOpenByDefault { get; set; }

        public string TestServer { get; set; }

        public bool SuperAdminRun { get; set; }

        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> Links { get; set; }

        public Dictionary<string, Dictionary<string, string>> LoanInformation { get; set; }

        public List<PluginAccessRight> Rights { get; set; }

        public CommunitySettings()
        {
            SideMenuOpenByDefault = string.Empty;
            TestServer = string.Empty;
            Links = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            LoanInformation = new Dictionary<string, Dictionary<string, string>>();
            Rights = new List<PluginAccessRight>();
        }
    }
}
