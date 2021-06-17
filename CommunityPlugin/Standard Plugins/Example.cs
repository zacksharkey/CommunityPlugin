using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.DataEngine.Log;
using EllieMae.EMLite.RemotingServices;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.BusinessObjects.Loans.Logging;
using System;
using System.Linq;

namespace CommunityPlugin.Standard_Plugins
{
    public class Example: Plugin, ILoanOpened, ILogEntryChanged, IFieldChange
    {
        public override void Configure()
        {
            Example_Form f = new Example_Form();
            f.ShowDialog();
        }
        public override void LoanOpened(object sender, EventArgs e) { }

        public override void FieldChanged(object sender, FieldChangeEventArgs e)
        {
            IDisclosureTracking2015Log[] idisclosureTracking2015Log = Session.DefaultInstance.LoanData.GetLogList().GetAllIDisclosureTracking2015Log(false);
            foreach (IDisclosureTracking2015Log disclosureTracking2015Log in idisclosureTracking2015Log)
            {
                disclosureTracking2015Log.IntentToProceed = true;
                disclosureTracking2015Log.BorrowerDisclosedMethod = DisclosureTrackingBase.DisclosedMethod.Email;
                disclosureTracking2015Log.BorrowerActualReceivedDate = DateTime.Now;
            }
        }

        public override void LogEntryChanged(object sender, LogEntryEventArgs e)
        {
            //int[] array = new int[] { 1, 2, 3, 4, 5 };
            //int lowest = array.Min(x => x);
            //if (e.LogEntry.EntryType == LogEntryType.TrackedDocument)
            //{
            //    TrackedDocument doc = (TrackedDocument)e.LogEntry;
            //    if (doc.Title.Equals("Closing Disclosure") || doc.Title.Equals("Closing Disclosure (Alternate)"))
            //    {
            //        doc.ShippingReadyDate = DateTime.Now;
            //    }
            //}

        }
    }
}
