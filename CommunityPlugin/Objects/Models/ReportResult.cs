using EllieMae.EMLite.ClientServer.Reporting;

namespace CommunityPlugin.Objects.Models
{
    public class ReportResult
    {
        public FieldFilter[] Filters { get; set; }
        public ColumnInfo[] Columns { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string MatchingProperty { get; set; }
    }
}
