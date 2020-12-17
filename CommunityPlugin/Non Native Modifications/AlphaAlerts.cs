using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.UI;
using System;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class AlphaAlerts : Plugin, ILoanTabChanged
    {
        private bool Should => EncompassHelper.Val("CX.ALERTS.ALPHA").ToString().Equals("X");
        public override void LoanTabChanged(object sender, EventArgs e)
        {
            Timer t = new Timer();
            t.Interval = 1000;
            t.Enabled = true;
            t.Tick += T_Tick;
        }

        private void T_Tick(object sender, EventArgs e)
        {
            Timer t = sender as Timer;
            t.Enabled = false;

            Alpha();
        }

        private void Alpha()
        {
            try
            {
                if(Should)
                {
                    GridView grid = FormWrapper.EncompassForm.Controls.Find("gvAlerts", true)[0] as GridView;
                    if(grid != null)
                    {
                        grid.StopEditing();
                        grid.Sort(0, SortOrder.Ascending);
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.HandleError(ex, nameof(AlphaAlerts));
            }
        }
    }
}
