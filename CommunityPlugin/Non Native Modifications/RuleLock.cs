using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Extension;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models;
using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.UI;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class RuleLock : Plugin, INativeFormLoaded,  ILogin, IDataExchangeReceived
    {
        private bool ConcurrentUser => EncompassHelper.User.ID.Equals("rulelock");
        private CheckBox Locked;
        private Form openedForm;


        public override void DataExchangeReceived(object sender, DataExchangeEventArgs e)
        {
            if (!ConcurrentUser)
                return;

            RuleLockInfo info = JsonConvert.DeserializeObject<RuleLockInfo>(e.Data.ToString(), new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            });


            RuleLockCDO cdo = CustomDataObject.Get<RuleLockCDO>(RuleLockCDO.Key);
            cdo.Rules.Add(info);
            CustomDataObject.Save<RuleLockCDO>(RuleLockCDO.Key, cdo);
        }

        public override void NativeFormLoaded(object sender, FormOpenedArgs e)
        {
            openedForm = e.OpenForm;
            if (openedForm == null || (openedForm.IsDisposed || !openedForm.Text.Contains("Trigger")))
                return;

            RuleLockCDO cdo = CustomDataObject.Get<RuleLockCDO>(RuleLockCDO.Key);
            int id = ((EllieMae.EMLite.Setup.TriggerEditor)openedForm.ActiveControl.TopLevelControl).Trigger.RuleID;
            RuleLockInfo locked = cdo.Rules?.FirstOrDefault(x => x.RuleID.Equals(id)) ?? (RuleLockInfo)null;

            TextBox comments = openedForm.AllControls<TextBox>().FirstOrDefault(x => x.Name.Equals("commentsTxt"));
            if (comments == null)
                return;

            Button ok = openedForm.AllControls<Button>().FirstOrDefault(x => x.Name.Equals("okBtn"));
            if (ok == null)
                return;

            ok.Click += Ok_Click;

            FlowLayoutPanel p = new FlowLayoutPanel();
            openedForm.Controls.Add(p);
            p.Left = comments.Left;
            p.Top = comments.Bottom + 20;
            p.Size = comments.Size;
            p.FlowDirection = FlowDirection.LeftToRight;

            Locked = new CheckBox();
            p.Controls.Add(Locked);
            Locked.Text = "Lock Rule";

            TextBox lockedBy = new TextBox();
            p.Controls.Add(lockedBy);

            Button diff = new Button();
            p.Controls.Add(diff);
            diff.Text = "Diff";
            diff.Click += Diff_Click;

            if (locked != null)
            {
                lockedBy.Text = $"Locked By {locked.ID}";
                Locked.Checked = locked.Locked;
                ok.Enabled = locked.Locked && locked.ID.Equals(EncompassHelper.User.ID) ? false : true;
            }
        }

        private void Diff_Click(object sender, EventArgs e)
        {
            //Show Diff
        }


        private void Ok_Click(object sender, EventArgs e)
        {
            TriggerInfo info = ((EllieMae.EMLite.Setup.TriggerEditor)openedForm.ActiveControl.TopLevelControl).Trigger;
            RuleLockInfo f = new RuleLockInfo(info)
            {
                ID = EncompassHelper.User.ID,
                Locked = Locked.Checked,
                RuleName = info.RuleName,
                RuleID = info.RuleID
            };

            DataExchange data = EncompassApplication.Session.DataExchange;
            data.PostDataToUser("rulelock", JsonConvert.SerializeObject(f));
        }
    }
}
