using CommunityPlugin.Config;
using CommunityPlugin.Objects;
using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.ClientServer.Authentication;
using EllieMae.EMLite.Common.UI;
using EllieMae.EMLite.DataEngine;
using EllieMae.EMLite.DataEngine.Log;
using EllieMae.EMLite.eFolder.LoanCenter;
using EllieMae.EMLite.RemotingServices;
using EllieMae.EMLite.WebServices;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommunityPlugin.Standard_Plugins
{
    public class RetrieveDocuments: Plugin, ILoanOpened, IBeforeCommit
    {
        private RetrieveDocumentsCDO Config;
        public override void Configure()
        {
            RetrieveDocuments_Config form = new RetrieveDocuments_Config();
            form.ShowDialog();
        }
        public override void LoanOpened(object sender, EventArgs e)
        {
            Config = CustomDataObject.Get<RetrieveDocumentsCDO>(RetrieveDocumentsCDO.Key);
        }

        public override void BeforeCommit(object sender, CancelableEventArgs e)
        {
            if (string.IsNullOrEmpty(Config.FieldID))
                return;

            //Some set of conditions maybe not run this on every loan save
            LoanDataMgr mgr = Session.DefaultInstance.LoanDataMgr;
            RetrySettings settings = new RetrySettings(mgr.SessionObjects); 
            ReauthenticateOnUnauthorised reauthenticateOnUnauthorised = new ReauthenticateOnUnauthorised(mgr.SessionObjects.StartupInfo.ServerInstanceName, mgr.SessionObjects.StartupInfo.SessionID, mgr.SessionObjects.StartupInfo.OAPIGatewayBaseUri, settings);
            List<DownloadLog> downloadList = new List<DownloadLog>();
            try
            {
                string tokenType = string.Empty;
                string accessToken = string.Empty;
                IEFolder eFolderMgr = Session.Application.GetService<IEFolder>();
                reauthenticateOnUnauthorised.Execute("sc", (Action<AccessToken>)(AccessToken =>
                {
                    if (eFolderMgr.IsConsumerConnectLoan(mgr, false) && AccessToken != null)
                    {
                        tokenType = AccessToken.Type;
                        accessToken = AccessToken.Token;
                    }
                    using (InboxServiceWse inboxServiceWse = new InboxServiceWse(tokenType, accessToken))
                    {
                        foreach (InboxFile file in inboxServiceWse.GetFiles(Session.CompanyInfo.ClientID, mgr.LoanData.GUID.Replace("{", string.Empty).Replace("}", string.Empty)))
                            downloadList.Add(new DownloadLog()
                            {
                                FileSource = file.FileSource,
                                FileID = file.FileID,
                                FileType = file.FileType,
                                Title = file.Title,
                                Sender = file.Sender,
                                DocumentID = file.DocumentID,
                                BarcodePage = file.BarcodePage,
                                Date = file.ReceivedDate.ToLocalTime()
                            });
                    }
                }));

                DownloadLog[] docs = Session.DefaultInstance.LoanDataMgr.LoanData.GetLogList().GetAllDownloads();
                bool needDownload = false;
                foreach (DownloadLog availableDownload in RetrieveDownloadDialog.GetAvailableDownloads(Session.DefaultInstance.LoanDataMgr, (RetrySettings)null))
                {
                    needDownload = false;
                    foreach (DownloadLog doc in docs)
                    {
                        if (doc.FileSource == availableDownload.FileSource && doc.FileID == availableDownload.FileID)
                            needDownload = true;
                    }
                    if (!needDownload)
                    {
                        EncompassApplication.CurrentLoan.Fields[Config.FieldID].Value = !needDownload ? "X" : string.Empty;
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(RetrieveDocuments));
            }
        }
    }
}
