
using CommunityPlugin.Objects.BaseClasses;
using CommunityPlugin.Objects.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CommunityPlugin.Objects.Models.Translation
{
    public static class XmlMapping
    {
        private const string PropertyNameForXPath = "XPath";
        private const string TableNameOfXPathGroup = "_XPathGroup";
        private const string ColumnNameOfName = "Name";
        private const string ColumnNameOfXPath = "XPath";

        public static object ProcessParameter(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return (object)string.Empty;
            parameter = parameter.Trim();
            if (parameter.StartsWith("\"") && parameter.EndsWith("\""))
                parameter = parameter.Substring(1, parameter.Length - 2).Trim();
            return (object)parameter;
        }

        public static string GetXPathByMapping(IMapping mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));
            return mapping.GetProperty("XPath");
        }

        public static IXmlMapping Convert(
          IMapping mapping,
          MappingDictionary mDict,
          bool nameAsXPath)
        {
            if (nameAsXPath && string.IsNullOrWhiteSpace(mapping.GetProperty("XPath")))
            {
                string columnName = mapping.ColumnName;
                mapping.Properties["XPath"] = columnName;
                if (mapping is IXmlMapping)
                    (mapping as IXmlMapping).XPath = columnName;
            }
            return XmlMapping.Convert(mapping, mDict);
        }

        public static IXmlMapping Convert(IMapping mapping, MappingDictionary mDict)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));
            if (mDict == null)
                throw new ArgumentNullException(nameof(mDict));
            if (!(mapping is IXmlMapping))
                throw new InvalidCastException("Can't convert current mapping to IXmlMapping.");
            IXmlMapping xmlMapping = mapping as IXmlMapping;
            string xpath = xmlMapping.XPath;
            if (xpath.StartsWith("@@"))
            {
                string xpathByMacro = XmlMapping.GetXPathByMacro(xpath, mDict);
                if (!string.IsNullOrWhiteSpace(xpathByMacro))
                    xmlMapping.XPath = xpathByMacro;
            }
            return xmlMapping;
        }

        public static string GetXPathByMacro(string xpath, MappingDictionary mDict)
        {
            if (string.IsNullOrWhiteSpace(xpath))
                throw new ArgumentNullException(nameof(xpath));
            if (mDict == null)
                throw new ArgumentNullException(nameof(mDict));
            string[] parameters;
            string name;
            if (Translator.ParseMacro(xpath, out name, out parameters, out string _))
            {
                object[] array = ((IEnumerable<string>)parameters).Select<string, object>((Func<string, object>)(p => XmlMapping.ProcessParameter(p))).ToArray<object>();
                if (mDict.InnerData.Tables.Contains("_XPathGroup"))
                {
                    string format = mDict.InnerData.Tables["_XPathGroup"].Rows.Cast<DataRow>().Where<DataRow>((Func<DataRow, bool>)(r => string.Format("{0}", r["Name"]) == name)).Select<DataRow, string>((Func<DataRow, string>)(r => string.Format("{0}", r["XPath"]))).FirstOrDefault<string>();
                    if (!string.IsNullOrWhiteSpace(format))
                    {
                        try
                        {
                            return string.Format(format, array);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}
