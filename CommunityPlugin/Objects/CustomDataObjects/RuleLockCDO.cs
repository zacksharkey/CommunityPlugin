using CommunityPlugin.Objects.Models;
using System.Collections.Generic;

namespace CommunityPlugin.Objects.CustomDataObjects
{
    public class RuleLockCDO
    {
        public static string Key = "RuleLockSettings.json";

        public List<RuleLockInfo> Rules { get; set; }
        public RuleLockCDO()
        {
            Rules = new List<RuleLockInfo>();
        }
    }
}
