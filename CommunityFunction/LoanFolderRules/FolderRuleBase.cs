using EncompassRest;
using System.Threading.Tasks;

namespace CommunityFunction.LoanFolderRules
{
    public class FolderRuleBase
    {
        public virtual async Task<bool> Execute(EncompassRestClient Client)
        {
            return false;
        }
    }
}
