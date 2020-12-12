
using System;
using System.Collections.Generic;
using System.Linq;
using EllieMae.EMLite.ClientServer.Query;
using EllieMae.EMLite.ClientServer.Reporting;
using System.Threading;
using EllieMae.EMLite.RemotingServices;
using EllieMae.EMLite.Common;
using EllieMae.EMLite.Common.UI;
using EllieMae.EMLite.Reporting;
using EllieMae.EMLite.ContactUI;
using EllieMae.Encompass.Automation;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Models;
using System.Net.Mail;

namespace CommunityPlugin.Objects.Factories
{
    public class EmailFactory : IFactory
    {
        public List<ITask> GetTriggers()
        {
            AutoMailerCDO cdo = CustomDataObject.Get<AutoMailerCDO>(AutoMailerCDO.Key);
            List<MailTrigger> Triggers = cdo.Triggers; 

            List<ITask> result = new List<ITask>();

            DateTime Now = DateTime.Now;
            foreach (MailTrigger trigger in Triggers.Where(x => x.Active))
            {
                bool run = false;
                if (!trigger.Active)
                    continue;

                bool onTime = trigger.Time.Hour.Equals(Now.Hour) && trigger.Time.Minute.Equals(Now.Minute) && Math.Abs(trigger.Time.Second - Now.Second) < 10;
                run = (trigger.Frequency == Enums.FrequencyType.Daily || trigger.Frequency == Enums.FrequencyType.Weekly || trigger.Frequency == Enums.FrequencyType.BiWeekly) && DaysOfWeek(trigger.Days).Contains(Now.DayOfWeek.ToString()) && onTime;
                if (!run)
                    run = trigger.Frequency == Enums.FrequencyType.Monthly && onTime && Now.Day.Equals(trigger.Date.Day);
                if (!run)
                    run = trigger.Frequency == Enums.FrequencyType.Yearly && onTime && Now.Day.Equals(trigger.Date.Day) && Now.Month.Equals(trigger.Date.Month);

                if (run)
                    result.Add(trigger);
            }

            return result;
        }

        private List<string> DaysOfWeek(int[] Days)
        {
            List<string> results = new List<string>();

            if (Days.Contains(0))
                results.Add("Monday");
            if (Days.Contains(1))
                results.Add("Tuesday");
            if (Days.Contains(2))
                results.Add("Wednesday");
            if (Days.Contains(3))
                results.Add("Thursday");
            if (Days.Contains(4))
                results.Add("Friday");
            if (Days.Contains(5))
                results.Add("Saturday");
            if (Days.Contains(6))
                results.Add("Sunday");

            return results;
        }

        public static void Run(MailTrigger Trigger)
        {
            GetGuidsFromReport(Trigger);
        }

        private static void GetGuidsFromReport(MailTrigger Trigger)
        {
            string folder = "\\AutoMailer\\";
            FileSystemEntry fileSystemEntry = new FileSystemEntry(folder, FileSystemEntry.Types.Folder, (string)null);
            Sessions.Session defaultInstance = Session.DefaultInstance;
            FSExplorer rptExplorer = new FSExplorer(defaultInstance);
            ReportMainControl r = new ReportMainControl(defaultInstance, false);
            ReportIFSExplorer ifsExplorer = new ReportIFSExplorer(r, defaultInstance);


            FileSystemEntry report = ifsExplorer.GetFileSystemEntries(fileSystemEntry).Where(x => x.Name.Equals(Trigger.ReportFilter)).FirstOrDefault();
            ReportSettings reportSettings = Session.DefaultInstance.ReportManager.GetReportSettings(report);

            LoanReportParameters reportParams1 = new LoanReportParameters();
            reportParams1.Fields.AddRange((IEnumerable<ColumnInfo>)reportSettings.Columns);
            reportParams1.FieldFilters.AddRange((IEnumerable<FieldFilter>)reportSettings.Filters);
            reportParams1.UseDBField = reportSettings.UseFieldInDB;
            reportParams1.UseDBFilter = reportSettings.UseFilterFieldInDB;
            reportParams1.UseExternalOrganization = reportSettings.ForTPO;
            reportParams1.CustomFilter = CreateLoanCustomFilter(reportSettings);
            ReportResults results = Session.DefaultInstance.ReportManager.QueryLoansForReport(reportParams1, null);


            List<string[]> reportResults = ReportResults.Download(results);
            List<string> guids = results.GetAllResults().Select(x => x.FirstOrDefault()).ToList();

            bool fieldsAreGuids = Guid.TryParse(guids.FirstOrDefault(), out Guid _);

            if (fieldsAreGuids)
            {
                SendEmails(Trigger, guids);
            }
        }

        private static void SendEmails(MailTrigger Trigger, List<string> guids)
        {
            foreach (string guid in guids)
            {

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(EncompassApplication.CurrentUser.Email, EncompassApplication.CurrentUser.FullName);
                mail.IsBodyHtml = true;
                mail.Subject = InsertEncompassValue(Trigger.Subject, guid);
                mail.Body = InsertEncompassValue(Trigger.Body, guid);
                foreach (string email in Trigger.To.Split(','))
                    mail.To.Add(new MailAddress(InsertEncompassValue(email, guid)));
                EncompassHelper.SendEmail(mail);
            }

            //Email Owner of Report
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(EncompassApplication.CurrentUser.Email, EncompassApplication.CurrentUser.FullName);
            mailMessage.Subject = $"Report for {Trigger.Name}";
            mailMessage.Body = $"Loans that were included in the emailed report {string.Join(Environment.NewLine, guids)}";
            mailMessage.To.Add(new MailAddress(EncompassApplication.CurrentUser.Email));
            EncompassHelper.SendEmail(mailMessage);
        }

        private static string InsertEncompassValue(string Convert, string guid)
        {
            if (!Convert.Contains("["))
                return Convert;

            string result = Convert;
            result = result.Replace("[", " [").Replace("]", "] ");
            string[] split = result.Split('[', ']');
            string finalHtml = String.Join(" ", split);
            string[] mergeFields = result.Split().Where(x => x.StartsWith("[") && x.EndsWith("]")).Select(x => x.Replace("[", "").Replace("]", "")).ToArray();
            string[] values = EncompassHelper.GetReportValues(mergeFields, guid);

            for (int i = 0; i < mergeFields.Length; i++)
            {
                result = result.Replace($"[{mergeFields[i]}]", values[i]);
            }

            return result;
        }

        private static QueryCriterion CreateLoanCustomFilter(ReportSettings ReportSettings)
        {
            QueryCriterion queryCriterion = ReportSettings.ToQueryCriterion();
            switch (ReportSettings.LoanFilterType)
            {
                case ReportLoanFilterType.Role:
                    QueryCriterion criterion1 = (QueryCriterion)new BinaryOperation(BinaryOperator.And, (QueryCriterion)new OrdinalValueCriterion("LoanAssociateUser.RoleID", (object)ReportSettings.LoanFilterRoleId), (QueryCriterion)new StringValueCriterion("LoanAssociateUser.UserID", ReportSettings.LoanFilterUserInRole));
                    queryCriterion = queryCriterion != null ? queryCriterion.And(criterion1) : criterion1;
                    break;
                case ReportLoanFilterType.Organization:
                    QueryCriterion criterion2 = (QueryCriterion)new OrdinalValueCriterion("AssociateUser.org_id", (object)ReportSettings.LoanFilterOrganizationId);
                    if (ReportSettings.LoanFilterIncludeChildren)
                        criterion2 = criterion2.Or((QueryCriterion)new XRefValueCriterion("Associateuser.org_id", "org_descendents.descendent", (QueryCriterion)new OrdinalValueCriterion("org_descendents.oid", (object)ReportSettings.LoanFilterOrganizationId)));
                    queryCriterion = queryCriterion != null ? queryCriterion.And(criterion2) : criterion2;
                    break;
                case ReportLoanFilterType.UserGroup:
                    QueryCriterion criterion3 = (QueryCriterion)new OrdinalValueCriterion("AssociateGroup.GroupID", (object)ReportSettings.LoanFilterUserGroupId);
                    queryCriterion = queryCriterion != null ? queryCriterion.And(criterion3) : criterion3;
                    break;
            }
            if (ReportSettings.DynamicQueryCriterion != null)
                queryCriterion = queryCriterion != null ? queryCriterion.And(ReportSettings.DynamicQueryCriterion) : ReportSettings.DynamicQueryCriterion;
            return queryCriterion;
        }
    }
}
