namespace CommunityPlugin.Objects.CustomDataObjects
{
    public class ExportServiceConfig
    {
        public static string Key = $"{nameof(ExportServiceConfig)}.json";

        public string ExportFieldID { get; set; }
        public ExportServiceConfig()
        {
            ExportFieldID = string.Empty;
        }
    }
}
