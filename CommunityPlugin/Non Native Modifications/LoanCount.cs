using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.UI;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.Query;
using EllieMae.Encompass.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class LoanCount : Plugin, INativeFormLoaded
    {
        private TreeView Tree;
        private Form SettingsForm;
        private GridView folders;

        public override void NativeFormLoaded(object sender, FormOpenedArgs e)
        {
            Form openedForm = e.OpenForm;
            if (openedForm == null || (openedForm.IsDisposed || !openedForm.Text.Equals("Encompass  Settings")))
                return;
            this.SettingsForm = openedForm;
            this.SettingsWindow_Activate();
        }
        public void SettingsWindow_Activate()
        {
            if (this.Tree != null)
                return;
            this.Tree = this.GetControl((Control)this.SettingsForm, "treeView") as TreeView;
            if (this.Tree != null)
            {
                this.Tree.AfterSelect += new TreeViewEventHandler(this.Tree_AfterSelect);
                this.SettingsForm.FormClosing += (FormClosingEventHandler)((sender, e) => this.Tree = (TreeView)null);
            }
        }

        private void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeView Tree = sender as TreeView;
            if (Tree.SelectedNode.FullPath.Equals("Loan Setup\\Loan Folders"))
            {
                folders = GetControl((Control)this.SettingsForm, "gvLoanFolders") as GridView;
                if (folders != null)
                {
                    GVColumn column = folders.Columns.Where(x => x.Name.Equals("Count")).FirstOrDefault();
                    if (column != null)
                        folders.Columns.Remove(column);
                    column = new GVColumn("Count");
                    folders.Columns.Add(column);

                    Timer t = new Timer();
                    t.Interval = 100;
                    t.Enabled = true;
                    t.Tick += T_Tick;
                }
            }
        }

        private void T_Tick(object sender, EventArgs e)
        {
            Timer t = sender as Timer;
            t.Enabled = false;
            foreach (GVItem row in folders.Items)
            {
                row.SubItems[3].Text = EncompassApplication.Session.Loans.Query(new StringFieldCriterion("Loan.LoanFolder", row.SubItems[0].Text, StringFieldMatchType.Exact, true))?.Count.ToString() ?? "0";
            }
        }

        private Control GetControl(Control Parent, string Name)
        {
            if (Parent == null || string.IsNullOrEmpty(Name))
                return (Control)null;
            return ((IEnumerable<Control>)Parent.Controls.Find(Name, true)).FirstOrDefault<Control>();
        }
    }
}
