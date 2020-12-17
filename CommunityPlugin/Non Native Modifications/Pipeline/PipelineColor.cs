using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.DataEngine;
using EllieMae.EMLite.UI;
using System;
using System.Drawing;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class PipelineColor : Plugin, IPipelineTabChanged
    {
        public override void PipelineTabChanged(object sender, EventArgs e)
        {
            try
            {
                GridView pipeline = FormWrapper.GetPipeline();
                pipeline.Items.Change += Items_Change;

                CheckRules();
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, "OpenReadOnly");
            }
        }

        private void Items_Change(object sender, EventArgs e)
        {
            CheckRules(sender as GVItemCollection);
        }

        private void CheckRules(GVItemCollection Items = null)
        {
            if (Items == null)
                return;

            foreach(GVItem item in Items)
            {
                PipelineInfo info = item.Tag as PipelineInfo;

                if(info.Info.ContainsKey("Loan.LoanAmount") && info.Info["Loan.LoanAmount"] != null && (((decimal)info.Info["Loan.LoanAmount"]) > 500000))
                {
                    item.BackColor = Color.Green;
                }
            }
        }
    }
}
