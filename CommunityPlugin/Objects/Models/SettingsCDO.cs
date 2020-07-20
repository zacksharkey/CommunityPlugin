using Newtonsoft.Json;
using System.Collections.Generic;

namespace CommunityPlugin.Objects.Models
{
    public class SettingsCDO
    {
        [JsonProperty("SideMenuOpenByDefault")]
        public string SideMenuOpenByDefault { get; set; }

        [JsonProperty("TestServer")]
        public string TestServer { get; set; }

        [JsonProperty("SuperAdminRun")]
        public bool SuperAdminRun { get; set; }

        [JsonProperty("Links")]
        public Dictionary<string, Dictionary<string,Dictionary<string, string>>> Links { get; set; }

        [JsonProperty("LoanInformation")]
        public Dictionary<string, Dictionary<string,string>> LoanInformation { get; set; }

        [JsonProperty("Permission")]
        public Dictionary<string, List<string>> Permission { get; set; }

        [JsonProperty("Plugins")]
        public Dictionary<string, PluginSettings> Plugins { get; set; }
    }
}
