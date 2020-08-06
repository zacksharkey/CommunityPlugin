using CommunityPlugin.Objects.Interface;
using System.Diagnostics;

namespace CommunityPlugin.Objects.Models
{
    public abstract class AnalysisBase : IAnalysisBase
    {
        public object Cache { get; set; }
        public abstract void LoadCache();
        public abstract bool IsTest();

        public abstract AnalysisResult ExecuteTest();

        public abstract AnalysisResult SearchResults(string Search);

        public AnalysisResult Execute()
        {
            AnalysisResult result = null;

            if(IsTest())
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                result = ExecuteTest();
                sw.Stop();

                result.Log = $"Execution Time: {sw.ElapsedMilliseconds} ms";
            }
            else
            {
                result = ExecuteTest();
            }

            return result;
        }

        public AnalysisResult Search(string Search)
        {
            return SearchResults(Search);
        }
    }
}