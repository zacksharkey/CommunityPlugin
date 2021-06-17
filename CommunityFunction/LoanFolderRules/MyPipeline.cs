using EncompassRest;
using EncompassRest.Filters;
using EncompassRest.LoanPipeline;
using System.Threading.Tasks;

namespace CommunityFunction.LoanFolderRules
{
    public class MyPipeline : FolderRuleBase
    {
        public override async Task<bool> Execute(EncompassRestClient Client)
        {
            string folder = "My Pipeline";
            NotEmptyFieldFilter appDate = new NotEmptyFieldFilter("Fields.3142");
            StringFieldFilter initialDisclosure = new StringFieldFilter("LOG.MS.STATUS.Initial Disclosure", StringFieldMatchType.Exact, "Expected");

            PipelineParameters pipe = new PipelineParameters(appDate.And(initialDisclosure));
            LoanPipelineCursor cursor = await Client.Pipeline.CreateCursorAsync(pipe);
            if (cursor.Count.Equals(0))
            {
                return false;
            }
            else
            {
                foreach (LoanPipelineData data in await cursor.GetItemsAsync(0, cursor.Count))
                    await Client.LoanFolders.MoveLoanToFolderAsync(data.LoanGuid, folder);
            }

            return true;
        }
    }
}
