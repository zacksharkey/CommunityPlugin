using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.RemotingServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommunityPlugin.Objects.Models
{
    public class InputForm
    {
        private InputFormInfo _formInfo;
        private string _formData;

        public string FormName { get; set; }
        public List<InputFormControl> FormControls { get; set; }
        public List<InputFormEvent> FormEvents { get; set; }

        public InputForm(SessionObjects sos, InputFormInfo formInfo)
        {
            _formInfo = formInfo;

            FormControls = new List<InputFormControl>();
            FormEvents = new List<InputFormEvent>();
            this.FormName = _formInfo.Name;
            LoadData(sos);
        }

        public System.Windows.Forms.HtmlDocument GetHtmlDocument(string html)
        {
            WebBrowser browser = new WebBrowser();
            browser.ScriptErrorsSuppressed = true;
            browser.DocumentText = html;
            browser.Document.OpenNew(true);
            browser.Document.Write(html);
            browser.Refresh();
            return browser.Document;
        }

        private void LoadData(SessionObjects sos)
        {
            _formData = ExtractForm(sos.FormManager.GetCustomForm(_formInfo.FormID));
            if (string.IsNullOrEmpty(_formData)) return;

            try
            {
                var htmlDoc = GetHtmlDocument(_formData);

                List<HtmlElement> elements = new List<HtmlElement>();
                foreach (HtmlElement item in htmlDoc.Body.Children)
                {
                    elements.Add(item);
                    if (item.CanHaveChildren)
                    {
                        var collectionElemetns = GetHtmlElements(item.Children);
                        elements.AddRange(collectionElemetns);
                    }
                }

                foreach (var item in elements)
                {
                    string encompassFieldID = item.GetAttribute("emid");
                    if (string.IsNullOrEmpty(encompassFieldID) == false)
                    {
                        string controlID = item.GetAttribute("fieldId");
                        string controlType = item.GetAttribute("type");

                        FormControls.Add(
                            new InputFormControl()
                            {
                                EncompassFormName = this.FormName,
                                LoanFieldID = encompassFieldID,
                                ObjectControlID = controlID,
                                ObjectControlType = controlType
                            }); ;
                    }


                    string eventType = item.GetAttribute("event");
                    if (string.IsNullOrEmpty(eventType) == false)
                    {
                        string eventLocation = item.GetAttribute("for");
                        string code = item.InnerHtml;
                        if (string.IsNullOrEmpty(code) == false)
                        {
                            FormEvents.Add(
                                new InputFormEvent()
                                {
                                    EncompassFormName = this.FormName,
                                    EventType = eventType,
                                    EventLocationId = eventLocation,
                                    CustomCode = code
                                });
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
  
        }

        private static List<HtmlElement> GetHtmlElements(HtmlElementCollection collection)
        {
            List<HtmlElement> response = new List<HtmlElement>();

            foreach (HtmlElement item in collection)
            {
                response.Add(item);
                if (item.CanHaveChildren)
                {
                    response.AddRange(GetHtmlElements(item.Children));
                }
            }
            return response;
        }

        private string ExtractForm(EllieMae.EMLite.RemotingServices.BinaryObject form)
        {
            if (form == null) return null;

            string guid = Guid.NewGuid() + "";
            string dir = Path.Combine(SystemSettings.TempFolderRoot, "NgPrep\\" + guid);

            try
            {
                Directory.CreateDirectory(dir);
                string filePath = dir + @"\form.emfrm";
                form.Write(filePath);
                FileCompressor.Instance.Unzip(filePath, dir);

                filePath = dir + @"\FORM.htm";
                if (!File.Exists(filePath)) return null;
                string formData = File.ReadAllText(filePath);

                try { Directory.Delete(dir, true); }
                catch (Exception) { }

                form.Dispose();
                return formData;
            }
            catch (Exception)
            { }
            return null;
        }
    }

}
