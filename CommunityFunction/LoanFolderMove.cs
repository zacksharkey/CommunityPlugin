using CommunityFunction.LoanFolderRules;
using CommunityFunction.Objects;
using EncompassRest;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CommunityFunction
{
    public static class LoanFolderMove
    {
        [FunctionName("LoanFolderMove")]
        public async static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            string clientID = Environment.GetEnvironmentVariable("clientid");
            string clientSecret = Environment.GetEnvironmentVariable("clientsecret");

            string instance = Environment.GetEnvironmentVariable("instance");
            string userID = Environment.GetEnvironmentVariable("userid");
            string password = Environment.GetEnvironmentVariable("password");

            ClientParameters clientParams = new ClientParameters(clientID, clientSecret);
            using (var client = await EncompassRestClient.CreateAsync(clientParams, token=> token.FromUserCredentialsAsync(instance, userID, password)))
            {
                InterfaceHelper i = new InterfaceHelper();
                List<Type> rules = i.GetAll(typeof(FolderRuleBase));
                foreach (Type rule in rules)
                {
                    FolderRuleBase p = Activator.CreateInstance(rule) as FolderRuleBase;
                    try
                    {
                        bool result = await p.Execute(client);
                    }
                    catch(Exception ex)
                    {
                        log.LogInformation($"Error in rule {p.GetType().Name}: {ex.InnerException}");
                    }
                }
            }
        }
    }
}
