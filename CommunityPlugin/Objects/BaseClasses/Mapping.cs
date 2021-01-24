using CommunityPlugin.Objects.Enums;
using CommunityPlugin.Objects.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityPlugin.Objects.BaseClasses
{
    public class Mapping : IMapping
    {
        public  const string PropertyNameForFieldName = "Field_Name";
        public const string PropertyNameForHeaderName = "Header_Name";
        public const string PropertyNameForEncompassFieldID = "Encompass_Field_ID";
        public const string PropertyNameForTranslation = "Translation_Or_Calculation";
        public const string PropertyNameForDescription = "Description";
        public const string PropertyNameForValueType = "ValueType";
        public const string PropertyNameForValueType2 = "DataType";
        public const string BlankValue = "$blank";
        public const string BlankOldValue = "blank"; 

        public string ColumnName { get; set; }

        public string Description { get; set; }

        public string Translation { get; set; }

        public TranslationType TranslationType { get; set; }

        public Enums.ValueType ValueType { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        public Mapping()
        {
            this.ValueType = Enums.ValueType.Unknown;
            this.Properties = (IDictionary<string, string>)new Dictionary<string, string>(8);
        }

        public Mapping(DataRow row)
        {
            this.ValueType = Enums.ValueType.Unknown;
            this.Properties = (IDictionary<string, string>)new Dictionary<string, string>(8);
            this.InitRow(row);
        }

        public string GetProperty(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            string index = this.Properties.Keys.Where<string>((Func<string, bool>)(s => s.Equals(name, StringComparison.CurrentCultureIgnoreCase))).FirstOrDefault<string>();
            return !string.IsNullOrWhiteSpace(index) ? this.Properties[index] : string.Empty;
        }

        public virtual void InitRow(DataRow row)
        {
            foreach (DataColumn column in (InternalDataCollectionBase)row.Table.Columns)
                this.InitProperties(row, column);
            this.InitTranslation();
            if (string.IsNullOrEmpty(this.Translation))
            {
                this.Translation = "$blank";
                this.TranslationType = TranslationType.Expression;
            }
            else
                this.TranslationType = this.GetTranslationType(this.Translation);
        }

        public virtual void InitProperties(DataRow row, DataColumn column)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            if (column == null)
                throw new ArgumentNullException(nameof(column));
            string str = string.Format("{0}", row[column]);
            switch (column.ColumnName)
            {
                case "Field_Name":
                    this.ColumnName = str;
                    break;
                case "Description":
                    this.Description = str;
                    break;
                case "Translation_Or_Calculation":
                    this.Translation = str;
                    break;
                case "ValueType":
                case "DataType":
                    Enums.ValueType result;
                    if (Enum.TryParse<Enums.ValueType>(str, out result))
                    {
                        this.ValueType = result;
                        break;
                    }
                    switch (str.ToLower())
                    {
                        case "number":
                        case "numeric":
                        case "decimal":
                        case "float":
                        case "single":
                        case "real":
                        case "money":
                        case "currency":
                            this.ValueType = Enums.ValueType.Numeric;
                            return;
                        case "datetime":
                        case "date":
                        case "time":
                            this.ValueType = Enums.ValueType.DateTime;
                            return;
                        case "string":
                            this.ValueType = Enums.ValueType.String;
                            return;
                        default:
                            this.ValueType = Enums.ValueType.Unknown;
                            return;
                    }
                default:
                    this.Properties.Add(column.ColumnName, str);
                    break;
            }
        }

        public virtual void InitTranslation()
        {
        }

        public TranslationType GetTranslationType(string translation)
        {
            if (string.IsNullOrWhiteSpace(translation) || string.Compare(translation, "$blank", true) == 0 || string.Compare(translation, "blank", true) == 0)
                return TranslationType.Blank;
            if (translation.StartsWith("@"))
                return TranslationType.Macro;
            if (translation.StartsWith("[") && translation.EndsWith("]"))
            {
                translation = translation.Substring(1, translation.Length - 2);
                return translation.IndexOfAny(new char[2]
                {
          '[',
          ']'
                }) >= 0 ? TranslationType.Expression : TranslationType.Directly;
            }
            if (!translation.StartsWith("\"") || !translation.EndsWith("\"") || translation.Length <= 2)
                return TranslationType.Expression;
            translation = translation.Substring(1, translation.Length - 2);
            char[] charArray = translation.ToCharArray();
            for (int index = 0; index < charArray.Length; ++index)
            {
                if (charArray[index] == '"' && index > 0 && charArray[index - 1] != '\\')
                    return TranslationType.Expression;
            }
            return TranslationType.Static;
        }

        internal void Clone(IMapping mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));
            this.ColumnName = mapping.ColumnName;
            this.Description = mapping.Description;
            this.Translation = mapping.Translation;
            this.TranslationType = mapping.TranslationType;
            this.ValueType = mapping.ValueType;
            this.Properties.Clear();
            foreach (string key in (IEnumerable<string>)mapping.Properties.Keys)
                this.Properties.Add(key, mapping.Properties[key]);
        }

        public static IMapping Create(DataRow row)
        {
            return (IMapping)new Mapping(row);
        }

        public static IMapping CreateByTranslation(string translation)
        {
            Mapping mapping = new Mapping()
            {
                ColumnName = "Testing Column",
                Description = "The mapping item only used for testing",
                Translation = translation
            };
            mapping.TranslationType = mapping.GetTranslationType(translation);
            return (IMapping)mapping;
        }
    }
}
