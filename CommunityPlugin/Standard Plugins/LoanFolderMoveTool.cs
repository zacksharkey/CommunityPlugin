using CommunityPlugin.Non_Native_Modifications;
using CommunityPlugin.Non_Native_Modifications.TopMenu;
using CommunityPlugin.Objects;
using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models;
using EllieMae.EMLite.RemotingServices;
using EllieMae.Encompass.BusinessObjects;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Linq;

namespace CommunityPlugin.Standard_Plugins
{
    public class LoanFolderMoveTool:Plugin, ILoanOpened, ICommitted
    {
        FolderMoveRequest Request;
        public override void Configure()
        {
            LoanFolderMove_Form f = new LoanFolderMove_Form();
            f.ShowDialog();
        }
        public override void LoanOpened(object sender, EventArgs e) { }
        public override void Committed(object sender, EventArgs e)
        {
            Loan l = EncompassHelper.CurrentLoan;
            string folder = l.LoanFolder;
            LoanFolderRules rules = CustomDataObject.Get<LoanFolderRules>();

            foreach (LoanFolderRule rule in rules.Rules.Where(x => x.Active && !x.FolderName.Equals(folder)).OrderBy(x => x.Order))
            {
                if (rule.Calculate())
                {
                    Request = new FolderMoveRequest()
                    {
                        LoanFolder = rule.FolderName,
                        GUID = l.Guid,
                        LoanName = l.LoanName
                    };

                    FormWrapper.TabControl.ControlRemoved += TabControl_ControlRemoved;
                }
            }
        }

        private void TabControl_ControlRemoved(object sender, System.Windows.Forms.ControlEventArgs e)
        {
            if(Request != null)
            {
                try
                {
                    EllieMae.EMLite.DataEngine.LoanIdentity i = new EllieMae.EMLite.DataEngine.LoanIdentity(Request.LoanFolder, Request.LoanName, Request.GUID);
                    Session.LoanManager.MoveLoan(i, Request.LoanFolder, EllieMae.EMLite.ClientServer.DuplicateLoanAction.Rename);
                }
                catch(Exception ex)
                {
                    Logger.HandleError(ex, nameof(LoanFolderMoveTool));
                }
                Request = null;

            }

            FormWrapper.TabControl.ControlRemoved -= TabControl_ControlRemoved;
        }
    }
}
