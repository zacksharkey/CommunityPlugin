using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.Common.UI;
using EllieMae.EMLite.DataEngine;
using EllieMae.EMLite.RemotingServices;
using EllieMae.EMLite.UI;
using System;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.Pipeline
{
    public class OpenReadOnly : Plugin, IPipelineTabChanged
    {
        private bool Added;
        private GridView PipelineGrid;
        public override void PipelineTabChanged(object sender, EventArgs e)
        {
            try
            {
                AddOrRemoveContextItem();
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, "OpenReadOnly");
            }
        }

        private void AddOrRemoveContextItem()
        {
            PipelineGrid = FormWrapper.GetPipeline();
            ToolStripItem toolStripItem = (ToolStripItem)new ToolStripMenuItem("Open Read Only");
            toolStripItem.Click += new EventHandler(Item_Click1);
            if (!Added)
            {
                PipelineGrid.ContextMenuStrip.Items.Insert(0, toolStripItem);
                Added = true;
            }
            else
                PipelineGrid.ContextMenuStrip.Items.Remove(toolStripItem);
        }

        private void Item_Click1(object sender, EventArgs e)
        {
            ToolStripItem toolStripItem = sender as ToolStripItem;
            if (!toolStripItem.Text.Equals("Open Read Only"))
                return;
            ReadOnly(toolStripItem);
        }

        private void ReadOnly(ToolStripItem item)
        {
            Sessions.Session defaultInstance = Session.DefaultInstance;
            if (item == null || defaultInstance == null)
                return;

            GVItem gvItem = ((item.GetCurrentParent() as ContextMenuStrip).SourceControl as GridView).SelectedItems.FirstOrDefault<GVItem>();
            if (gvItem == null)
                return;

            string guid = (gvItem.Tag as PipelineInfo).GUID;
            if (string.IsNullOrEmpty(guid))
                return;

            if (MessageBox.Show(FormWrapper.EncompassForm, "You only have read-only access to this loan file. You won't be able to save any changes. Do you still want to open this loan?", "Read Only", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                defaultInstance.Application.GetService<ILoanConsole>().OpenLoan(guid, LoanInfo.LockReason.NotLocked, true);
            }
        }
    }
}
