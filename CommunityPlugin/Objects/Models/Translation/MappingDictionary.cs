
using CommunityPlugin.Objects.BaseClasses;
using CommunityPlugin.Objects.Enums;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using EllieMae.Encompass.BusinessObjects;
using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class MappingDictionary : ISettings
    {
        private static readonly string[] __systemTable = new string[3]
        {
      nameof (Mappings),
      "Header",
      "Setting"
        };
        public const string TableOfMapping = "Mapping";
        public const string TableOfHeaderMapping = "HeaderMapping";
        public const string TableOfHeader = "Header";
        public const string TableOfSetting = "Setting";
        public const string TableOfMappings = "Mappings";
        private const string MetaColumnOfMappingsID = "Mappings_Id";
        private const string MetaColumnOfVersion = "version";
        private const string MetaColumnOfMappingType = "type";
        private const string ColumnNameOfTableName = "TableName";
        private const string PrefixOfTableDefine = "<<";
        private const string SuffixOfTableDefine = ">>";
        private const string MappingTypeOfExport = "export";
        private const string MappingTypeOfImport = "import";
        private IList<string> _needFields;

        public bool IsEmpty { get; private set; }

        public IDictionary<string, string> Metas { get; private set; }

        public MappingVersion Version { get; private set; }

        public IDictionary<string, string> Settings { get; private set; }

        public IDictionary<string, IMapping> Mappings
        {
            get
            {
                return ((IEnumerable<string>)this.MappingNames).Contains<string>("Mapping") ? this.GetMappings("Mapping") : (IDictionary<string, IMapping>)new Dictionary<string, IMapping>();
            }
        }

        public IDictionary<string, IMapping> HeaderMappings
        {
            get
            {
                return ((IEnumerable<string>)this.MappingNames).Contains<string>("HeaderMapping") ? this.GetMappings("HeaderMapping") : (IDictionary<string, IMapping>)new Dictionary<string, IMapping>();
            }
        }

        public Func<DataRow, IMapping> MappingConvertFunc { get; set; }

        public Func<DataColumn, IMapping, DataColumn> DataColumnConvertFunc { get; set; }

        public string[] MappingNames
        {
            get
            {
                return this.InnerData.Tables.Cast<DataTable>().Select<DataTable, string>((Func<DataTable, string>)(table => table.TableName)).Where<string>((Func<string, bool>)(name => !((IEnumerable<string>)MappingDictionary.__systemTable).Contains<string>(name) && !name.StartsWith("_"))).ToArray<string>();
            }
        }

        protected internal DataSet InnerData { get; private set; }

        public static MappingDictionary Empty
        {
            get
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    XElement.Parse("<Mappings/>").Save((Stream)memoryStream);
                    if (memoryStream.CanSeek)
                        memoryStream.Seek(0L, SeekOrigin.Begin);
                    MappingDictionary mappingDictionary = MappingDictionary.Parse((Stream)memoryStream);
                    mappingDictionary.IsEmpty = true;
                    return mappingDictionary;
                }
            }
        }

        private MappingDictionary(Func<DataRow, IMapping> convert, DataSet dataSet)
        {
            if (dataSet == null)
                throw new ArgumentNullException(nameof(dataSet));
            this.MappingConvertFunc = convert == null ? new Func<DataRow, IMapping>(this.CovertMapping) : convert;
            this.Metas = (IDictionary<string, string>)new Dictionary<string, string>(8);
            this.Settings = (IDictionary<string, string>)new Dictionary<string, string>(16);
            this._needFields = (IList<string>)null;
            this.Init(dataSet);
            if (dataSet.Tables.Contains(nameof(Mappings)))
            {
                DataTable table = dataSet.Tables[nameof(Mappings)];
                if (table.Rows.Count > 0)
                {
                    DataRow row = table.Rows[0];
                    foreach (DataColumn column in (InternalDataCollectionBase)table.Columns)
                    {
                        string version = string.Format("{0}", row[column.ColumnName]);
                        switch (column.ColumnName)
                        {
                            case "Mappings_Id":
                                continue;
                            case "version":
                                this.Version = new MappingVersion(version);
                                break;
                        }
                        this.Metas.Add(column.ColumnName, version);
                    }
                }
            }
            if (this.Version == null)
                this.Version = MappingVersion.Default;
            this.GetNeededFields();
        }

        public static MappingDictionary GetDict(string configKey)
        {
            if (string.IsNullOrEmpty(configKey))
                throw new ArgumentNullException(nameof(configKey));
            switch (GlobalConfiguration.Mode)
            {
                case RunMode.EncompassServer:
                    using (Stream inStream = GlobalConfiguration.CurrentSession.DataExchange.GetCustomDataObject(GlobalConfiguration.AppSettings[configKey]).OpenStream())
                        return MappingDictionary.Parse(inStream);
                case RunMode.WebServer:
                case RunMode.Client:
                    return MappingDictionary.Parse(string.Format("{0}//{1}", (object)Environment.CurrentDirectory, (object)GlobalConfiguration.AppSettings[configKey]));
                default:
                    throw new ArgumentNullException("Can't support get mapping dictionary for current mode.");
            }
        }

        public static MappingDictionary ParseText(
          IList<string> fieldIDs,
          TextMappingParser parser)
        {
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                StreamWriter streamWriter = new StreamWriter((Stream)memoryStream);
                try
                {
                    foreach (string fieldId in (IEnumerable<string>)fieldIDs)
                        streamWriter.WriteLine(fieldId);
                    streamWriter.Flush();
                    return MappingDictionary.ParseText((Stream)memoryStream, parser);
                }
                finally
                {
                    streamWriter?.Close();
                }
            }
            finally
            {
                memoryStream?.Close();
            }
        }

        public static MappingDictionary ParseText(
          string fileName,
          TextMappingParser parser)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (!File.Exists(fileName))
                throw new FileNotFoundException(string.Format("Can't find mapping file:{0}", (object)fileName));
            FileStream fileStream = File.OpenRead(fileName);
            try
            {
                return MappingDictionary.ParseText((Stream)fileStream, parser);
            }
            finally
            {
                fileStream?.Close();
            }
        }

        public static MappingDictionary ParseText(
          Stream stream,
          TextMappingParser parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            DataSet dataSet = new DataSet();
            StreamReader streamReader = new StreamReader(stream);
            DataTable table = (DataTable)null;
            try
            {
                while (!streamReader.EndOfStream)
                {
                    string line = string.Format("{0}", (object)streamReader.ReadLine()).Trim();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        string tableName;
                        if (parser.ParseTableName(line, out tableName))
                        {
                            table?.AcceptChanges();
                            if (dataSet.Tables.Contains(tableName))
                                throw new Exception(string.Format("Duplication table name:{0}.", (object)tableName));
                            table = parser.CreateSchema(tableName);
                            dataSet.Tables.Add(table);
                        }
                        else
                        {
                            if (table == null)
                            {
                                table = parser.CreateSchema(parser.DefaultTableName);
                                dataSet.Tables.Add(table);
                            }
                            DataRow row = table.NewRow();
                            Enums.ValueType valueType = parser.ParseTypeFunc == null ? Enums.ValueType.Unknown : parser.ParseTypeFunc(line);
                            object[] objArray = new object[4]
                            {
                parser.DataColumnNameConvertFunc != null ? (object) parser.DataColumnNameConvertFunc(line) : (object) line,
                (object) line,
                (object) string.Format("[{0}]", (object) line),
                (object) valueType.ToString()
                            };
                            row.ItemArray = objArray;
                            table.Rows.Add(row);
                        }
                    }
                    else
                        break;
                }
                table?.AcceptChanges();
                dataSet.AcceptChanges();
            }
            finally
            {
                streamReader?.Close();
            }
            return new MappingDictionary(parser.MappingConvertFunc, dataSet)
            {
                DataColumnConvertFunc = parser.DataColumnConvertFunc
            };
        }

        public static MappingDictionary ParseText(
          Assembly assembly,
          string resName,
          TextMappingParser parser)
        {
            Stream manifestResourceStream = assembly.GetManifestResourceStream(resName);
            try
            {
                return MappingDictionary.ParseText(manifestResourceStream, parser);
            }
            finally
            {
                manifestResourceStream?.Close();
            }
        }

        private static DataTable CreateSchema(string tableName)
        {
            return new DataTable(tableName)
            {
                Columns = {
          {
            "Field_Name",
            typeof (string)
          },
          {
            "Description",
            typeof (string)
          },
          {
            "Translation_Or_Calculation",
            typeof (string)
          },
          {
            "ValueType",
            typeof (string)
          }
        }
            };
        }

        public static MappingDictionary Parse(Session session, string mappingFile)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));
            if (string.IsNullOrEmpty(mappingFile))
                throw new ArgumentNullException(nameof(mappingFile));
            switch (GlobalConfiguration.Mode)
            {
                case RunMode.EncompassServer:
                    return MappingDictionary.getMappingDictFromServer(session, mappingFile);
                case RunMode.WebServer:
                case RunMode.Client:
                    return File.Exists(mappingFile) ? MappingDictionary.Parse(new FileInfo(mappingFile).FullName) : MappingDictionary.getMappingDictFromServer(session, mappingFile);
                default:
                    throw new NotSupportedException(string.Format("Invalid support mode:{0}", (object)GlobalConfiguration.Mode));
            }
        }

        public static MappingDictionary getMappingDictFromServer(
          Session session,
          string mappingFile)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));
            if (string.IsNullOrEmpty(mappingFile))
                throw new ArgumentNullException(nameof(mappingFile));
            DataObject customDataObject = session.DataExchange.GetCustomDataObject(mappingFile);
            if (customDataObject == null || customDataObject.Size == 0)
                throw new FileNotFoundException(string.Format("Can't find mapping file: {0}", (object)mappingFile));
            using (Stream inStream = customDataObject.OpenStream())
                return MappingDictionary.Parse(inStream);
        }

        public static MappingDictionary Parse(string fileName)
        {
            return MappingDictionary.Parse(fileName, (Func<DataRow, IMapping>)null);
        }

        public static MappingDictionary Parse(
          string fileName,
          Func<DataRow, IMapping> convert)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (!File.Exists(fileName))
                throw new FileNotFoundException(string.Format("Can't find mapping file:{0}", (object)fileName));
            FileStream fileStream = File.OpenRead(fileName);
            try
            {
                return MappingDictionary.Parse((Stream)fileStream, convert);
            }
            finally
            {
                fileStream?.Close();
            }
        }

        public static MappingDictionary Parse(Stream inStream)
        {
            return MappingDictionary.Parse(inStream, (Func<DataRow, IMapping>)null);
        }

        public static MappingDictionary Parse(
          Stream inStream,
          Func<DataRow, IMapping> convert)
        {
            if (inStream == null)
                throw new ArgumentNullException(nameof(inStream));
            DataSet dataSet = new DataSet();
            int num = (int)dataSet.ReadXml(inStream);
            if (dataSet.Tables.Count == 0)
                dataSet.Tables.Add("Mapping");
            return new MappingDictionary(convert, dataSet);
        }

        public static MappingDictionary Parse(Assembly assembly, string resName)
        {
            return MappingDictionary.Parse(assembly, resName, (Func<DataRow, IMapping>)null);
        }

        public static MappingDictionary Parse(
          Assembly assembly,
          string resName,
          Func<DataRow, IMapping> convert)
        {
            Stream manifestResourceStream = assembly.GetManifestResourceStream(resName);
            try
            {
                MappingDictionary mappingDictionary = MappingDictionary.Parse(manifestResourceStream, convert);
                mappingDictionary.IsEmpty = mappingDictionary.Mappings.Count == 0;
                return mappingDictionary;
            }
            finally
            {
                manifestResourceStream?.Close();
            }
        }

        public bool GetBoolSetting(string key)
        {
            return this.GetBoolSetting(key, false);
        }

        public bool GetBoolSetting(string key, bool defaultValue)
        {
            return string.Compare(this.GetSetting(key, defaultValue ? "true" : "false"), "true", true) == 0;
        }

        public string GetSetting(string key)
        {
            return this.GetSetting(key, string.Empty);
        }

        public string GetSetting(string key, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            return MappingDictionary.GetSetting(this, key, defaultValue);
        }

        public static string GetSetting(MappingDictionary mDict, string key)
        {
            return MappingDictionary.GetSetting(mDict, key, string.Empty);
        }

        public static string GetSetting(MappingDictionary mDict, string key, string defaultValue)
        {
            if (mDict == null)
                throw new ArgumentNullException(nameof(mDict));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            return MappingDictionary.GetSetting(mDict.Settings, key, defaultValue);
        }

        public static string GetSetting(IDictionary<string, string> settings, string key)
        {
            return MappingDictionary.GetSetting(settings, key, string.Empty);
        }

        public static string GetSetting(
          IDictionary<string, string> settings,
          string key,
          string defaultValue)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            string str = string.Empty;
            if (settings.ContainsKey(key))
                str = settings[key];
            if (string.IsNullOrWhiteSpace(str))
                str = GlobalConfiguration.AppSettings[key];
            return !string.IsNullOrWhiteSpace(str) ? str : defaultValue;
        }

        internal void Init(DataSet dataSet)
        {
            if (dataSet == null)
                throw new ArgumentNullException(nameof(dataSet));
            this.InnerData = dataSet;
            string[] strArray = new string[4]
            {
        "Header",
        "Setting",
        "Mapping",
        "HeaderMapping"
            };
            foreach (string name in strArray)
            {
                DataTable table = this.InnerData.Tables.Contains(name) ? dataSet.Tables[name] : (DataTable)null;
                if (table != null)
                {
                    switch (name)
                    {
                        case "Header":
                        case "Setting":
                            if (!(name == "Header") || !((IEnumerable<string>)this.MappingNames).Contains<string>("Setting"))
                            {
                                this.Settings = (IDictionary<string, string>)table.Rows.Cast<DataRow>().Select<DataRow, string[]>((Func<DataRow, string[]>)(row => new string[2]
                              {
                  string.Format("{0}", row["Key"]),
                  string.Format("{0}", row["Value"])
                              })).Where<string[]>((Func<string[], bool>)(ss => !string.IsNullOrWhiteSpace(ss[0]))).ToDictionary<string[], string, string>((Func<string[], string>)(ss => ss[0]), (Func<string[], string>)(ss => ss[1]));
                                continue;
                            }
                            continue;
                        case "Mapping":
                            if (table.Columns.Contains("TableName"))
                            {
                                this.convertToTables(table);
                                continue;
                            }
                            continue;
                        default:
                            continue;
                    }
                }
            }
        }

        private void convertToTables(DataTable table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));
            DataSet dataSet = table.DataSet;
            foreach (string str in table.Rows.Cast<DataRow>().GroupBy<DataRow, string>((Func<DataRow, string>)(r => string.Format("{0}", r["TableName"]))).Select<IGrouping<string, DataRow>, string>((Func<IGrouping<string, DataRow>, string>)(g => g.Key)).ToArray<string>())
            {
                string tableName = str;
                DataTable table1 = table.Clone();
                table1.TableName = tableName;
                if (table1.Columns.Contains("TableName"))
                    table1.Columns.Remove("TableName");
                foreach (DataRow dataRow in table.Rows.Cast<DataRow>().Where<DataRow>((Func<DataRow, bool>)(r => string.Format("{0}", r["TableName"]) == tableName)).Select<DataRow, DataRow>((Func<DataRow, DataRow>)(r => r)).ToArray<DataRow>())
                {
                    DataRow row = table1.NewRow();
                    foreach (DataColumn column in (InternalDataCollectionBase)table1.Columns)
                        row[column] = dataRow[column.ColumnName];
                    table1.Rows.Add(row);
                }
                dataSet.Tables.Add(table1);
            }
            dataSet.Tables.Remove(table);
            dataSet.AcceptChanges();
        }

        private IMapping CovertMapping(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            string version = this.Version.Version;
            string strA = this.Metas.ContainsKey("type") ? this.Metas["type"] : string.Empty;
            if (string.Compare(strA, "export", true) == 0)
                return ExportMapping.Create(row);
            if (string.Compare(strA, "import", true) == 0)
                return ImportMapping.Create(row);
            if (version == MappingVersion.Default.Version)
                return ExportMapping.Create(row);
            throw new Exception("Please set value of type in mapping metas.");
        }

        public IDictionary<string, IMapping> GetMappings(string tableName)
        {
            if (string.IsNullOrWhiteSpace(nameof(tableName)))
                throw new ArgumentNullException(nameof(tableName));
            if (!this.InnerData.Tables.Contains(tableName))
                throw new ArgumentException(string.Format("Invalid table name:{0}", (object)tableName));
            return (IDictionary<string, IMapping>)this.InnerData.Tables[tableName].Rows.Cast<DataRow>().Select<DataRow, IMapping>((Func<DataRow, int, IMapping>)((row, i) => this.MappingConvertFunc(row))).ToDictionary<IMapping, string, IMapping>((Func<IMapping, string>)(m => string.Format("{0}_{1}", (object)m.ColumnName, (object)Guid.NewGuid())), (Func<IMapping, IMapping>)(m => m));
        }

        public IDictionary<string, IXmlMapping> GetXmlMappings(
          string tableName,
          bool nameAsXPath)
        {
            if (string.IsNullOrWhiteSpace(nameof(tableName)))
                throw new ArgumentNullException(nameof(tableName));
            if (!this.InnerData.Tables.Contains(tableName))
                throw new ArgumentException(string.Format("Invalid table name:{0}", (object)tableName));
            return (IDictionary<string, IXmlMapping>)this.GetMappings(tableName).Cast<KeyValuePair<string, IMapping>>().ToDictionary<KeyValuePair<string, IMapping>, string, IXmlMapping>((Func<KeyValuePair<string, IMapping>, string>)(p => p.Key), (Func<KeyValuePair<string, IMapping>, IXmlMapping>)(p => XmlMapping.Convert(p.Value, this, nameAsXPath)));
        }

        public IList<string> GetNeededFields()
        {
            if (this._needFields == null)
            {
                this._needFields = (IList<string>)new List<string>(16);
                foreach (string mappingName in this.MappingNames)
                {
                    foreach (IMapping mapping1 in (IEnumerable<IMapping>)this.GetMappings(mappingName).Values)
                    {
                        IMapping mapping = mapping1;
                        Action<string> action = (Action<string>)(s =>
                        {
                            s = this.ProcessFieldID(s, mapping);
                            if (this._needFields.Contains(s))
                                return;
                            this._needFields.Add(s);
                        });
                        switch (mapping.TranslationType)
                        {
                            case TranslationType.Directly:
                                action(mapping.Translation);
                                break;
                            case TranslationType.Expression:
                            case TranslationType.Macro:
                                foreach (string str in Translator.GetNeededFieldsByTranslation(mapping.Translation).ToArray<string>())
                                    action(str);
                                break;
                            default:
                                continue;
                        }
                        if (mapping.Properties.ContainsKey("DoNotExportCriteria"))
                        {
                            foreach (string str in (IEnumerable<string>)Translator.GetNeededFieldsByTranslation(mapping.GetProperty("DoNotExportCriteria")))
                                action(str);
                        }
                    }
                }
                if (!this._needFields.Contains("364"))
                    this._needFields.Add("364");
            }
            return this._needFields;
        }

        protected virtual string ProcessFieldID(string fieldID, IMapping mapping)
        {
            return !fieldID.StartsWith("[") || !fieldID.EndsWith("]") ? fieldID : fieldID.Substring(1, fieldID.Length - 2);
        }
    }
}
