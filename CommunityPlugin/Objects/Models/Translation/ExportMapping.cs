using CommunityPlugin.Objects.BaseClasses;
using CommunityPlugin.Objects.Interface;
using System;
using System.Data;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class ExportMapping : Mapping, ISupportFormat, IXmlMapping, IMapping
    {
        protected internal const string PropertyNameForFormat = "Format";
        protected internal const string PropertyNameForFormat2 = "Formatting";

        public bool HasFormat
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Format);
            }
        }

        public string Format { get; private set; }

        public string XPath { get; set; }

        internal ExportMapping(DataRow row)
          : base(row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
        }

        public override void InitRow(DataRow row)
        {
            base.InitRow(row);
            this.Format = !this.Properties.ContainsKey("Format") ? (!this.Properties.ContainsKey("Formatting") ? string.Empty : this.Properties["Formatting"]) : this.Properties["Format"];
            this.XPath = XmlMapping.GetXPathByMapping((IMapping)this);
        }

        public override void InitTranslation()
        {
            string strA = string.Empty;
            if (this.Properties.ContainsKey("Encompass_Field_ID"))
                strA = string.Format("{0}", (object)this.Properties["Encompass_Field_ID"]).Trim();
            if (!string.IsNullOrWhiteSpace(this.Translation) || string.IsNullOrWhiteSpace(strA))
                return;
            if (string.Compare(strA, "$blank", true) == 0 || string.Compare(strA, "blank", true) == 0)
                this.Translation = "$blank";
            else if (strA.StartsWith("\"") && strA.EndsWith("\"") && strA.Length > 2)
            {
                this.Translation = strA;
            }
            else
            {
                if (strA.StartsWith("[") && strA.EndsWith("]"))
                    strA = strA.Substring(1, strA.Length - 1);
                if (string.IsNullOrWhiteSpace(strA))
                    return;
                this.Translation = string.Format("[{0}]", (object)strA);
            }
        }

        public new static IMapping Create(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            return (IMapping)new ExportMapping(row);
        }
    }
}
