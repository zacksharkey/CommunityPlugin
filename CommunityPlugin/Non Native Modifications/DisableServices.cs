using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.ePass.Services;
using EllieMae.EMLite.UI;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class DisableServices:Plugin, ILoanTabChanged
    {
        private string ServicesToDisable;

        public override void LoanTabChanged(object sender, EventArgs e)
        {
            bool shouldRun = EncompassApplication.Session.Loans.FieldDescriptors.CustomFields.Cast<FieldDescriptor>().Any(x => x.FieldID.Equals("CX.DISABLE.SERVICES"));
            if (!shouldRun)
                return;

            Timer t = new Timer();
            t.Interval = 1000;
            t.Tick += T_Tick;
            t.Enabled = true;
        }

        private void T_Tick(object sender, EventArgs e)
        {
            Timer t = sender as Timer;
            t.Enabled = false;

            Remove();
        }

        private void Remove()
        {
            ServicesToDisable = EncompassApplication.CurrentLoan.Fields["CX.DISABLE.SERVICES"].FormattedValue.ToLower().Replace(" ", "");

            bool all = ServicesToDisable.Equals("all", StringComparison.OrdinalIgnoreCase);
            Control[] controlArray = FormWrapper.EncompassForm.Controls.Find("toolsFormsTabControl", true);
            if (((IEnumerable<Control>)controlArray).Count<Control>() < 1)
                return;
            TabControl tabcontrol = controlArray[0] as TabControl;
            TabPage servicePage = tabcontrol.TabPages.Cast<TabPage>().Where(x => x.Name.Equals("servicesPage")).FirstOrDefault();
            GradientMenuStrip menuStrip = (GradientMenuStrip)FormWrapper.EncompassForm.Controls.Find("mainMenu", true)?[0];

            if (all)
            {
                if (tabcontrol.TabPages.Contains(servicePage))
                    tabcontrol.TabPages.Remove(servicePage);
            }
            else
            {
                Panel panel = tabcontrol.Controls.Find("pnlCategories", true).FirstOrDefault() as Panel;
                if (panel == null)
                    return;


                EpassCategoryControl[] controls = panel.Controls.OfType<EpassCategoryControl>().ToArray();
                foreach (EpassCategoryControl control in controls)
                {
                    string name = control.Title.ToLower().Replace(" ", "");
                    control.Visible = string.IsNullOrEmpty(ServicesToDisable) || !ServicesToDisable.Contains(name);
                }
            }

            ToolStripMenuItem serviceItem = (ToolStripMenuItem)menuStrip.Items["menuItemServices"];
            serviceItem.Visible = string.IsNullOrEmpty(ServicesToDisable);
        }

    }
}