using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Models;
using EllieMae.EMLite.DataEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu.AnalysisTools
{
    public class SearchCustomFields : AnalysisBase
    {
        public override AnalysisResult ExecuteTest() { return null; }

        public override bool IsTest() { return false; }

        public override void LoadCache()
        {
            Cache = EncompassHelper.SessionObjects.ConfigurationManager.GetLoanCustomFields();
        }

        public override AnalysisResult SearchResults(string Search)
        {
            CustomFieldsInfo Fields = (CustomFieldsInfo)Cache;
            Search = Search.ToUpper();
            List<CustomFieldInfo> info = Fields.Cast<CustomFieldInfo>()
                                               .Where(x => x.FieldID.ToUpper().Contains(Search) 
                                                        || x.Calculation.ToUpper().Contains(Search) 
                                                        || x.Description.ToUpper().Contains(Search)).ToList();

            return new AnalysisResult(nameof(SearchCustomFields)) { Result = info };
        }
    }
}
