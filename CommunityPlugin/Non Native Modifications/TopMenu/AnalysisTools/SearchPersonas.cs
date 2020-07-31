using Aspose.Words.Lists;
using CommunityPlugin.Objects.Extension;
using CommunityPlugin.Objects.Models;
using Elli.Common.Extensions;
using EllieMae.EMLite.Common;
using EllieMae.EMLite.RemotingServices;
using EllieMae.Encompass.Automation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu.AnalysisTools
{
    public class SearchPersonas : AnalysisBase
    {
        public override AnalysisResult ExecuteTest() { return null; }

        public override bool IsTest() { return false; }

        public override AnalysisResult SearchResults(string Search)
        {
            List<PersonaResult> result = null;

            if (Search.Empty())
                result = EncompassApplication.Session.Users.Personas.Cast<Persona>().Select(x => new PersonaResult() { Name = x.Name }).ToList();
            else
            {
                EllieMae.Encompass.BusinessObjects.Users.Persona p = EncompassApplication.Session.Users.Personas.GetPersonaByName(Search);
                if (p != null)
                    result = EncompassApplication.Session.Users.GetUsersWithPersona(p, false).Cast<UserInfo>().Select(x=> new PersonaResult() {Name = x.FullName, UserID = x.Userid, Personas = x.UserPersonas.ToString() }).ToList();
            }

            return new AnalysisResult(nameof(SearchPersonas)) { Result = result };
        }
    }
}
