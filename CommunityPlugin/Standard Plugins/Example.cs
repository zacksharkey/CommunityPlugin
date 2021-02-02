using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Interface;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.BusinessObjects.Loans.Logging;
using System;

namespace CommunityPlugin.Standard_Plugins
{
    public class Example: Plugin, ILoanOpened, IFieldChange, ILogEntryChanged
    {
        public override void LoanOpened(object sender, EventArgs e) { }

        public override void LogEntryChanged(object sender, LogEntryEventArgs e)
        {
            if (e.LogEntry.EntryType == LogEntryType.TrackedDocument)
            {
                TrackedDocument doc = (TrackedDocument)e.LogEntry;
                if (doc.Title.Equals("Closing Disclosure") || doc.Title.Equals("Closing Disclosure (Alternate)"))
                {
                    doc.ShippingReadyDate = DateTime.Now;
                }
            }
        }
    }
}
