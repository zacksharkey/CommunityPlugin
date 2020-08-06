using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Models;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu.AnalysisTools
{
    public class SearchLog : AnalysisBase
    {
        public override AnalysisResult ExecuteTest() { return null; }

        public override bool IsTest() { return false; }

        public override void LoadCache()
        {

        }

        public override AnalysisResult SearchResults(string Search)
        {
            return new AnalysisResult(nameof(SearchLog)) { Result = EncompassHelper.ReadLog(Search) };
        }
    }
}
