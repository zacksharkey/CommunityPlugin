namespace CommunityPlugin.Objects.CustomDataObjects
{
    public class RetrieveDocumentsCDO
    {
        public static string Key = $"{nameof(RetrieveDocumentsCDO)}.json";
        public string FieldID { get; set; }

        public RetrieveDocumentsCDO()
        {
            FieldID = string.Empty;
        }
    }
}
