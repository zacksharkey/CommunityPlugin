using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans.Templates;
using System;

namespace CommunityPlugin.Standard_Plugins
{
    public class AutomateInputFormSet : Plugin, ILoanOpened
    {
        public override void LoanOpened(object sender, EventArgs e)
        {
            InputFormSet formSet = GetInputFormByPersona();
            if (formSet != null)
                EncompassHelper.Loan.ApplyTemplate(formSet, false);

        }

        private InputFormSet GetInputFormByPersona()
        {
            InputFormSet id = GetTemplate($"Public:\\Persona\\User_{EncompassHelper.User.ID}");
            InputFormSet loanType = GetTemplate($"Public:\\Persona\\LoanType_{EncompassHelper.Loan.Fields["1172"].FormattedValue}_{EncompassHelper.LastPersona}");
            InputFormSet milestone = GetTemplate($"Public:\\Persona\\Milestone_{EncompassHelper.Loan.Log.MilestoneEvents.LastCompletedEvent.MilestoneName}_{EncompassHelper.LastPersona}");
            InputFormSet persona = GetTemplate($"Public:\\Persona\\Persona_{EncompassHelper.LastPersona}");
            InputFormSet defaultSet = GetTemplate($"Public:\\Persona\\Default");

            return id ?? loanType ?? milestone ?? persona ?? defaultSet;
        }

        private InputFormSet GetTemplate(string path)
        {
            return (InputFormSet)EncompassApplication.Session.Loans.Templates.GetTemplate(TemplateType.InputFormSet, path);
        }

    }
}
