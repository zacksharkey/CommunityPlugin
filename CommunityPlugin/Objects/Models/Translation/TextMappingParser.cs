using CommunityPlugin.Objects.BaseClasses;
using CommunityPlugin.Objects.Interface;
using System;
using System.Data;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class TextMappingParser
    {
        protected const string PrefixOfTableDefine = "<<";
        protected const string SuffixOfTableDefine = ">>";
        private const string DefinedDefaultTableName = "Table1";
        private static TextMappingParser __parser;

        public string DefaultTableName { get; set; }

        public Func<string, Enums.ValueType> ParseTypeFunc { get; protected set; }

        public Func<DataRow, IMapping> MappingConvertFunc { get; set; }

        public Func<DataColumn, IMapping, DataColumn> DataColumnConvertFunc { get; set; }

        public Func<string, string> DataColumnNameConvertFunc { get; set; }

        protected TextMappingParser()
        {
            this.DefaultTableName = "Table1";
        }

        public virtual bool ParseTableName(string line, out string tableName)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new ArgumentNullException(nameof(line));
            tableName = string.Empty;
            if (!line.StartsWith("<<") || !line.EndsWith(">>"))
                return false;
            int length1 = "<<".Length;
            int length2 = ">>".Length;
            tableName = line.Substring(length1, line.Length - length1 - length2);
            return true;
        }

        public virtual DataTable CreateSchema(string tableName)
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

        private static IMapping MappingConvert(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException("null");
            return Mapping.Create(row);
        }

        public static TextMappingParser Create()
        {
            if (TextMappingParser.__parser == null)
            {
                TextMappingParser.__parser = new TextMappingParser();
                TextMappingParser.__parser.MappingConvertFunc = new Func<DataRow, IMapping>(TextMappingParser.MappingConvert);
            }
            return TextMappingParser.__parser;
        }
    }
}
