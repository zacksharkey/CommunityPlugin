using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.DataEngine;
using EllieMae.EMLite.UI;
using System;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.Pipeline
{
    public class Reassign : Plugin, IPipelineTabChanged
    {

        public override void PipelineTabChanged(object sender, EventArgs e)
        {
            GridView gridView = FormWrapper.GetPipeline();

            ToolStripItem readOnly = (ToolStripItem)NewItem("Loan Reassignment");
            if (!gridView.ContextMenuStrip.Items.Contains(readOnly))
                gridView.ContextMenuStrip.Items.Insert(0, readOnly);
            else
                gridView.ContextMenuStrip.Items.Remove(readOnly);
        }
        private ToolStripMenuItem NewItem(string Name)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(Name);
            item.Click += Item_Click;
            return item;
        }

        private void Item_Click(object sender, EventArgs e)
        {
            GridView gridView = FormWrapper.GetPipeline();
            ReassignmentDialog r = new ReassignmentDialog(gridView.SelectedItems.Select(x => x.Tag as PipelineInfo).ToList<PipelineInfo>());
            r.ShowDialog();
        }
    }
}
