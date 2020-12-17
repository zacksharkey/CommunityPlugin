using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.BusinessObjects.Loans.Logging;
using EllieMae.Encompass.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommunityPlugin.Standard_Plugins
{
    public class VirtualFields : Plugin, ILoanOpened, ILogEntryAdded, ILogEntryChanged
    {
        private List<FieldDescriptor> DocReceivedFields;
        private List<FieldDescriptor> DocAttachmentFields;
        private bool CanRun = false;


        public override void LoanOpened(object sender, EventArgs e)
        {
            if (EncompassApplication.Session.Loans.FieldDescriptors.CustomFields.Contains("CX.LOANOPEN"))
            {
                EncompassHelper.Set("CX.LOANOPEN", string.Empty);
                EncompassHelper.Set("CX.LOANOPEN", "X");
            }

            DocReceivedFields = EncompassHelper.GetPrefixedFields("CX.DOCRECEIVED.");
            DocAttachmentFields = EncompassHelper.GetPrefixedFields("CX.DOCATTACH.");
            CanRun = DocReceivedFields.Count > 0 || DocAttachmentFields.Count > 0;

            RunChecks();
        }

        private void RunChecks(LogEntry Entry = null)
        {
            //Add more virtual fields here
            CheckDocumentAttachments(Entry);
            CheckDocumentRecentAttachment(Entry);
        }

        public override void LogEntryAdded(object sender, LogEntryEventArgs e)
        {
            RunChecks(e.LogEntry);
        }

        public override void LogEntryChanged(object sender, LogEntryEventArgs e)
        {
            RunChecks(e.LogEntry);
        }

        private void CheckDocumentAttachments(LogEntry Entry = null)
        {
            if (!CanRun)
                return;

            if (DocReceivedFields == null || DocReceivedFields.Count().Equals(0))
                return;

            if (Entry != null && (!Entry.EntryType.Equals(LogEntryType.TrackedDocument) || !DocReceivedFields.Any(x => x.Description.Equals((Entry as TrackedDocument).Title))))
                return;

            foreach(FieldDescriptor field in DocReceivedFields)
            {
                if(!string.IsNullOrEmpty(field.Description))
                {
                    AttachmentList attachments = GetAttachments(field);
                    if (attachments == null)
                        continue;

                    EncompassHelper.Set(field.FieldID, attachments.Count.ToString());
                }
            }
        }

        private void CheckDocumentRecentAttachment(LogEntry Entry = null)
        {
            if (!CanRun)
                return;

            if (DocAttachmentFields == null || DocAttachmentFields.Count().Equals(0))
                return;

            if (Entry != null && (!Entry.EntryType.Equals(LogEntryType.TrackedDocument) || !DocAttachmentFields.Any(x => x.Description.Equals((Entry as TrackedDocument).Title))))
                return;

            foreach (FieldDescriptor field in DocAttachmentFields)
            {
                if (!string.IsNullOrEmpty(field.Description))
                {
                    AttachmentList attachments = GetAttachments(field);
                    if (attachments == null || attachments.Count == 0)
                        continue;
                    DateTime newest = attachments.Cast<Attachment>().OrderByDescending(x => x.Date).FirstOrDefault().Date;
                    EncompassHelper.Set(field.FieldID, newest.ToString());
                }
            }
        }

        private AttachmentList GetAttachments(FieldDescriptor Field, LogEntry Entry = null)
        {
            AttachmentList result = new AttachmentList();
            if (Field == null)
                return result;

            if (Entry != null && (!Entry.EntryType.Equals(LogEntryType.TrackedDocument) || !Field.Description.Equals((Entry as TrackedDocument).Title)))
                return result;


            if (!string.IsNullOrEmpty(Field.Description))
            {
                CollectionBase documents = (CollectionBase)EncompassHelper.Loan.Log.TrackedDocuments.GetDocumentsByTitle(Field.Description);
                if (documents == null || documents.Count.Equals(0))
                    return result;

                foreach (TrackedDocument document in documents)
                {
                    AttachmentList attachments = document.GetAttachments();
                    if (attachments == null || attachments.Count.Equals(0))
                        continue;
                    foreach (Attachment a in attachments)
                        result.Add(a);
                }
            }

            return result;
        }
    }
}
