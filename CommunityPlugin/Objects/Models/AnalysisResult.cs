namespace CommunityPlugin.Objects.Models
{
    public class AnalysisResult
    {
        public object Result { get; set; }
        public string Log { get; set; }
        public string Name { get; set; }

        public AnalysisResult(string Name)
        {
            this.Name = Name;
        }
    }
}
