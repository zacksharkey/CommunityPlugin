using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models;
using CommunityPlugin.Properties;
using EllieMae.EMLite.DataEngine;
using EllieMae.EMLite.UI;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.Client;
using Newtonsoft.Json;
using System;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class Doorbell : Plugin, ILogin, ILoanClosing, IPipelineTabChanged, IDataExchangeReceived
    {
        private bool Hide;
        private ToolStripItem DoorBellItem;
        private string DingBackID;
        private string DingBackBorrower;
        private GridView Pipeline;
        private bool SendOutOfFileMessage;
        private PipelineInfo Tag;

        public override void Login(object sender, EventArgs e)
        {
            DoorBellItem = new ToolStripMenuItem("DoorBell");
        }

        public override void LoanClosing(object sender, EventArgs e)
        {
            if (SendOutOfFileMessage)
                DingBack(string.Empty);

            SendOutOfFileMessage = false;
            DingBackID = string.Empty;
            DingBackBorrower = string.Empty;
        }

        public override void PipelineTabChanged(object sender, EventArgs e)
        {
            Pipeline = FormWrapper.GetPipeline();
            Pipeline.ContextMenuStrip.Opened += ContextMenuStrip_Opened;
        }

        private void ContextMenuStrip_Opened(object sender, EventArgs e)
        {
            if (Pipeline == null)
                Pipeline = FormWrapper.GetPipeline();
            if (Pipeline.SelectedItems.Count < 1)
                return;

            Tag = Pipeline.SelectedItems[0].Tag as PipelineInfo;
            if (!string.IsNullOrEmpty(Tag.LockInfo.LockedBy))
            {
                if (!Pipeline.ContextMenuStrip.Items.Contains(DoorBellItem))
                {
                    Pipeline.ContextMenuStrip.Items.Insert(0, DoorBellItem);

                    DoorBellItem.Click -= DoorBellItem_Click;
                    DoorBellItem.Click += DoorBellItem_Click;
                }
            }
            else
            {
                if (Pipeline.ContextMenuStrip.Items.Contains(DoorBellItem))
                {
                    Pipeline.ContextMenuStrip.Items.Remove(DoorBellItem);
                }
            }
        }

        private void DoorBellItem_Click(object sender, EventArgs e)
        {
            ToolStripItem Item = sender as ToolStripItem;
            if (Item.Text.Equals("DoorBell"))
                RingDoorbell();
        }

        private void RingDoorbell()
        {
            DataExchange data = EncompassApplication.Session.DataExchange;
            string lockInfo = $"{EncompassHelper.User.FullName} Is Trying to Access Loan File #{Tag.LoanNumber}, Please Exit the File For A Moment";
            data.PostDataToUser(Tag.LockInfo.LockedBy, lockInfo);
        }

        private void DingBack(string LoanNumber)
        {
            DataExchange data = EncompassApplication.Session.DataExchange;
            string loanNumber = !string.IsNullOrEmpty(LoanNumber) ? LoanNumber : EncompassApplication.CurrentLoan.LoanNumber;
            string locked = $"{EncompassHelper.User.FullName} Is Out Of {DingBackBorrower} Loans #{LoanNumber}";
            data.PostDataToUser(DingBackID, locked);
        }

        public override void DataExchangeReceived(object sender, DataExchangeEventArgs e)
        {
            bool isDoorbell = e.Data.ToString().Contains("Is Out Of") || e.Data.ToString().Contains("Trying to Access");
            if (!Hide && isDoorbell)
            {
                Hide = false;
                bool exit = e.Data.ToString().Contains("Exit");
                System.Media.SoundPlayer music = new System.Media.SoundPlayer(exit ? Resources.Exit : Resources.Out);
                music.Play();
                EncompassHelper.ShowOnTop("DoorBell Notification", e.Data.ToString());

                if (exit)
                {
                    DingBackID = e.Source.UserID;
                    if (EncompassApplication.CurrentLoan == null)
                    {
                        string loanNumber = e.Data.ToString().Split('#')[1].Split(',')[0];
                        DingBack(loanNumber);
                    }
                    else
                    {
                        SendOutOfFileMessage = true;
                        DingBackBorrower = $"{EncompassApplication.CurrentLoan.Fields["4002"].FormattedValue}, {EncompassApplication.CurrentLoan.Fields["4000"].FormattedValue}";
                    }
                }
            }
        }
    }
}
