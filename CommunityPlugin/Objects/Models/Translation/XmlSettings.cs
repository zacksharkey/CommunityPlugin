using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class XmlSettings
    {
        private XElement _parent;

        public XmlSettings(XElement parent)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            this._parent = parent;
        }

        public XmlSettingSection GetSection(string xpath)
        {
            IList<XmlSettingSection> sections = this.GetSections(xpath);
            return sections.Count <= 0 ? (XmlSettingSection)null : sections[0];
        }

        public IList<XmlSettingSection> GetSections(string xpath)
        {
            if (string.IsNullOrWhiteSpace(xpath))
                throw new ArgumentNullException(nameof(xpath));
            IList<XmlSettingSection> xmlSettingSectionList = (IList<XmlSettingSection>)new List<XmlSettingSection>(16);
            foreach (XElement xpathSelectElement in this._parent.XPathSelectElements(xpath))
                xmlSettingSectionList.Add(new XmlSettingSection(xpathSelectElement));
            return xmlSettingSectionList;
        }
    }
}
