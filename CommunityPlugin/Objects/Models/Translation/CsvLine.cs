using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class CsvLine
    {
        private const char DefaultSplitChar = ',';
        private const string CR = "\r";
        private const string LF = "\n";
        private const string CRLF = "\r\n";
        private const string Space = " ";
        private int _count;
        private char[] _splitChars;
        private StringBuilder _builder;

        public char[] SplitChars
        {
            get
            {
                return this._splitChars;
            }
        }

        public int ItemCount
        {
            get
            {
                return this._count;
            }
        }

        public bool AlwaysQuote { get; set; }

        public bool TrimCellValue { get; set; }

        public bool ReplaceSpecialChars { get; set; }

        public string PrefixOfNumber { get; set; }

        public string PrefixOfDateTime { get; set; }

        public CsvLine()
          : this(new char[1] { ',' })
        {
        }

        public CsvLine(char[] splitChars)
        {
            this._splitChars = splitChars;
            this.AlwaysQuote = false;
            this.ReplaceSpecialChars = false;
            this.PrefixOfNumber = string.Empty;
            this.PrefixOfDateTime = string.Empty;
        }

        public static IList<string> Split(string line)
        {
            return CsvLine.SplitByChar(line, ',');
        }

        public static IList<string> SplitByChar(string line, char splitChar)
        {
            int startIndex = 0;
            bool flag = false;
            List<string> stringList = new List<string>();
            for (int index = 0; index < line.Length; ++index)
            {
                if ((int)line[index] == (int)splitChar)
                {
                    if (!flag)
                    {
                        stringList.Add(line.Substring(startIndex, index - startIndex).Trim());
                        startIndex = index + 1;
                    }
                }
                else if (line[index] == '"')
                    flag = !flag;
            }
            stringList.Add(line.Substring(startIndex).Trim());
            for (int index = 0; index < stringList.Count; ++index)
            {
                string str1 = stringList[index];
                string str2 = str1.StartsWith("\"") ? str1.Substring(1) : str1;
                string str3 = (str2.EndsWith("\"") ? str2.Substring(0, str2.Length - 1) : str2).Replace("\"\"", "\"");
                stringList[index] = str3;
            }
            return (IList<string>)stringList;
        }

        public void AddItem(string item)
        {
            this.AddItem(item, string.Empty);
        }

        public void AddItem(string item, string prefix)
        {
            if (this.ReplaceSpecialChars)
            {
                item = item.Replace(",", string.Empty);
                item = item.Replace("-", string.Empty);
            }
            if (item.Contains("\""))
            {
                item = item.Replace("\"", "\"\"");
                item = string.Format("\"{0}\"", (object)item);
            }
            else if (item.Contains(",") && ((IEnumerable<char>)this._splitChars).Contains<char>(',') || this.AlwaysQuote)
                item = string.Format("\"{0}\"", (object)item);
            if (item.Contains("\r\n"))
                item = item.Replace("\r\n", " ");
            if (item.Contains("\r"))
                item = item.Replace("\r", " ");
            if (item.Contains("\n"))
                item = item.Replace("\n", " ");
            if (!string.IsNullOrEmpty(prefix) && item.IndexOf(',') < 0)
                item = string.Format("{0}{1}", (object)prefix, (object)item);
            if (this._builder == null)
                this._builder = new StringBuilder();
            else
                this._builder.Append(this.SplitChars);
            this._builder.Append(item);
            ++this._count;
        }

        public void Load(DataRow row)
        {
            foreach (DataColumn column in (InternalDataCollectionBase)row.Table.Columns)
            {
                string s = string.Format("{0}", row[column.ColumnName]);
                if (this.TrimCellValue)
                    s = s.Trim();
                if (!string.IsNullOrEmpty(this.PrefixOfNumber) && double.TryParse(s, out double _))
                    this.AddItem(s, this.PrefixOfNumber);
                else if (!string.IsNullOrEmpty(this.PrefixOfDateTime) && DateTime.TryParse(s, out DateTime _))
                    this.AddItem(s, this.PrefixOfDateTime);
                else
                    this.AddItem(s);
            }
        }

        public override string ToString()
        {
            return this._builder.ToString();
        }

        public class CVSCell
        {
            private bool _IsDoubleQuotBegin;
            private int _DoubleQuotCount;

            public string Value { get; private set; }

            public bool IsComplete { get; private set; }

            public void Complete()
            {
                if (this.IsComplete)
                    return;
                if (this.Value.StartsWith("\""))
                    this.Value = this.Value.Remove(0, 1);
                if (this.Value.EndsWith("\""))
                    this.Value = this.Value.Remove(this.Value.Length - 1, 1);
                this.Value = this.Value.Replace("\"\"", "\"");
                this.IsComplete = true;
            }

            public void AddChar(char c)
            {
                if (this.IsComplete)
                    throw new Exception("Can not add char,you can user method Complete to reset me");
                switch (c)
                {
                    case '"':
                        if (this.Value == string.Empty)
                            this._IsDoubleQuotBegin = true;
                        this.Value += (string)(object)c;
                        ++this._DoubleQuotCount;
                        break;
                    case ',':
                        if (this._IsDoubleQuotBegin && this._DoubleQuotCount % 2 == 1)
                        {
                            this.Value += (string)(object)c;
                            break;
                        }
                        this.Complete();
                        break;
                    default:
                        this.Value += (string)(object)c;
                        break;
                }
            }

            public void Reset()
            {
                this.Value = string.Empty;
                this._IsDoubleQuotBegin = false;
                this.IsComplete = false;
            }
        }
    }
}
