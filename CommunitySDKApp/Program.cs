using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.ClientServer.Contacts;
using EllieMae.EMLite.ClientServer.CustomFields;
using EllieMae.EMLite.ClientServer.Query;
using EllieMae.EMLite.ContactUI;
using EllieMae.EMLite.ContactUI.CustomFields;
using EllieMae.Encompass.BusinessObjects.Contacts;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunitySDKApp
{
    class Program
    {
        static void Main(string[] args)
        {
            new EllieMae.Encompass.Runtime.RuntimeServices().Initialize();
            Run();
        }

        private static void Run()
        {
            string instance = "";
            string uri = $"https://{instance}.ea.elliemae.net${instance}";
            string user = "";
            string pass = "";


            Session s = new Session();
            s.Start(uri, user, pass);
            EllieMae.EMLite.RemotingServices.Session.Start(uri, user, pass, "api.Encompass");

            string guid = "";
            Loan current = s.Loans.Open(guid);
            if (current == null)
                return;

        }
    }
}
