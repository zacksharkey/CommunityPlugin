using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Models;
using Elli.Common.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu.AnalysisTools
{
    public class SearchTriggers : AnalysisBase
    {
        public override AnalysisResult ExecuteTest() { return null; }

        public override bool IsTest() { return false; }

        public override AnalysisResult SearchResults(string Search)
        {
            Search = Search.ToUpper();
            List<BusRule> results = EncompassHelper.SessionObjects.BpmManager.GetRules().Where(x => x.RuleName.ToUpper().Contains(Search)
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
