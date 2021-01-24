using CommunityPlugin.Objects.BaseClasses;
using CommunityPlugin.Objects.Interface;
using System;
using System.Data;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class ImportMapping : Mapping, IXmlMapping, IMapping
    {
        protected internal const string PropertyNameForTargetFieldID = "Target_Field_ID";

        public string TargetFieldID { get; private set; }

        public string XPath { get; set; }

        internal ImportMapping(DataRow row)
          : base(row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
        }

        public override void InitRow(DataRow row)
        {
            base.InitRow(row);
            this.XPath = XmlMapping.GetXPathByMapping((IMapping)this);
        }

        public override void InitProperties(DataRow row, DataColumn column)
        {
            switch (column.ColumnName)
            {
                case "Target_Field_ID":
                case "Encompass_Field_ID":
                    this.TargetFieldID = string.Format("{0}", row[column]);
                    break;
            }
            base.InitProperties(row, column);
        }

        public override void InitTranslation()
        {
            if (!string.IsNullOrWhiteSpace(this.Translation) || string.IsNullOrWhiteSpace(this.ColumnName))
                return;
            this.Translation = string.Format("[{0}]", (object)this.ColumnName);
        }

        public new static IMapping Create(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            return (IMapping)new ImportMapping(row);
        }
    }
}
