using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.UI;
using System;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.Pipeline
{
    public class ShowColumnField : Plugin, INativeFormLoaded
    {
        private GridView Grid;
        public override void NativeFormLoaded(object sender, FormOpenedArgs e)
        {
            if (e.OpenForm.Name.Equals("TableLayoutColumnSelector"))
            {
                Grid = e.OpenForm.Controls.Find("gvColumns", true)[0] as GridView;
                if (Grid != null)
                {
                    Grid.Columns.Add(new GVColumn("Field ID"));
                    Timer t = new Timer();
                    t.Interval = 1000;
                    t.Enabled = true;
                    t.Tick += T_Tick;
                }
            }
        }

        private void T_Tick(object sender, EventArgs e)
        {
            Timer t = sender as Timer;
            t.Enabled = false;
            if (Grid != null)
            {
                foreach (GVItem item in Grid.Items)
                {
                    item.SubItems[1].Text = ((EllieMae.EMLite.ClientServer.TableLayout.Column)item.Value).Tag;
                }
            }
        }
    }
}
