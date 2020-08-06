using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Models;
using Elli.Common.Extensions;
using EllieMae.EMLite.ClientServer;
using System.Collections.Generic;
using System.Linq;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu.AnalysisTools
{
    public class SearchTriggers : AnalysisBase
    {
        public override AnalysisResult ExecuteTest() { return null; }

        public override bool IsTest() { return false; }

        public override void LoadCache()
        {
            Cache = EncompassHelper.SessionObjects.BpmManager.GetRules();
        }

        public override AnalysisResult SearchResults(string Search)
        {
            BizRuleInfo[] Rules = (BizRuleInfo[])Cache;
            Search = Search.ToUpper();
            List<BusRule> results = Rules.Where(x => x.RuleName.ToUpper().Contains(Search)
                                                                                                || x.Condition.ToStringFieldValue().ToUpper().Contains(Search)
                                                                                                || x.Condition2.ToStringFieldValue().ToUpper().Contains(Search)
                                                                                                || x.ConditionState.ToStringFieldValue().ToUpper().Contains(Search)
                                                                                                || x.ConditionState2.ToStringFieldValue().ToUpper().Contains(Search))
                                                                                        .Select(x=> new BusRule() { Name = x.RuleName, AdvancedCondition = x.ConditionState })
                                                                                        .ToList<BusRule>();

            return new AnalysisResult(nameof(SearchTriggers)) { Result = results };

        }
    }
}
