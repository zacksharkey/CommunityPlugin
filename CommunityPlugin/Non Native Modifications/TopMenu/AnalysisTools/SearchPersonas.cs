using CommunityPlugin.Objects.Extension;
using CommunityPlugin.Objects.Models;
using Elli.Common.Extensions;
using EllieMae.EMLite.RemotingServices;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Users;
using System.Collections.Generic;
using System.Linq;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu.AnalysisTools
{
    public class SearchPersonas : AnalysisBase
    {
        public override AnalysisResult ExecuteTest() { return null; }

        public override bool IsTest() { return false; }

        public override void LoadCache()
        {

        }

        public override AnalysisResult SearchResults(string Search)
        {
            List<PersonaResult> result = null;

            if (Search.Empty())
                result = EncompassApplication.Session.Users.Personas.Cast<EllieMae.Encompass.BusinessObjects.Users.Persona>().Select(x => new PersonaResult() { Name = x.Name }).ToList();
            else
            {
                EllieMae.Encompass.BusinessObjects.Users.Persona p = EncompassApplication.Session.Users.Personas.GetPersonaByName(Search);
                if (p != null)
                    result = EncompassApplication.Session.Users.GetUsersWithPersona(p, false).Cast<User>().Select(x=> new PersonaResult() {Name = x.FullName, UserID = x.ID, Personas = x.Personas.ToString() }).ToList();
            }

            return new AnalysisResult(nameof(SearchPersonas)) { Result = result };
        }
    }
}
