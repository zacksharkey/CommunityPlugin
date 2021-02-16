using System.Collections.Generic;

namespace CommunityPlugin.Objects.CustomDataObjects
{
    public class ExportServiceConfigs
    {
        public static string Key = $"{nameof(ExportServiceConfigs)}.json";

        public List<ExportServiceConfig> Configs { get; set; }
        public ExportServiceConfigs()
        {
            Configs = new List<ExportServiceConfig>();
        }
    }

    public class ExportServiceConfig
    {
        public string Service { get; set; }

        public string ExportControlID { get; set; }

        public List<string> Forms { get; set; }

        public ExportServiceConfig()
        {
            ExportControlID = string.Empty;
            Service = string.Empty;
            Forms = new List<string>();
        }
    }
}
