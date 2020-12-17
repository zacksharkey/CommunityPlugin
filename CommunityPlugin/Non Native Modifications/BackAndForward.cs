using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Properties;
using EllieMae.Encompass.Automation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class BackAndForward : Plugin, ILogin, ITabChanged, ILoanTabChanged
    {
        private Panel ButtonsPanel = new Panel();
        private Button Back = new Button();
        private Button Forward = new Button();
        private List<string> ReverseForms = new List<string>();
        private List<string> ForwardForms = new List<string>();
        private List<string> AllForms = new List<string>();
        private bool Inserted = true;
        private Panel InputFormsPanel;
        private string CurrentForm;

        public override void Login(object sender, EventArgs e) { }

        public override void LoanTabChanged(object sender, EventArgs e)
        {
            AddControls();
        }

        public override void TabChanged(object sender, EventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            TabPage tabPage = tabControl.TabPages[tabControl.SelectedIndex];
            if (tabPage == null || tabPage.Name != "loanTabPage")
                RemoveControls();
        }

        private void RemoveControls()
        {
            try
            {
                InputFormsPanel.Controls.Remove((Control)ButtonsPanel);
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(BackAndForward));
            }
        }

        private void AddControls()
        {
            new Timer() { Interval = 1000, Enabled = true }.Tick += new EventHandler(T_Tick);
        }

        private void T_Tick(object sender, EventArgs e)
        {
            (sender as Timer).Enabled = false;
            Forward = new Button();
            Back = new Button();
            if (EncompassApplication.CurrentLoan == null)
                return;

            ListBox listBox1 = FormWrapper.EncompassForm.Controls.Find("emFormMenuBox", true)[0] as ListBox ?? (ListBox)null;
            if (listBox1 != null)
            {
                listBox1.SelectedIndexChanged -= new EventHandler(FormBox_SelectedIndexChanged);
                listBox1.SelectedIndexChanged += new EventHandler(FormBox_SelectedIndexChanged);
            }
            ListBox listBox2 = FormWrapper.EncompassForm.Controls.Find("emToolMenuBox", true)[0] as ListBox ?? (ListBox)null;
            if (listBox2 != null)
            {
                listBox2.SelectedIndexChanged -= new EventHandler(ToolsBox_SelectedIndexChanged);
                listBox2.SelectedIndexChanged += new EventHandler(ToolsBox_SelectedIndexChanged);
            }
            InputFormsPanel = listBox1.Parent.Parent.Parent as Panel;
            if (InputFormsPanel != null)
            {
                if (InputFormsPanel.Controls.Contains((Control)ButtonsPanel))
                    RemoveControls();
                ButtonsPanel = new Panel();
                InputFormsPanel.Controls.Add((Control)ButtonsPanel);
                Back.MouseDown -= new MouseEventHandler(BackButton_Click);
                Back.MouseDown += new MouseEventHandler(BackButton_Click);
                Forward.MouseDown -= new MouseEventHandler(ForwardButton_Click);
                Forward.MouseDown += new MouseEventHandler(ForwardButton_Click);
            }
            ReverseForms.Clear();
            ForwardForms.Clear();
            AllForms.Clear();
            CurrentForm = string.Empty;
            ButtonsPanel.Name = "ButtonPanel";
            ButtonsPanel.Size = new Size(55, 22);
            ButtonsPanel.Location = new Point(202, -1);
            ButtonsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Back.Name = "BackButton";
            Back.Text = "";
            Back.Size = new Size(26, 22);
            Back.Image = (Image)Resources.Back;
            Back.EnabledChanged += new EventHandler(BackButton_EnabledChanged);
            Forward.Name = "ForwardButton";
            Forward.Text = "";
            Forward.Size = new Size(26, 22);
            Forward.Image = (Image)Resources.Forward;
            ButtonsPanel.Controls.Add((Control)Back);
            Back.Location = new Point(1, 0);
            ButtonsPanel.Controls.Add((Control)Forward);
            Forward.Location = new Point(28, 0);
            ButtonAccess();
            ButtonsPanel.BringToFront();
        }

        private void BackButton_EnabledChanged(object sender, EventArgs e)
        {
            Button button = sender as Button;
        }

        private void ToolsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormsList((sender as ListBox).Text);
        }

        private void FormBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormsList((sender as ListBox).Text);
        }

        private void ForwardButton_Click(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            GoToForm(false);
        }

        private void BackButton_Click(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            GoToForm(true);
        }

        private void GoToForm(bool back = false)
        {
            try
            {
                Inserted = false;
                (EncompassApplication.Screens[EncompassScreen.Loans] as LoansScreen).GoToForm(AllForms[AllForms.LastIndexOf(CurrentForm) + (back ? 1 : -1)]);
                ButtonAccess();
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(BackAndForward));
            }
        }

        private void ButtonAccess()
        {
            Back.Enabled = false;
            Forward.Enabled = false;
            if (AllForms.Count > 1 && AllForms.LastIndexOf(CurrentForm) != AllForms.Count - 1)
                Back.Enabled = true;
            if (!AllForms.Contains(CurrentForm) || !(AllForms[0] != CurrentForm))
                return;
            Forward.Enabled = true;
        }

        private void UpdateFormsList(string FormName)
        {
            if (string.IsNullOrEmpty(FormName) || CurrentForm == FormName)
                return;
            if (Inserted)
            {
                if (AllForms.Contains(FormName))
                    AllForms.Remove(FormName);
                AllForms.Insert(0, FormName);
            }
            CurrentForm = FormName;
            ButtonAccess();
            Inserted = true;
        }
    }
}
