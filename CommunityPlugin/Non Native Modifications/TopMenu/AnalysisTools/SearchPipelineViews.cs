using CommunityPlugin.Objects.Extension;
using CommunityPlugin.Objects.Models;
using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.ClientServer.Reporting;
using EllieMae.EMLite.Common.UI;
using EllieMae.EMLite.RemotingServices;
using EllieMae.Encompass.Automation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu.AnalysisTools
{
    public class SearchPipelineViews : AnalysisBase
    {
        public override AnalysisResult ExecuteTest() { return null; }

        public override bool IsTest() { return false; }

        public override void LoadCache()
        {
            Dictionary<string, List<PersonaPipelineView>> Views = new Dictionary<string, List<PersonaPipelineView>>();
            PipelineViewAclManager pviewMgr = (PipelineViewAclManager)Session.ACL.GetAclManager(EllieMae.EMLite.ClientServer.AclCategory.PersonaPipelineView);
            foreach (EllieMae.Encompass.BusinessObjects.Users.Persona persona in EncompassApplication.Session.Users.Personas.Cast<EllieMae.Encompass.BusinessObjects.Users.Persona>())
            {
                List<PersonaPipelineView> personaViews = new List<PersonaPipelineView>();
                foreach (PersonaPipelineView view in pviewMgr.GetPersonaPipelineViews(persona.ID))
                {
                    personaViews.Add(view);
                }

                Views.Add(persona.Name, personaViews);
            }
            Cache = Views;
        }

        public override AnalysisResult SearchResults(string Search)
        {
            Dictionary<string, List<PersonaPipelineView>> Views = (Dictionary<string, List<PersonaPipelineView>>)Cache;
            List<SearchResult> results = new List<SearchResult>();
            LoanReportFieldDefs defs = LoanReportFieldDefs.GetLoanReportFieldDefs(Session.DefaultInstance).GetFieldDefsI(EllieMae.EMLite.ClientServer.LoanReportFieldFlags.AllDatabaseFields, false, Session.DefaultInstance);
            LoanReportFieldDef def = defs.GetFieldByID(Search);
            foreach (EllieMae.Encompass.BusinessObjects.Users.Persona persona in EncompassApplication.Session.Users.Personas.Cast<EllieMae.Encompass.BusinessObjects.Users.Persona>())
            {
                foreach (PersonaPipelineView view in Views[persona.Name])
                {
                    if (view.Filter != null)
                    {
                        foreach (FieldFilter f in view.Filter.Where(x => x.FieldID.Equals(Search, StringComparison.OrdinalIgnoreCase)))
                        {
                            results.Add(new SearchResult() { Persona = persona.Name, Name = view.Name, MatchingProperty = $"Filter [{f.FieldID}] {f.OperatorTypeAsString}  {f.ValueFrom} {f.ValueTo}" });
                        }
                    }

                    if (view.Columns != null)
                    {
                        foreach (PersonaPipelineViewColumn c in view.Columns.Where(x => x.ColumnDBName.Equals((def != null ? def.ToTableLayoutColumn().ColumnID : ""), StringComparison.OrdinalIgnoreCase)))
                        {
                            results.Add(new SearchResult() { Persona = persona.Name, Name = view.Name, MatchingProperty = "Column" });
                        }
                    }
                }
            }

            return new AnalysisResult(nameof(SearchPipelineViews)) { Result = results.OrderBy(x => x.Persona).ToList() };
        }
    }
}
