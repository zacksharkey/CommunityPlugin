using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Extension;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class GridSearch : Plugin, ILogin
    {

        private string key = "flpSearch";
        private TreeView Tree;
        private Form SettingsForm;
        private GridView AllDataGrid;
        private GridView Grid;
        private FlowLayoutPanel searchPanel;
        private bool Loading;

        public override void Login(object sender, EventArgs e)
        {
            FormWrapper.FormOpened += new FormOpenedHandler(this.EncompassMainUI_FormOpened);
        }

        private void EncompassMainUI_FormOpened(object sender, FormOpenedArgs e)
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
                this.Tree.BeforeSelect += new TreeViewCancelEventHandler(this.Tree_BeforeSelect);
                this.SettingsForm.FormClosing += (FormClosingEventHandler)((sender, e) => this.Tree = (TreeView)null);
            }
        }

        private void Tree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            Panel control = this.GetControl((Control)this.SettingsForm, "gpHeader") as Panel;
            if (control == null || !control.Controls.ContainsKey(this.key))
                return;
            control.Controls.RemoveByKey(this.key);
        }

        private void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.Grid = SettingsForm.AllControls<GridView>().FirstOrDefault<GridView>();
            if (this.Grid == null || !this.Grid.Items.Any<GVItem>())
                return;
            if (!Loading)
            {
                this.AllDataGrid = new GridView();
                foreach (GVItem gvItem in (IEnumerable<GVItem>)this.Grid.Items)
                    this.AllDataGrid.Items.Add(gvItem);
                Panel control = this.GetControl((Control)this.SettingsForm, "gpHeader") as Panel;
                if (control == null)
                    return;
                if (!control.Controls.ContainsKey(this.key))
                {
                    searchPanel = this.GetSearchPanel(control.Width, this.key);
                    control.Controls.Add((Control)searchPanel);
                    ((IEnumerable<Control>)control.Controls.Find(this.key, true)).FirstOrDefault<Control>().BringToFront();

                    Loading = true;
                    (searchPanel.Controls[0] as TextBox).Text = "_";
                    Timer t = new Timer();
                    t.Interval = 50;
                    t.Enabled = true;
                    t.Tick += T_Tick;
                }
            }
        }

        private void T_Tick(object sender, EventArgs e)
        {
            (searchPanel.Controls[0] as TextBox).Text = "";
            Timer t = sender as Timer;
            t.Enabled = false;
            Loading = false;
        }

        private FlowLayoutPanel GetSearchPanel(int headerWidth, string key)
        {
            try
            {
                TextBox textBox = new TextBox();
                textBox.Name = key;
                textBox.Width = 200;
                textBox.TextChanged += new EventHandler(this.TextBox_TextChanged);
                FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
                flowLayoutPanel.Name = key;
                flowLayoutPanel.Width = textBox.Width + 30;
                flowLayoutPanel.Left = (headerWidth - flowLayoutPanel.Width) / 2;
                flowLayoutPanel.Controls.Add((Control)textBox);
                return flowLayoutPanel;
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(GridSearch));
                return (FlowLayoutPanel)null;
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.AddGridItems((sender as TextBox).Text);
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(GridSearch));
            }
        }

        private void AddGridItems(string Text)
        {
            this.Grid.Items.Clear();
            this.Grid.Items.AddRange(this.AllDataGrid.Items.Where<GVItem>((Func<GVItem, bool>)(x =>
            {
                if (!x.Text.ToLower().Contains(Text.ToLower()))
                    return x.SubItems.Any<GVSubItem>((Func<GVSubItem, bool>)(y => y.Text.ToLower().Contains(Text.ToLower())));
                return true;
            })).ToArray<GVItem>());
        }

        private Control GetControl(Control Parent, string Name)
        {
            if (Parent == null || string.IsNullOrEmpty(Name))
                return (Control)null;
            return ((IEnumerable<Control>)Parent.Controls.Find(Name, true)).FirstOrDefault<Control>();
        }
    }
}
