using CommunityPlugin.Objects.Extension;
using CommunityPlugin.Objects.Models;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Users;
using System.Collections.Generic;
using System.Linq;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu.AnalysisTools
{
    public class SearchUserGroups : AnalysisBase
    {
        private List<UserGroup> UserGroups = null;
        public override AnalysisResult ExecuteTest() { return null; }

        public override bool IsTest() { return false; }

        public override void LoadCache()
        {
            UserGroups = EncompassApplication.Session.Users.Groups.Cast<UserGroup>().ToList();
        }

        public override AnalysisResult SearchResults(string Search)
        {
            Search = Search.ToUpper();
            List<UserGroup> groups = null;
            List<User> users = null;
            if(Search.Empty())
            {
                groups = UserGroups;
            }
            else
            {
                UserGroup g = UserGroups.Where(x=>x.Name.ToUpper().Equals(Search)).FirstOrDefault();
                if (g == null)
                    groups = UserGroups.Where(x => x.Name.ToUpper().Contains(Search)).ToList();
                else
                    users = g.GetUsers().Cast<User>().ToList();
            }

            return new AnalysisResult(nameof(SearchUserGroups)) { Result = groups == null ? users.Cast<User>().Select(x => new { Text = x.ID }).ToList() : groups.Cast<UserGroup>().Select(x => new { Text = x.Name }).ToList() };
        }
    }
}
