
using CommunityPlugin.Objects.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class Translator : ITranslator
    {
        internal const string TrueValue = "true";
        internal const string FalseValue = "false";
        internal const string PrefixOfDefintion = "#define";
        private ICriterionParser _parser;

        public ICriterionParser Parser
        {
            get
            {
                if (this._parser == null)
                {
                    lock (this)
                    {
                        if (this._parser == null)
                            this._parser = this.CreateParser();
                    }
                }
                return this._parser;
            }
        }

        public Translator(ICriterionParser parser)
        {
            if (parser == null)
                return;
            this._parser = parser;
        }

        public static IList<string> GetNeededFieldsByTranslation(string translation)
        {
            if (string.IsNullOrEmpty("content"))
                throw new ArgumentNullException("content");
            if (translation.ToLower() == "[]" || translation.ToLower() == "[blank]")
                return (IList<string>)new List<string>();
            List<string> stringList = new List<string>(8);
            if (translation.StartsWith("#define"))
            {
                string str1 = translation;
                string[] separator = new string[1] { "\r\n" };
                foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!str2.StartsWith("#define"))
                    {
                        try
                        {
                            foreach (string str3 in (IEnumerable<string>)Translator.GetNeededFieldsByTranslation(string.Format("{0}", (object)str2).Trim()))
                            {
                                if (!stringList.Contains(str3))
                                    stringList.Add(str3);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                return (IList<string>)stringList;
            }
            int num = -1;
            for (int index = 0; index < translation.Length; ++index)
            {
                char ch = translation[index];
                if (ch == '[')
                    num = index;
                if (ch == ']' && num != -1)
                {
                    string str = string.Format("{0}", (object)translation.Substring(num + 1, index - num - 1)).Trim();
                    if (!stringList.Contains(str))
                        stringList.Add(str);
                    num = -1;
                }
            }
            return (IList<string>)stringList;
        }

        public static bool ParseMacro(
          string macro,
          out string name,
          out string[] parameters,
          out string expression)
        {
            name = string.Empty;
            parameters = new string[0];
            expression = string.Empty;
            if (string.IsNullOrEmpty(macro))
                return false;
            int length1 = macro.IndexOf('(');
            int num1 = macro.IndexOf(')');
            int length2 = macro.IndexOf('{');
            int num2 = macro.LastIndexOf('}');
            if (length1 < 0 && num1 < 0 && (length2 < 0 && num2 < 0))
            {
                name = macro.Trim();
                return true;
            }
            if (length1 > 0 && num1 > 0)
            {
                if ((length2 < 0 || length1 >= length2 || num1 >= length2) && length2 >= 0)
                    return false;
                name = string.Format("{0}", (object)macro.Substring(0, length1)).Trim();
                string str = macro.Substring(length1 + 1, num1 - length1 - 1).Trim();
                if (string.IsNullOrWhiteSpace(str))
                    parameters = new string[0];
                else
                    parameters = str.Split(',');
            }
            if (length2 > 0 && num2 > 0)
            {
                if (length2 > num2)
                    return false;
                if (string.IsNullOrWhiteSpace(name))
                    name = string.Format("{0}", (object)macro.Substring(0, length2)).Trim();
                expression = macro.Substring(length2 + 1, num2 - length2 - 1).Trim();
            }
            return true;
        }

        public static bool ParseTargetExpression(
          string expression,
          out string targetFieldId,
          out string criterion)
        {
            return Translator.ParseTargetExpression(expression, "=", out targetFieldId, out criterion);
        }

        public static bool ParseTargetExpression(
          string expression,
          string split,
          out string targetFieldId,
          out string criterion)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentNullException(nameof(expression));
            if (string.IsNullOrWhiteSpace(split))
                throw new ArgumentNullException(nameof(split));
            targetFieldId = string.Empty;
            criterion = string.Empty;
            int num1 = expression.IndexOf("[");
            int num2 = expression.IndexOf("]");
            if (num1 < 0 || num2 < 0 || num2 < num1)
                return false;
            targetFieldId = expression.Substring(num1 + 1, num2 - num1 - 1).Trim();
            expression = expression.Substring(num2 + 1, expression.Length - num2 - 1);
            int num3 = expression.IndexOf(split);
            if (num3 < 0)
                return false;
            criterion = expression.Substring(num3 + 1, expression.Length - num3 - 1).Trim();
            return true;
        }

        protected virtual ICriterionParser CreateParser()
        {
            throw new NotSupportedException("Not support to create parser in base class.");
        }

        public string GetFieldValue(string fieldID, IMapping mapping)
        {
            if (string.IsNullOrEmpty(fieldID) || fieldID.Trim().ToLower() == "blank" || (fieldID.Trim() == "\"\"" || fieldID.Trim() == "##") || fieldID.Trim() == "?")
                return string.Empty;
            return fieldID.StartsWith("\"") && fieldID.EndsWith("\"") && fieldID.Length >= 2 ? fieldID.Substring(1, fieldID.Length - 2) : this.GetTranslatioinValue(string.Format("[{0}]", (object)fieldID), mapping);
        }

        public virtual string GetTranslatioinValue(string translation, IMapping mapping)
        {
            translation = string.Format("{0}", (object)translation).Trim();
            if (translation.ToLower().StartsWith("#define"))
            {
                int num1 = translation.IndexOf("{");
                int num2 = translation.LastIndexOf("}");
                translation = translation.Substring(num1 + 1, num2 - num1 - 1);
                return this.Parser.ExecuteDefinition(translation, mapping);
            }
            StringReader stringReader = new StringReader(translation);
            try
            {
                string translation1;
                string result;
                string conditions;
                do
                {
                    string str = stringReader.ReadLine();
                    if (str != null)
                        translation1 = string.Format("{0}", (object)str).Trim();
                    else
                        goto label_6;
                }
                while (string.IsNullOrEmpty(translation1) || translation1.ToLower() == "else" || Translator.TryToParseIfTranslation(translation1, out result, out conditions) && !this._parser.Test(conditions, mapping));
                return this._parser.GetResult(result, mapping);
            label_6:
                return string.Empty;
            }
            finally
            {
                stringReader?.Close();
            }
        }

        protected internal static IList<string> GetTranslations(string translation)
        {
            translation = string.Format("{0}", (object)translation).Trim();
            IList<string> stringList = (IList<string>)new List<string>(16);
            StringReader stringReader = new StringReader(translation);
            try
            {
                while (true)
                {
                    string str1;
                    do
                    {
                        string str2 = stringReader.ReadLine();
                        if (str2 != null)
                            str1 = string.Format("{0}", (object)str2).Trim();
                        else
                            goto label_4;
                    }
                    while (string.IsNullOrEmpty(str1));
                    stringList.Add(str1);
                }
            label_4:
                return stringList;
            }
            finally
            {
                stringReader?.Close();
            }
        }

        protected internal static bool TryToParseIfTranslation(
          string translation,
          out string result,
          out string conditions)
        {
            translation = string.Format("{0}", (object)translation).Trim();
            result = translation;
            conditions = string.Empty;
            if (string.IsNullOrEmpty(translation))
                return false;
            Regex regex = new Regex(":\\s+If", RegexOptions.IgnoreCase);
            if (!regex.IsMatch(translation))
            {
                result = translation.Trim();
                return false;
            }
            string[] strArray = regex.Split(translation);
            string separator = regex.Match(translation).Value;
            if (strArray.Length == 2)
            {
                string str1 = strArray[0].Trim();
                string str2 = strArray[1].Trim();
                if (!str1.StartsWith("\"") || str1.StartsWith("\"") && str1.EndsWith("\""))
                {
                    result = str1;
                    conditions = str2;
                    return true;
                }
            }
            bool flag = false;
            int num = 0;
            for (int index = 0; index < strArray.Length; ++index)
            {
                string str = strArray[index].Trim();
                if (str.StartsWith("\""))
                    flag = true;
                if (str.EndsWith("\""))
                    flag = false;
                if (!flag)
                {
                    num = index + 1;
                    break;
                }
            }
            if (num <= 0)
                return false;
            result = string.Join(separator, strArray, 0, num);
            conditions = string.Join(separator, strArray, num, strArray.Length - num);
            return true;
        }
    }
}
