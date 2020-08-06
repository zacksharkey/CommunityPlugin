using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Models;
using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.InputEngine.Forms;
using EllieMae.EMLite.RemotingServices;
using EllieMae.Encompass.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu.AnalysisTools
{
    public class SearchInputForms : AnalysisBase
    {
        public override AnalysisResult ExecuteTest() { return null; }

        public override bool IsTest() { return false; }

        public override AnalysisResult SearchResults(string Search)
        {
            Search = Search.ToUpper();

            //var inputForms = (List<InputForm>)ObjectToSearch;

            List<InputForm> inputForms = new List<InputForm>();
            InputFormInfo[] forms = EncompassHelper.SessionObjects.FormManager.GetAllFormInfos();
            foreach (InputFormInfo form in forms)
            {
                InputForm encForm = new InputForm(EncompassHelper.SessionObjects, form);
                inputForms.Add(encForm);

            }

            List<InputFormResult> results = new List<InputFormResult>();

            foreach (var inputform in inputForms)
            {
                List<InputFormResult> controls = inputform.FormControls.Where(x => x.LoanFieldID.ToUpper().Contains(Search))
                    .Select(y => new InputFormResult() { InputFormName = inputform.FormName, Location = y.ObjectControlType, LocationDetails = y.ObjectControlID })
                    .ToList();

                results.AddRange(controls);

                if (inputform.FormEvents != null)
                {
                    //string[] lines = inputform.CustomCode.Split(
                    //   new[] { "\r\n", "\r", "\n" },
                    //   StringSplitOptions.None);


                    List<InputFormResult> customCode = inputform.FormEvents.Where(x => x.CustomCode != null).Where(x => x.CustomCode.ToUpper().Contains(Search))
                        .Select(x => new InputFormResult() { InputFormName = inputform.FormName, Location = $"{x.EventType} - {x.EventLocationId}", LocationDetails = x.CustomCode })
                        .ToList();

                    results.AddRange(customCode);

                }

            }

            return new AnalysisResult(nameof(SearchInputForms)) { Result = results };

        }

        //public override object GetObjects()
        //{
        //    List<InputForm> inputForms = new List<InputForm>();
        //    InputFormInfo[] forms = EncompassHelper.SessionObjects.FormManager.GetAllFormInfos();
        //    foreach (InputFormInfo form in forms)
        //    {
        //        InputForm encForm = new InputForm(EncompassHelper.SessionObjects, form);
        //        inputForms.Add(encForm);

        //    }
        //    return inputForms;
        //}
    }
}
