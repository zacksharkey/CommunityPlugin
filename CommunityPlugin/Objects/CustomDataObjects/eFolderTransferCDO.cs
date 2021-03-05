using System.Collections.Generic;

namespace CommunityPlugin.Objects.CustomDataObjects
{
    public class eFolderTransferCDO
    {
        public static string Key = $"{nameof(eFolderTransferCDO)}.json";

        public List<eFolderTransferRule> Rules { get; set; }

        public eFolderTransferCDO()
        {
            Rules = new List<eFolderTransferRule>();
        }
    }

    public class eFolderTransferRule
    {
        public string FormName { get; set; }
        public string ControlID { get; set; }
        public eFolderTransferRule()
        {
            ControlID = string.Empty;
            FormName = string.Empty;
        }
    }
}
