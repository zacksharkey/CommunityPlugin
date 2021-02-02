using CommunityPlugin.Non_Native_Modifications.SideMenu.Controls;
using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.SideMenu
{
    public class SideMenuUI
    {
        public static bool Created = false;
        private static Form EncompassForm = FormWrapper.EncompassForm;
        public const int PluginWidth = 350;
        private static MenuButton menuButton;

        public static void Closing()
        {
            SideMenuUI.Created = false;
        }

        public static void Click()
        {
            if (SideMenuUI.menuButton == null)
                return;
            SideMenuUI.menuButton.PerformClick();
        }

        public static async void CreateMenu(bool load)
        {
            if (SideMenuUI.EncompassForm == null || SideMenuUI.Created)
                return;
            try
            {
                Panel _rightPanel = await Task.Run<Panel>((Func<Panel>)(() =>
                {
                    while (SideMenuUI.RightPanel() == null)
                        Thread.Sleep(500);
                    return SideMenuUI.RightPanel();
                }));
                if (!SideMenuUI.Created)
                {
                    SideMenuUI.RemoveControlById("pnlMain", _rightPanel);
                    SideMenuUI.RemoveControlById("pnlMenuButton", _rightPanel);
                    if (load)
                    {
                        SideMenuUI.menuButton = new MenuButton();
                        SideMenuUI.menuButton.BackColor = Color.White;
                        MenuPanel MenuPanel = new MenuPanel(SideMenuUI.GetMenu(), SideMenuUI.menuButton);
                        MenuPanel.Name = "pnlMain";
                        MenuPanel.Dock = DockStyle.Right;
                        MenuPanel.AutoScroll = false;
                        MenuPanel.HorizontalScroll.Enabled = false;
                        MenuPanel.AutoScroll = true;
                        MenuPanel.Visible = false;
                        Panel panel = new Panel();
                        panel.Name = "pnlMenuButton";
                        panel.Width = 27;
                        panel.Dock = DockStyle.Right;
                        panel.Controls.Add((Control)SideMenuUI.menuButton);
                        _rightPanel.Controls.Add((Control)MenuPanel);
                        _rightPanel.Controls.Add((Control)panel);
                        SideMenuUI.Created = true;
                        _rightPanel = (Panel)null;
                        SideMenuUI.menuButton = (MenuButton)null;
                        MenuPanel = (MenuPanel)null;
                        panel = (Panel)null;
                        MenuPanel = (MenuPanel)null;
                        panel = (Panel)null;
                    }
                }
                _rightPanel = (Panel)null;
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(SideMenuUI));
            }
        }

        public static List<MenuPanelSection> GetMenu()
        {
            List<MenuPanelSection> source = new List<MenuPanelSection>();
            foreach (System.Type type in ((IEnumerable<System.Type>)typeof(SideMenuUI).Assembly.GetTypes()).Where<System.Type>((Func<System.Type, bool>)(type => type.IsSubclassOf(typeof(LoanMenuControl)))).ToList<System.Type>())
            {
                LoanMenuControl loanMenuControl = Activator.CreateInstance(type) as LoanMenuControl;
                if (loanMenuControl != null && (loanMenuControl.CanRun()) && source.FirstOrDefault<MenuPanelSection>((Func<MenuPanelSection, bool>)(x => x.GetType() == loanMenuControl.GetType())) == null)
                {
                    source.Add(new MenuPanelSection(SideMenuUI.GetHeading(loanMenuControl.Name), true, (loanMenuControl.CanShow() ? 1 : 0) != 0, new Control[1]
                    {
            (Control) loanMenuControl
                    }));
                    loanMenuControl.RunBase();
                }
            }
            return source;
        }

        public static Panel RightPanel()
        {
            try
            {
                return SideMenuUI.EncompassForm.Controls.Find("rightPanel", true)[0] as Panel;
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(SideMenuUI));
                return (Panel)null;
            }
        }

        public static TabControl TabControl()
        {
            try
            {
                return SideMenuUI.MainForm().Controls.Find("tabControl", true)[0] as TabControl;
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(SideMenuUI));
                return (TabControl)null;
            }
        }

        public static Form MainForm()
        {
            return SideMenuUI.FindForm("Encompass");
        }

        private static Form FindForm(string name)
        {
            Form form = (Form)null;
            foreach (Form openForm in (ReadOnlyCollectionBase)Application.OpenForms)
            {
                if (openForm.Text.ToLower().Contains("encompass"))
                {
                    form = openForm;
                    SideMenuUI.EncompassForm = form;
                    break;
                }
            }
            return form;
        }

        public static void RemoveControlById(string controlID, Panel panel)
        {
            Control[] controlArray = panel.Controls.Find(controlID, true);
            if (((IEnumerable<Control>)controlArray).Count<Control>() <= 0)
                return;
            for (int index = 0; index < ((IEnumerable<Control>)controlArray).Count<Control>(); ++index)
                controlArray[index].Parent.Controls.Remove(controlArray[index]);
        }

        private static Label GetHeading(string text)
        {
            Label label = new Label();
            label.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label.AutoSize = true;
            label.BackColor = Color.FromArgb(227, 227, 227);
            label.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            label.Location = new Point(0, 0);
            label.Margin = new Padding(0);
            label.Name = "lbl1";
            label.Padding = new Padding(7);
            label.Size = new Size(350, 23);
            label.TabIndex = 0;
            label.Text = text;
            return label;
        }
    }
}
