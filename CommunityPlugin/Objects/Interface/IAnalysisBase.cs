using CommunityPlugin.Objects.Models;

namespace CommunityPlugin.Objects.Interface
{
    public interface IAnalysisBase
    {
        AnalysisResult Execute();
        AnalysisResult Search(string Search);
        //object GetObjectsToSearch();
    }
}
