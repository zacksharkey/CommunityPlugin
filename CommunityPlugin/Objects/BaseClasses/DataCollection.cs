using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Enums;
using CommunityPlugin.Objects.Handlers;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommunityPlugin.Objects.BaseClasses
{
    public abstract class DataCollection : MarshalByRefObject, IXmlSerializable, IDisposable
    {
        protected internal const string FieldIDOfNow = "$$now";
        protected internal const string FieldIDOfToday = "$$today";
        protected internal const string FieldIDOfEmpty = "$$empty";
        private const string ClassName = "Utility4.DataExchange.Translation.DataCollection";
        private const char Space = ' ';
        private const string XlementNameOfItemName = "Item";
        private const string XlementNameOfItemFieldID = "FieldID";
        private const string XlementNameOfItemValue = "Value";
        private const string XlementNameOfItemValueType = "ValueType";
        protected IDictionary<string, DataCollection.DataItem> pitems;
        private ITranslator _translator;
        public Func<object, ItemFilterEventArgs, bool> ProcessItemFunc;

        public event TranslationFilterHandler PreProcessExpression;

        public event TranslationFilterHandler ProcessExpression;

        public event TranslationFilterHandler ProcessMacro;

        public event ItemFilterHandler ProcessItem;

        internal bool IsBinded { get; set; }

        public bool AllowSpaceInFieldID { get; set; }

        protected internal bool Silent { get; set; }

        public object this[string fieldID]
        {
            get
            {
                if (!this.pitems.ContainsKey(fieldID))
                    throw new Exception(string.Format("Invalid field id:{0}.", (object)fieldID));
                DataCollection.DataItem pitem = this.pitems[fieldID];
                if (pitem.Value == null || pitem.Value == DBNull.Value)
                    return (object)DBNull.Value;
                switch (pitem.ValueType)
                {
                    case Enums.ValueType.Numeric:
                        return (object)(Decimal)pitem.Value;
                    case Enums.ValueType.String:
                        return pitem.Value.GetType() == typeof(string[]) ? (object)string.Join(",", pitem.Value as string[]) : (object)(string)pitem.Value;
                    case Enums.ValueType.DateTime:
                        return (object)(DateTime)pitem.Value;
                    default:
                        return (object)(string)pitem.Value;
                }
            }
        }

        public virtual object[] Keys
        {
            get
            {
                return ((IEnumerable<string>)this.KeyFieldIDs).Select<string, object>((Func<string, object>)(k => this[k])).ToArray<object>();
            }
        }

        protected internal string[] KeyFieldIDs { get; set; }

        protected virtual ITranslator Translator
        {
            get
            {
                if (this._translator == null)
                {
                    lock (this)
                    {
                        if (this._translator == null)
                        {
                            ICriterionParser parser = (ICriterionParser)this.CreateParser();
                            if (parser.CustomFunctions == null && this.CustomFunctions != null)
                                parser.CustomFunctions = this.CustomFunctions;
                            this._translator = (ITranslator)new Translator(parser);
                        }
                    }
                }
                return this._translator;
            }
        }

        public Func<string, IMapping, string> PreProcessExpressionFunc { get; set; }

        public Func<string, IMapping, string> ProcessExpressionFunc { get; set; }

        public Func<string, IMapping, string> ProcessMacroFunc { get; set; }

        public Func<string, string[], string> CustomFunctions { get; set; }

        public Func<string, Enums.ValueType> GetValueTypeFunc { get; set; }

        protected DataCollection()
        {
            this.AllowSpaceInFieldID = false;
            this.pitems = (IDictionary<string, DataCollection.DataItem>)new Dictionary<string, DataCollection.DataItem>(128);
            this.Silent = false;
        }

        public XmlSchema GetSchema()
        {
            return (XmlSchema)null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            while (reader.Read())
            {
                string name;
                if ((name = reader.Name) != null && !(name == "Item") && (!(name == "FieldID") && !(name == "Value")))
                {
                    int num = name == "ValueType" ? 1 : 0;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            writer.WriteStartElement("Item");
            foreach (string key in (IEnumerable<string>)this.pitems.Keys)
            {
                DataCollection.DataItem pitem = this.pitems[key];
                writer.WriteElementString("FieldID", pitem.FieldID);
                writer.WriteElementString("Value", string.Format("{0}", pitem.Value));
                writer.WriteElementString("ValueType", pitem.ValueType.ToString());
            }
            writer.WriteEndElement();
        }

        public bool ContainsField(string fieldID)
        {
            return !string.IsNullOrWhiteSpace(fieldID) && this.pitems.ContainsKey(fieldID);
        }

        public virtual string GetTextKey()
        {
            object[] keys = this.Keys;
            if (keys == null || keys.Length == 0)
                return Guid.NewGuid().ToString();
            return keys.Length == 1 ? keys[0].ToString() : string.Join(".", ((IEnumerable<object>)keys).Select<object, string>((Func<object, string>)(o => o.ToString())).ToArray<string>());
        }

        protected virtual bool IsSystemField(string fieldID)
        {
            if (string.IsNullOrWhiteSpace(fieldID))
                throw new ArgumentNullException(nameof(fieldID));
            return fieldID.StartsWith("$") || fieldID.StartsWith("@") || fieldID.Contains("$index");
        }

        public string GetString(string fieldID)
        {
            return this.GetString(fieldID, (IMapping)null);
        }

        public string GetString(string fieldID, IMapping mapping)
        {
            if (string.IsNullOrWhiteSpace(fieldID))
                throw new ArgumentNullException(nameof(fieldID));
            DataCollection.DataItem dataItem = this.GetItem(fieldID, mapping);
            if (dataItem == null || dataItem.Value == null)
                return string.Empty;
            switch (dataItem.ValueType)
            {
                case Enums.ValueType.Numeric:
                    return dataItem.Value.GetType() == typeof(string[]) ? string.Join(",", ((IEnumerable<string>)(dataItem.Value as string[])).Select<string, string>((Func<string, string>)(s => string.Format("{0}", (object)s))).ToArray<string>()) : string.Format("{0}", dataItem.Value);
                case Enums.ValueType.String:
                    return dataItem.Value.GetType() == typeof(string[]) ? string.Join(",", ((IEnumerable<string>)(dataItem.Value as string[])).Select<string, string>((Func<string, string>)(s => string.Format("\"{0}\"", (object)s))).ToArray<string>()) : string.Format("\"{0}\"", (object)(string)dataItem.Value);
                case Enums.ValueType.DateTime:
                    return dataItem.Value.GetType() == typeof(string[]) ? string.Join(",", ((IEnumerable<string>)(dataItem.Value as string[])).Select<string, string>((Func<string, string>)(s => string.Format("#{0}#", (object)DateTime.Parse(s)))).ToArray<string>()) : string.Format("#{0}#", (object)(DateTime)dataItem.Value);
                default:
                    return object.Equals(dataItem.Value, (object)DBNull.Value) ? string.Empty : string.Format("{0}", dataItem.Value);
            }
        }

        public DataCollection.DataItem GetItem(string fieldID)
        {
            return this.GetItem(fieldID, (IMapping)null);
        }

        public virtual DataCollection.DataItem GetItem(string fieldID, IMapping mapping)
        {
            if (string.IsNullOrWhiteSpace(fieldID))
                throw new ArgumentNullException(nameof(fieldID));
            bool flag = false;
            ItemFilterEventArgs e = (ItemFilterEventArgs)null;
            if (this.ProcessItemFunc != null)
            {
                e = new ItemFilterEventArgs()
                {
                    Mapping = mapping,
                    FieldID = fieldID
                };
                if (this.ProcessItemFunc((object)this, e))
                    flag = true;
            }
            if (!flag && this.ProcessItem != null)
            {
                e = new ItemFilterEventArgs()
                {
                    Mapping = mapping,
                    FieldID = fieldID
                };
                if (this.ProcessItem((object)this, e))
                    flag = true;
            }
            if (flag)
            {
                DataCollection.DataItem dataItem = new DataCollection.DataItem()
                {
                    FieldID = fieldID,
                    Value = e.Result
                };
                dataItem.ValueType = e.ValueType != Enums.ValueType.Unknown ? e.ValueType : DataCollection.GetValueType(e.Result);
                return dataItem;
            }
            if (!this.pitems.ContainsKey(fieldID))
            {
                fieldID = this.ProcessFieldID(fieldID, mapping);
                if (!this.pitems.ContainsKey(fieldID))
                    throw new Exception(string.Format("Invalid field id: {0}.", (object)fieldID));
            }
            return this.pitems[fieldID];
        }

        protected virtual string ProcessFieldID(string fieldID, IMapping mapping)
        {
            if (!this.AllowSpaceInFieldID && fieldID.IndexOf(' ') >= 0)
                fieldID = fieldID.Replace(" ", string.Empty);
            return fieldID;
        }

        public void SetItem(string fieldID, object fieldValue)
        {
            this.SetItem(fieldID, fieldValue, DataCollection.GetValueType(fieldValue));
        }

        public void SetItem(string fieldID, object fieldValue, Enums.ValueType valueType)
        {
            if (string.IsNullOrWhiteSpace(fieldID))
                throw new ArgumentNullException(nameof(fieldID));
            //if (fieldID.StartsWith("$") && !this.Silent)
            //    GlobalTracer.TraceVerboseFormat("start to set system field, {0}:{1}", (object)fieldID, fieldValue);
            if (this.pitems.ContainsKey(fieldID))
            {
                DataCollection.DataItem pitem = this.pitems[fieldID];
                pitem.Value = fieldValue;
                pitem.ValueType = valueType;
            }
            else
            {
                DataCollection.DataItem dataItem = DataCollection.CreateItem(fieldID, fieldValue, valueType);
                this.pitems.Add(fieldID, dataItem);
            }
        }

        protected virtual CriterionParser CreateParser()
        {
            return (CriterionParser)new DataCriterionParser(this);
        }

        protected static DataCollection.DataItem CreateItem(
          string fieldID,
          object fieldValue)
        {
            return DataCollection.CreateItem(fieldID, fieldValue, DataCollection.GetValueType(fieldValue));
        }

        protected static DataCollection.DataItem CreateItem(
          string fieldID,
          object fieldValue,
          Enums.ValueType valueType)
        {
            return new DataCollection.DataItem()
            {
                FieldID = fieldID,
                Value = fieldValue,
                ValueType = valueType
            };
        }

        protected static Enums.ValueType GetValueType(object fieldValue)
        {
            return fieldValue == null ? Enums.ValueType.Unknown : DataCollection.GetValueType(Type.GetTypeFromHandle(Type.GetTypeHandle(fieldValue)));
        }

        protected static Enums.ValueType GetValueType(Type type)
        {
            if (type == (Type)null)
                throw new ArgumentNullException(nameof(type));
            if (type == typeof(string) || type == typeof(char))
                return Enums.ValueType.String;
            if (type == typeof(DateTime))
                return Enums.ValueType.DateTime;
            if (type == typeof(byte[]))
                return Enums.ValueType.ByteArray;
            return type == typeof(Decimal) || type == typeof(float) || (type == typeof(byte) || type == typeof(short)) || (type == typeof(ushort) || type == typeof(int) || (type == typeof(uint) || type == typeof(long))) || type == typeof(ulong) ? Enums.ValueType.Numeric : Enums.ValueType.Unknown;
        }

        protected virtual DataCollection.DataItem CreateDataItem(string fieldID)
        {
            if (string.IsNullOrWhiteSpace(fieldID))
                throw new ArgumentNullException(nameof(fieldID));
            try
            {
                object dataItemValue = this.GetDataItemValue(fieldID);
                Enums.ValueType valueType = this.GetValueTypeFunc == null ? DataCollection.GetValueType(dataItemValue) : this.GetValueTypeFunc(fieldID);
                return DataCollection.CreateItem(fieldID, dataItemValue, valueType);
            }
            catch (Exception ex)
            {
                //if (!this.Silent)
                //    GlobalTracer.TraceWarningFormat("Create data item failed, field id:{0}, details:{1}", (object)fieldID, (object)ex.Message);
                return (DataCollection.DataItem)null;
            }
        }

        protected virtual object GetDataItemValue(string fieldID)
        {
            throw new NotImplementedException("The method can't be implemented.");
        }

        public virtual void Fill(MappingDictionary mDict)
        {
            if (mDict == null)
                throw new ArgumentNullException(nameof(mDict));
            this.Fill(mDict.GetNeededFields());
        }

        public virtual void Fill(IList<string> fieldIDs)
        {
            if (fieldIDs == null)
                throw new ArgumentNullException(nameof(fieldIDs));
            if (!this.CheckFieldIDs(fieldIDs))
                throw new ArgumentException("Some field identities are invalid.");
            if (this.pitems.Count == 0)
                this.InitDefinition();
            IList<string> stringList = (IList<string>)new List<string>(fieldIDs.Count);
            foreach (string fieldId in (IEnumerable<string>)fieldIDs)
            {
                if (!this.pitems.ContainsKey(fieldId) && !stringList.Contains(fieldId))
                {
                    if (!this.IsSystemField(fieldId))
                        stringList.Add(fieldId);
                }
            }
            foreach (string index in (IEnumerable<string>)stringList)
            {
                DataCollection.DataItem dataItem = this.CreateDataItem(index);
                if (dataItem != null)
                {
                    if (this.pitems.ContainsKey(index))
                        this.pitems[index] = dataItem;
                    else
                        this.pitems.Add(index, dataItem);
                }
            }
            GC.Collect();
        }

        protected virtual bool CheckFieldIDs(IList<string> fieldIDs)
        {
            return true;
        }

        protected virtual void InitDefinition()
        {
            this.SetItem("$$now", (object)DateTime.Now, Enums.ValueType.DateTime);
            this.SetItem("$$today", (object)DateTime.Today, Enums.ValueType.DateTime);
            this.SetItem("$$empty", (object)string.Empty, Enums.ValueType.DateTime);
        }

        public string GetResultByMapping(IMapping mapping)
        {
            return this.GetResultByMapping(mapping, false);
        }

        public virtual string GetResultByMapping(IMapping mapping, bool throwEx)
        {
            string result = string.Empty;
            try
            {
                switch (mapping.TranslationType)
                {
                    case TranslationType.Directly:
                        result = this.GetDirectlyValue(mapping);
                        break;
                    case TranslationType.Static:
                        result = this.GetStaticValue(mapping);
                        break;
                    case TranslationType.Blank:
                        result = this.GetBlankValue(mapping);
                        break;
                    case TranslationType.Expression:
                        result = this.GetExpressionValue(mapping);
                        break;
                    case TranslationType.Macro:
                        result = this.GetMacroValue(mapping);
                        break;
                }
                string str = this.ProcessResult(result, mapping);
                //if (!this.Silent)
                //    GlobalTracer.TraceVerbose(string.Format("Get value:{0}\t for translation:{1}", (object)str, (object)mapping.Translation), string.Format("{0}.GetResultByMapping", (object)"Utility4.DataExchange.Translation.DataCollection"));
                return str;
            }
            catch (Exception ex)
            {
                if (throwEx)
                    throw ex;
                //if (!this.Silent)
                //    GlobalTracer.TraceWarning(string.Format("Failed to get result by translation:{0}, details:{1}", (object)mapping.Translation, (object)ex.Message), string.Format("{0}.GetResultByMapping", (object)"Utility4.DataExchange.Translation.DataCollection"));
                return string.Empty;
            }
        }

        public bool ExecuteTranslation(string translation, out string result, out string message)
        {
            if (string.IsNullOrWhiteSpace(translation))
                throw new ArgumentNullException(nameof(translation));
            result = string.Empty;
            message = string.Empty;
            IMapping byTranslation = Mapping.CreateByTranslation(translation);
            try
            {
                this.Fill(Models.Translation.Translator.GetNeededFieldsByTranslation(translation));
                result = this.GetResultByMapping(byTranslation, true);
            }
            catch (Exception ex)
            {
                result = string.Empty;
                message = ex.Message;
                return false;
            }
            return true;
        }

        protected virtual string GetStaticValue(IMapping mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));
            return mapping.Translation;
        }

        protected virtual string GetDirectlyValue(IMapping mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));
            string fieldID = this.GetPreProcessedTranslation(mapping.Translation, mapping);
            if (fieldID.StartsWith("[") && fieldID.EndsWith("]"))
                fieldID = fieldID.Substring(1, fieldID.Length - 2);
            DataCollection.DataItem dataItem = this.GetItem(fieldID, mapping);
            if (dataItem == null || dataItem.Value == null)
                return string.Empty;
            return (fieldID.ToUpper() == "LOAN.LASTMODIFIED" || fieldID.ToUpper() == "LOANLASTMODIFIED") && GlobalConfiguration.UseMilliSecondsForLastModified ? string.Format("{0}", (object)((DateTime)dataItem.Value).ToString("MM/dd/yyyy hh:mm:ss.fff tt")) : string.Format("{0}", dataItem.Value);
        }

        protected virtual string GetExpressionValue(IMapping mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));
            if (string.IsNullOrWhiteSpace(mapping.Translation))
                return string.Empty;
            string translation = this.GetPreProcessedTranslation(mapping.Translation, mapping);
            bool flag = false;
            TranslationFilterEventArgs e = new TranslationFilterEventArgs()
            {
                Transalation = translation,
                Mapping = mapping
            };
            if (this.ProcessExpression != null)
                flag = this.ProcessExpression((object)this, e);
            if (flag)
                translation = e.FilteredTranslation;
            if (this.ProcessExpressionFunc != null)
                translation = this.ProcessExpressionFunc(translation, mapping);
            return this.Translator.GetTranslatioinValue(translation, mapping);
        }

        protected virtual string GetMacroValue(IMapping mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));
            if (string.IsNullOrWhiteSpace(mapping.Translation))
                return string.Empty;
            string processedTranslation = this.GetPreProcessedTranslation(mapping.Translation, mapping);
            bool flag = false;
            TranslationFilterEventArgs e = new TranslationFilterEventArgs()
            {
                Transalation = processedTranslation,
                Mapping = mapping
            };
            if (this.ProcessMacro != null)
            {
                foreach (Delegate invocation in this.ProcessMacro.GetInvocationList())
                {
                    if (invocation is TranslationFilterHandler)
                    {
                        flag = (invocation as TranslationFilterHandler)((object)this, e);
                        if (flag)
                            break;
                    }
                }
            }
            if (flag)
                return e.FilteredTranslation;
            if (this.ProcessMacroFunc != null)
                return this.ProcessMacroFunc(processedTranslation, mapping);
            throw new Exception(string.Format("The macro:{0} can't be defined.", (object)mapping.Translation));
        }

        protected virtual string GetBlankValue(IMapping mapping)
        {
            return string.Empty;
        }

        protected virtual string GetPreProcessedTranslation(string translation, IMapping mapping)
        {
            if (string.IsNullOrWhiteSpace(translation))
                return string.Empty;
            bool flag = false;
            TranslationFilterEventArgs e = new TranslationFilterEventArgs()
            {
                Transalation = translation,
                Mapping = mapping
            };
            if (this.PreProcessExpression != null)
            {
                foreach (Delegate invocation in this.PreProcessExpression.GetInvocationList())
                {
                    if (invocation is TranslationFilterHandler)
                    {
                        flag = (invocation as TranslationFilterHandler)((object)this, e);
                        if (flag)
                            break;
                    }
                }
            }
            if (flag)
                translation = e.FilteredTranslation;
            if (this.PreProcessExpressionFunc != null)
                translation = this.PreProcessExpressionFunc(translation, mapping);
            return translation;
        }

        protected internal virtual string ProcessResult(string result, IMapping mapping)
        {
            if (string.IsNullOrWhiteSpace(result))
                return string.Empty;
            if (result.StartsWith("\"") && result.EndsWith("\"") && result.Length >= 2)
                result = result.Substring(1, result.Length - 2);
            if (result.StartsWith("#") && result.EndsWith("#") && result.Length >= 2)
                result = result.Substring(1, result.Length - 2);
            if (mapping is ISupportFormat && (mapping as ISupportFormat).HasFormat)
                result = ValueFormatter.FormatValue(result, (mapping as ISupportFormat).Format);
            result = this.ResizeResult(result, mapping);
            return result;
        }

        protected virtual string ResizeResult(string result, IMapping mapping)
        {
            string[] strArray = new string[2] { "Size", "Length" };
            int result1 = -1;
            foreach (string key in strArray)
            {
                if (mapping.Properties.ContainsKey(key) && int.TryParse(mapping.Properties[key], out result1) && result1 >= 0)
                    break;
            }
            if (result1 <= 0)
                return result;
            return result.Length > result1 ? result.Substring(0, result1) : result.PadRight(result1, ' ');
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            int num = disposing ? 1 : 0;
            if (this.pitems != null)
            {
                this.pitems.Clear();
                this.pitems = (IDictionary<string, DataCollection.DataItem>)null;
            }
            GC.SuppressFinalize((object)this);
        }

        public class DataItem
        {
            public string FieldID { get; internal set; }

            public object Value { get; internal set; }

            public Enums.ValueType ValueType { get; internal set; }
        }
    }
}
