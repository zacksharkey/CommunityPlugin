using CommunityPlugin.Objects;
using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.Common.UI;
using EllieMae.EMLite.DataEngine;
using EllieMae.EMLite.RemotingServices;
using EllieMae.EMLite.UI;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Users;
using EllieMae.Encompass.Collections;
using System;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class VIP : Plugin, IPipelineTabChanged
    {
        public override void PipelineTabChanged(object sender, EventArgs e)
        {
            GridView gridView = FormWrapper.GetPipeline();
            if (gridView == null || gridView.ContextMenuStrip.Items.Count.Equals(0))
                return;

            if (EncompassHelper.IsSuper)
            {
                ToolStripItem readOnly = (ToolStripItem)NewItem(nameof(VIP));

                if (!gridView.ContextMenuStrip.Items.Contains(readOnly))
                    gridView.ContextMenuStrip.Items.Insert(0, readOnly);
                else
                    gridView.ContextMenuStrip.Items.Remove(readOnly);
            }

            gridView.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            gridView.ItemDoubleClick += GridView_ItemDoubleClick;
        }

        private void GridView_ItemDoubleClick(object source, GVItemEventArgs e)
        {
            VIPCDO cdo = CustomDataObject.Get<VIPCDO>();

            if (cdo.Loans.Contains(EncompassApplication.CurrentLoan.Guid))
            {
                UserGroup group = EncompassApplication.Session.Users.Groups.GetGroupByName("VIP");
                if (group == null)
                    return;

                UserList users = group.GetUsers();
                if (!users.Contains(EncompassApplication.CurrentUser))
                {
                    Session.Application.GetService<ILoanConsole>().CloseLoanWithoutPrompts(false);
                    EncompassHelper.ShowOnTop("VIP", "You do not have access to this loan.");
                }
            }
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ContextMenuStrip menu = sender as ContextMenuStrip;
            ToolStripItem vip = menu.Items.Cast<ToolStripItem>().Where(x => x.Text.Contains(nameof(VIP))).FirstOrDefault();
            if (vip != null)
            {
                VIPCDO cdo = CustomDataObject.Get<VIPCDO>();
                GVItem selected = FormWrapper.GetPipeline().SelectedItems.FirstOrDefault();
                vip.Text = "Mark As VIP";

                if (cdo.Loans.Contains((selected?.Tag as PipelineInfo).GUID))
                    vip.Text = "Marked VIP";
            }
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
            VIPCDO cdo = CustomDataObject.Get<VIPCDO>();
            string guid = (gridView.SelectedItems.FirstOrDefault().Tag as PipelineInfo).GUID;
            if (cdo.Loans.Contains(guid))
                cdo.Loans.Remove(guid);
            else
                cdo.Loans.Add(guid);


            CustomDataObject.Save<VIPCDO>(cdo);
        }
    }
}
