using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class XmlSettingSection
    {
        public string this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));
                key = key.ToLower();
                return !this.Settings.ContainsKey(key) ? string.Empty : this.Settings[key];
            }
        }

        public string Text { get; private set; }

        protected XElement Element { get; private set; }

        protected IDictionary<string, string> Settings { get; private set; }

        internal XmlSettingSection(XElement xe)
        {
            if (xe == null)
                throw new ArgumentNullException(nameof(xe));
            this.Element = xe;
            this.Settings = (IDictionary<string, string>)new Dictionary<string, string>();
            this.init(xe);
        }

        private void init(XElement xe)
        {
            if (xe == null)
                throw new ArgumentNullException(nameof(xe));
            foreach (XNode node in xe.Nodes())
            {
                if (node is XElement)
                {
                    string str1 = (node as XElement).Name.ToString();
                    string str2 = (node as XElement).Value;
                    if (!string.IsNullOrWhiteSpace(str1))
                    {
                        string lower = str1.ToLower();
                        if (!this.Settings.ContainsKey(lower))
                            this.Settings.Add(lower, str2);
                    }
                }
            }
            foreach (XAttribute attribute in xe.Attributes())
            {
                string str1 = attribute.Name.ToString();
                string str2 = attribute.Value;
                if (!string.IsNullOrWhiteSpace(str1))
                {
                    string lower = str1.ToLower();
                    if (!this.Settings.ContainsKey(lower))
                        this.Settings.Add(lower, str2);
                }
            }
            this.Text = xe.Value;
        }

        public XmlSettingSection GetSection(string xpath)
        {
            if (string.IsNullOrWhiteSpace(xpath))
                throw new ArgumentNullException(nameof(xpath));
            XElement xe = this.Element.XPathSelectElement(xpath);
            return xe == null ? (XmlSettingSection)null : new XmlSettingSection(xe);
        }

        public IList<XmlSettingSection> GetSections(string xpath)
        {
            if (string.IsNullOrWhiteSpace(xpath))
                throw new ArgumentNullException(nameof(xpath));
            IList<XmlSettingSection> xmlSettingSectionList = (IList<XmlSettingSection>)new List<XmlSettingSection>();
            foreach (XElement xpathSelectElement in this.Element.XPathSelectElements(xpath))
                xmlSettingSectionList.Add(new XmlSettingSection(xpathSelectElement));
            return xmlSettingSectionList;
        }
    }
}
