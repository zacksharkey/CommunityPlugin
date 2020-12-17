using CommunityPlugin.Non_Native_Modifications;
using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.Common.UI;
using EllieMae.EMLite.InputEngine;
using EllieMae.EMLite.RemotingServices;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Standard_Plugins
{
    public class OpenInputForm : Plugin, ILoanOpened, IFieldChange
    {
        private bool Open = false;
        private bool HasFields = false;

        public override void LoanOpened(object sender, EventArgs e)
        {
            HasFields = EncompassApplication.Session.Loans.FieldDescriptors.CustomFields.Cast<FieldDescriptor>().Any(x => x.FieldID.Equals("CX.OPENFORM"))
                     && EncompassApplication.Session.Loans.FieldDescriptors.CustomFields.Cast<FieldDescriptor>().Any(x => x.FieldID.Equals("CX.OPENFORM.SIZE"));
        }

        public override void FieldChanged(object sender, FieldChangeEventArgs e)
        {
            if (!HasFields)
                return;

            if (e.FieldID.Equals("CX.OPENFORM") && !string.IsNullOrEmpty(e.NewValue))
            {
                InputFormInfo form = Session.FormManager.GetFormInfoByName(e.NewValue);
                if (form == null)
                    return;

                string size = EncompassHelper.Val("CX.OPENFORM.SIZE").ToString();
                string[] setSize = size.Contains(',') ? size.Split(',') : new string[0];
                Size controlSize = setSize.Count() > 0 ? new System.Drawing.Size(Convert.ToInt32(setSize[0]), Convert.ToInt32(setSize[1])) : new System.Drawing.Size(600, 600);
                QuickEntryPopupDialog2 q = new QuickEntryPopupDialog2(Session.LoanData, $"pop{form.Name}", form, controlSize.Width, controlSize.Height, EllieMae.Encompass.Forms.FieldSource.CurrentLoan, "", Session.DefaultInstance);
                q.Show();
                Open = true;

                EncompassHelper.SetBlank("CX.OPENFORM");
            }
            else if (Open)
            {
                foreach (Form f in FormWrapper.OpenForms.Where(x => x.Name.StartsWith("pop")).Select(x => x))
                {
                    LoanScreen screen = f?.Controls[0] as LoanScreen;
                    if (screen == null)
                        return;

                    if (screen != null && !screen.ContainsFocus)
                        screen.RefreshLoanContents();
                    else
                        Session.Application.GetService<ILoanEditor>()?.RefreshContents();
                }
            }
        }

        public override void LoanClosing(object sender, EventArgs e)
        {
            List<Form> close = FormWrapper.OpenForms.Where(x => x.Name.StartsWith("pop")).ToList();
            foreach (Form f in close)
                f.Close();
        }
    }
}
