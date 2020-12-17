using CommunityPlugin.Non_Native_Modifications;
using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.DataEngine;
using EllieMae.EMLite.DataEngine.Log;
using EllieMae.EMLite.eFolder;
using EllieMae.EMLite.eFolder.Documents;
using EllieMae.EMLite.RemotingServices;
using EllieMae.EMLite.UI;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Linq;
using System.Timers;

namespace CommunityPlugin.Standard_Plugins
{
    public class OpeneFolderDocument : Plugin, IFieldChange, ILoanOpened
    {
        private string DocumentName;
        public override void LoanOpened(object sender, EventArgs e)
        {

        }

        public override void FieldChanged(object sender, FieldChangeEventArgs e)
        {  
            if (e.FieldID.Equals("CX.OPENDOCUMENT") && !string.IsNullOrEmpty(e.NewValue))
            {
                DocumentName = e.NewValue;
                OpenEfolderDocument();
                EncompassHelper.Set("CX.OPENDOCUMENT", "");
            }
        }

        private void OpenEfolderDocument()
        {
            try
            {
                eFolderDialog e = new eFolderDialog(Session.DefaultInstance.LoanDataMgr, Session.DefaultInstance);
                GridView gvDocuments = (GridView)e.Controls.Find("gvDocuments", true)[0]; if (gvDocuments != null)
                {
                    DocumentLog document = (DocumentLog)gvDocuments.Items.FirstOrDefault(x => ((DocumentLog)x.Tag).Title.Equals(DocumentName)).Tag;
                    DocumentDetailsDialog.ShowInstance(Session.DefaultInstance.LoanDataMgr, document);
                }
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(OpenEfolderDocument), (object)null);
            }
        }
    }
}
