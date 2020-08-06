using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Extension;
using CommunityPlugin.Objects.Models;
using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.FormEditor;
using EllieMae.EMLite.InputEngine.Forms;
using EllieMae.EMLite.RemotingServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu.AnalysisTools
{
    public class SearchInputForms2 : AnalysisBase
    {
        private List<InputFormResult> Result = new List<InputFormResult>();
        public override AnalysisResult ExecuteTest() { return null; }

        public override bool IsTest() { return false; }

        public override void LoadCache()
        {
            foreach (InputFormInfo info in Session.FormManager.GetFormInfos(InputFormType.Custom))
            {
                string html = string.Empty;

                try
                {
                    string o = FormStore.GetFormHTMLPath(Session.DefaultInstance, info.FormID);
                    if (!o.Empty())
                    {
                        FileInfo fi1 = new FileInfo(o);
                        using (StreamReader sr = fi1.OpenText())
                        {
                            string s = "";
                            while ((s = sr.ReadLine()) != null)
                            {
                                html += s;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.HandleError(ex, nameof(SearchInputForms2));
                }


                Result.Add(new InputFormResult(info.FormID, info.Name, html ?? string.Empty));
            }
        }

        public override AnalysisResult SearchResults(string Search)
        {
            return new AnalysisResult(nameof(SearchInputForms2)) {Result= Result };
        }
    }
}