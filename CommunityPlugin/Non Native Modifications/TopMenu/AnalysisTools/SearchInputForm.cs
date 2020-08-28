using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Models;
using EllieMae.EMLite.ClientServer;
using System.Collections.Generic;
using System.Linq;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu.AnalysisTools
{
    public class SearchInputForm : AnalysisBase
    {
        public override AnalysisResult ExecuteTest() { return null; }

        public override bool IsTest() { return false; }

        public override void LoadCache()
        {
            List<InputForm> inputForms = new List<InputForm>();
            InputFormInfo[] forms = EncompassHelper.SessionObjects.FormManager.GetAllFormInfos();
            foreach (InputFormInfo form in forms)
            {
                InputForm encForm = new InputForm(EncompassHelper.SessionObjects, form);
                inputForms.Add(encForm);

            }
            Cache =  inputForms;
        }

        public override AnalysisResult SearchResults(string Search)
        {
            Search = Search.ToUpper();
            List<InputForm> inputForms = (List<InputForm>)Cache;

            List<InputFormResult> results = new List<InputFormResult>();
            foreach (var inputform in inputForms)
            {
                List<InputFormResult> controls = inputform.FormControls.Where(x => x.LoanFieldID.ToUpper().Contains(Search))
                    .Select(y => new InputFormResult() { InputFormName = inputform.FormName, Location = y.ObjectControlType, LocationDetails = y.ObjectControlID })
                    .ToList();

                results.AddRange(controls);

                if (inputform.FormEvents != null)
                {
                    List<InputFormResult> customCode = inputform.FormEvents.Where(x => x.CustomCode != null).Where(x => x.CustomCode.ToUpper().Contains(Search))
                        .Select(x => new InputFormResult() { InputFormName = inputform.FormName, Location = $"{x.EventType} - {x.EventLocationId}", LocationDetails = x.CustomCode })
                        .ToList();

                    results.AddRange(customCode);

                }

            }

            return new AnalysisResult(nameof(SearchInputForm)) { Result = results };

        }
    }
}
