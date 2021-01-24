using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.BaseClasses;
using CommunityPlugin.Objects.Enums;
using CommunityPlugin.Objects.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class CriterionParser : ICriterionParser
    {
        internal static readonly string[] SignalList = new string[11]
        {
      "**?",
      "*?*",
      "?**",
      "==",
      "<>",
      ">=",
      "<=",
      "!=",
      ">",
      "<",
      "="
        };
        internal static readonly DateTime EmptyDate = DateTime.Parse("#01/01/0001 12:00:00 AM#");
        internal const string LogicList = "And|Or|Xor";
        protected object psource;
        protected CriterionParser.Replacer preplacer;
        private IFunctionParser _functionParser;

        public Func<string, string[], string> CustomFunctions { get; set; }

        protected internal IFunctionParser FunctionParser
        {
            get
            {
                if (this._functionParser == null)
                {
                    lock (this)
                    {
                        if (this._functionParser == null)
                            this._functionParser = this.CreateFunctionParser();
                    }
                }
                return this._functionParser;
            }
        }

        protected CriterionParser()
        {
            this.preplacer = new CriterionParser.Replacer();
        }

        protected virtual IFunctionParser CreateFunctionParser()
        {
           throw new NotSupportedException("The super class not support this method.");
        }

        public bool Test(string criterionString)
        {
            return this.Test(criterionString, (IMapping)null);
        }

        public bool Test(string criterionString, IMapping mapping)
        {
            //if (string.IsNullOrEmpty(criterionString))
            //    throw new ArgumentNullException(nameof(criterionString));
            this.preplacer.Reset();
            string str = this.preplacer.ReplaceValue(this.ProcessCurrentDateTime(criterionString));
            this.CheckCriterionAfterReplace(str);
            return this.ConvertFullExpression(str, mapping, true) == "true";
        }

        protected virtual void CheckCriterionAfterReplace(string criterions)
        {
        }

        public string GetResult(string criterionString)
        {
            return this.GetResult(criterionString, (IMapping)null);
        }

        public string GetResult(string criterionString, IMapping mapping)
        {
           // if (criterionString == null)
               // throw new ArgumentNullException(nameof(criterionString));
            if (criterionString == string.Empty)
                return string.Empty;
            this.preplacer.Reset();
            string str = this.preplacer.ReplaceValue(this.ProcessCurrentDateTime(criterionString));
            this.CheckCriterionAfterReplace(str);
            return this.ConvertValues(this.GetItemValue(this.ConvertFullExpression(str, mapping, false), mapping), mapping);
        }

        public string ExecuteDefinition(string definition)
        {
            return this.ExecuteDefinition(definition, (IMapping)null);
        }

        public string ExecuteDefinition(string definition, IMapping mapping)
        {
            return this.ExecuteDefinition(definition, mapping, (IDictionary<string, string>)new Dictionary<string, string>(8));
        }

        public string ExecuteDefinition(
          string definition,
          IMapping mapping,
          IDictionary<string, string> variants)
        {
            if (string.IsNullOrEmpty(definition))
                return string.Empty;
            IList<string> translations = Translator.GetTranslations(definition);
            Stack<bool> boolStack = new Stack<bool>(translations.Count / 2);
            Stack<string> stringStack = new Stack<string>(translations.Count / 4);
            IDictionary<string, StringBuilder> dictionary = (IDictionary<string, StringBuilder>)new Dictionary<string, StringBuilder>(stringStack.Count);
            for (int index1 = 0; index1 < translations.Count; ++index1)
            {
                string str = translations[index1].Trim();
                string lower = str.ToLower();
                string conditions;
                switch (lower)
                {
                    case "else":
                        //if (boolStack.Count == 0)
                            //throw new CriterionException("Invalid \"else\" expression", str);
                        boolStack.Push(!boolStack.Pop());
                        break;
                    case "end if":
                       // if (boolStack.Count == 0)
                            //throw new CriterionException("Invalid \"end if\" expression", str);
                        boolStack.Pop();
                        break;
                    case "end while":
                        //if (stringStack.Count == 0)
                          //  throw new CriterionException("Invalid \"end while\" expression", str);
                        string index2 = stringStack.Pop();
                        if (dictionary.ContainsKey(index2))
                        {
                            this.ParseConditionLine(index2, out conditions);
                            string definition1 = dictionary[index2].ToString();
                            while (this.Test(this.ReplaceVariants(conditions, variants)))
                                this.ExecuteDefinition(definition1, mapping, variants);
                            break;
                        }
                        break;
                    default:
                        if (stringStack.Count > 0)
                        {
                            string[] array = stringStack.ToArray();
                            dictionary[array[array.Length - 1]].AppendFormat("{0}\r\n", (object)str);
                            break;
                        }
                        if (lower.StartsWith("$"))
                        {
                            if (boolStack.Count > 0)
                            {
                                IList<bool> array = (IList<bool>)boolStack.ToArray();
                                if (!array[array.Count - 1])
                                    break;
                            }
                            string variant;
                            string expression;
                            this.ParseVariantLine(str, out variant, out expression);
                            expression = this.ReplaceVariants(expression, variants);
                           // if (string.IsNullOrEmpty(variant))
                                //throw new CriterionException("empty variant in defintion", str);
                            string result = this.GetResult(expression);
                            if (variants.ContainsKey(variant))
                            {
                                variants[variant] = result;
                                break;
                            }
                            variants.Add(variant, result);
                            break;
                        }
                        if (lower.StartsWith("if") || lower.StartsWith("else if"))
                        {
                            if (lower.StartsWith("else if"))
                            {
                               // if (boolStack.Count == 0)
                                    //throw new CriterionException("Invalid \"else if\" line", str);
                                boolStack.Pop();
                            }
                            this.ParseConditionLine(str, out conditions);
                            conditions = this.ReplaceVariants(conditions, variants);
                            if (this.Test(conditions))
                            {
                                boolStack.Push(true);
                                break;
                            }
                            boolStack.Push(false);
                            break;
                        }
                        if (lower.StartsWith("while"))
                        {
                            stringStack.Push(str);
                            if (!dictionary.ContainsKey(str))
                            {
                                dictionary.Add(str, new StringBuilder());
                                break;
                            }
                            break;
                        }
                        if (lower.StartsWith("return"))
                            return this.GetResult(this.ReplaceVariants(str.Substring("return".Length).Trim(), variants));
                        break;
                }
            }
            return string.Empty;
        }

        protected void ParseVariantLine(string line, out string variant, out string expression)
        {
            variant = string.Empty;
            expression = string.Empty;
            if (string.IsNullOrEmpty(line))
                return;
            int length = line.IndexOf("=");
           // if (length <= 0)
               // throw new CriterionException("Invalid definition line.", line);
            variant = line.Substring(0, length).Trim();
            expression = line.Substring(length + 1).Trim();
        }

        protected void ParseConditionLine(string line, out string conditions)
        {
            conditions = string.Empty;
            if (string.IsNullOrEmpty(line))
                return;
            int num1 = line.IndexOf("(");
            int num2 = line.LastIndexOf(")");
            //if (num1 < 0 || num2 < 0)
              //  throw new CriterionException("Invalid if expression.", line);
            conditions = line.Substring(num1 + 1, num2 - num1 - 1);
        }

        protected virtual string ReplaceVariants(string line, IDictionary<string, string> variants)
        {
            if (string.IsNullOrEmpty(line) || variants.Count == 0)
                return line;
            using (IEnumerator<string> enumerator = variants.Keys.GetEnumerator())
            {
            label_7:
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    if (line.ToLower().Contains(current.ToLower()))
                    {
                        while (true)
                        {
                            int length = line.IndexOf(current, StringComparison.OrdinalIgnoreCase);
                            if (length >= 0)
                                line = length != 0 ? string.Format("{0}{1}{2}", (object)line.Substring(0, length), (object)variants[current], (object)line.Substring(length + current.Length)) : string.Format("{0}{1}", (object)variants[current], (object)line.Substring(current.Length));
                            else
                                goto label_7;
                        }
                    }
                }
            }
            return line;
        }

        protected virtual string ConvertFullExpression(
          string criterionPart,
          IMapping mapping,
          bool checkLogic)
        {
            IList<char> charList = (IList<char>)new List<char>((IEnumerable<char>)new char[8]
            {
        ' ',
        '(',
        ')',
        '&',
        '+',
        '-',
        '*',
        '/'
            });
            int num1 = 0;
            Stack<int> intStack = new Stack<int>(16);
            foreach (char ch in criterionPart)
            {
                if (ch == '(')
                    intStack.Push(num1);
                if (ch == ')')
                {
                   // if (intStack.Count == 0)
                       // throw new CriterionException("Can't match \"(\" for expression.", criterionPart);
                    int length1 = intStack.Pop();
                    int num2 = num1;
                    string str1 = length1 > 0 ? criterionPart.Substring(0, length1) : string.Empty;
                    string criterionPart1 = criterionPart.Substring(length1 + 1, num2 - length1 - 1);
                    if (length1 == 0 || length1 > 0 && charList.Contains(criterionPart[length1 - 1]))
                    {
                        string str2 = this.ConvertExpression(criterionPart1, mapping, checkLogic);
                        criterionPart = string.Format("{0}{1}{2}", new object[3]
                        {
              (object) str1,
              (object) str2,
              (object) criterionPart.Substring(num2 + 1, criterionPart.Length - num2 - 1)
                        });
                        return this.ConvertFullExpression(criterionPart, mapping, checkLogic);
                    }
                    int num3 = str1.LastIndexOf('.');
                    int length2 = str1.LastIndexOfAny((charList as List<char>).ToArray());
                    string empty = string.Empty;
                    bool flag = num3 > 0 && num3 > length2;
                    if (flag)
                        length2 = num3;
                    string funName = length2 >= 0 ? (length2 != 0 ? (length1 > length2 ? str1.Substring(length2 + 1, length1 - length2 - 1) : string.Empty) : str1.Substring(1, str1.Length - 1)) : str1;
                    //if (!this.IsFunction(funName))
                    //    throw new CriterionException(string.Format("Invalid function name:{0}", (object)funName), criterionPart);
                    if (flag)
                    {
                        string str2 = str1.Substring(0, length2);
                        int num4 = str2.LastIndexOfAny((charList as List<char>).ToArray());
                        string str3 = num4 >= 0 ? str2.Substring(num4 + 1, str2.Length - num4 - 1) : str2;
                        string parameterList = string.IsNullOrEmpty(criterionPart1) ? str3 : string.Format("{0}, {1}", (object)str3, (object)criterionPart1);
                        criterionPart = string.Format("{0}{1}{2}", new object[3]
                        {
              num4 == 0 ? (object) string.Empty : (object) str2.Substring(0, num4 + 1),
              (object) this.ConvertFunction(funName, parameterList, mapping),
              (object) criterionPart.Substring(num2 + 1, criterionPart.Length - num2 - 1)
                        });
                    }
                    else
                    {
                        string parameterList = criterionPart1;
                        criterionPart = string.Format("{0}{1}{2}", new object[3]
                        {
              length2 < 0 ? (object) string.Empty : (length2 == 0 ? (object) str1.Substring(0, 1) : (object) str1.Substring(0, length2 + 1)),
              (object) this.ConvertFunction(funName, parameterList, mapping),
              (object) criterionPart.Substring(num2 + 1, criterionPart.Length - num2 - 1)
                        });
                    }
                    return this.ConvertFullExpression(criterionPart, mapping, checkLogic);
                }
                ++num1;
            }
            //if (intStack.Count > 0)
            //    throw new CriterionException("Can't match \")\" for expression.", criterionPart);
            criterionPart = this.ConvertExpression(criterionPart, mapping, checkLogic);
            return criterionPart;
        }

        protected virtual string ConvertExpression(
          string criterionPart,
          IMapping mapping,
          bool checkLogic)
        {
            if (checkLogic)
            {
                criterionPart = this.ConvertLogic(criterionPart, mapping).Trim();
                criterionPart = this.ConvertCondition(criterionPart, mapping).Trim();
            }
            criterionPart = this.ConvertOperations(criterionPart, mapping).Trim();
            return criterionPart;
        }

        protected virtual string ConvertFunction(
          string funName,
          string parameterList,
          IMapping mapping)
        {
            //if (string.IsNullOrEmpty(funName))
               // throw new ArgumentNullException("funname");
           // if (parameterList == null)
              //  throw new ArgumentNullException(nameof(parameterList));
            string[] parameters = parameterList.Split(new char[1]
            {
        ','
            }, StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < parameters.Length; ++index)
            {
                if (index == 0 && funName.Equals("IIF", StringComparison.OrdinalIgnoreCase))
                    parameters[index] = this.ConvertCondition(parameters[index], mapping);
                parameters[index] = this.ConvertOperations(parameters[index], mapping);
                parameters[index] = this.GetItemValue(parameters[index], mapping);
            }
            return this.preplacer.ReplaceValue(this.FunctionParser.GetResult(funName, parameters));
        }

        protected virtual string ConvertLogic(string criterionPart, IMapping mapping)
        {
            Regex regex = new Regex("And|Or|Xor", RegexOptions.IgnoreCase);
            MatchCollection matchCollection = regex.Matches(criterionPart);
            if (matchCollection.Count == 0)
                return this.ConvertCondition(criterionPart, mapping);
            bool flag1 = false;
            string[] strArray = regex.Split(criterionPart);
            for (int index = 0; index < matchCollection.Count; ++index)
            {
                bool flag2 = index == 0 ? this.ConvertCondition(strArray[0].Trim(), mapping) == "true" : flag1;
                bool flag3 = this.ConvertCondition(strArray[index + 1].Trim(), mapping) == "true";
                switch (matchCollection[index].Value.ToLower())
                {
                    case "and":
                        flag1 = flag2 && flag3;
                        break;
                    case "or":
                        flag1 = flag2 || flag3;
                        break;
                    case "xor":
                        flag1 = flag2 != flag3;
                        break;
                }
            }
            return !flag1 ? "false" : "true";
        }

        protected virtual string ConvertCondition(string criterionPart, IMapping mapping)
        {
            string leftPart;
            string rightPart;
            SignalType signalType;
            if (!this.ParseCondition(criterionPart, out leftPart, out rightPart, out signalType))
                return criterionPart;
            string itemValue1 = this.GetItemValue(leftPart, mapping);
            string itemValue2 = this.GetItemValue(rightPart, mapping);
            return !this.CompareValue(signalType, itemValue1, itemValue2) ? "false" : "true";
        }

        protected bool ParseCondition(
          string criterionPart,
          out string leftPart,
          out string rightPart,
          out SignalType signalType)
        {
           // if (string.IsNullOrEmpty(criterionPart))
               // throw new ArgumentNullException(nameof(criterionPart));
            leftPart = string.Empty;
            rightPart = string.Empty;
            signalType = SignalType.Unknown;
            string[] strArray1 = criterionPart.Split(new char[1]
            {
        ' '
            }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray1.Length == 3)
            {
                leftPart = strArray1[0].Trim();
                signalType = this.GetSignalType(strArray1[1].Trim());
              //  if (signalType == SignalType.Unknown)
                  //  throw new CriterionException(string.Format("Invalid sygnal:{0} in expression.", (object)strArray1[1].Trim()), criterionPart);
                rightPart = strArray1[2].Trim();
                return true;
            }
            for (int index = 0; index < CriterionParser.SignalList.Length; ++index)
            {
                if (criterionPart.Contains(CriterionParser.SignalList[index]))
                {
                    string[] strArray2 = criterionPart.Split(new string[1]
                    {
            CriterionParser.SignalList[index]
                    }, StringSplitOptions.RemoveEmptyEntries);
                    if (strArray2.Length < 2)
                        return false;
                    leftPart = strArray2[0];
                    rightPart = strArray2[1];
                    signalType = this.GetSignalType(CriterionParser.SignalList[index]);
                    //if (signalType == SignalType.Unknown)
                        //throw new CriterionException(string.Format("Invalid sygnal:{0} in expression.", (object)strArray2[1].Trim()), criterionPart);
                    return true;
                }
            }
            return false;
        }

        private bool CompareValue(SignalType signalType, string leftValue, string rightValue)
        {
            bool flag1 = this.IsEmpty(leftValue);
            bool flag2 = this.IsEmpty(rightValue);
            if (flag1 || flag2)
                return flag1 && flag2 ? signalType == SignalType.Equal : signalType == SignalType.NotEqual;
            Enums.ValueType valueType1 = this.GetValueType(leftValue);
            Enums.ValueType valueType2 = this.GetValueType(rightValue);
            Enums.ValueType valueType3;
            if (valueType1 != valueType2)
            {
                if (valueType1 == Enums.ValueType.Numeric && valueType2 == Enums.ValueType.String)
                {
                    valueType3 = Enums.ValueType.Numeric;
                    rightValue = rightValue.Substring(1, rightValue.Length - 2);
                }
                else if (valueType1 == Enums.ValueType.DateTime && valueType2 == Enums.ValueType.String)
                {
                    valueType3 = Enums.ValueType.DateTime;
                    rightValue = string.Format("#{0}#", (object)rightValue.Substring(1, rightValue.Length - 2));
                }
                else
                {
                    //if (valueType1 != Enums.ValueType.String || valueType2 == Enums.ValueType.String)
                        //throw new CriterionException(string.Format("Can't compare value between different type:{0} and {1}.", (object)leftValue, (object)rightValue));
                    if (valueType2 == Enums.ValueType.Numeric)
                        rightValue = string.Format("\"{0}\"", (object)rightValue);
                    else if (valueType2 == Enums.ValueType.Numeric)
                        rightValue = string.Format("\"{0}\"", (object)rightValue.Substring(1, rightValue.Length - 2));
                    else if (valueType2 == Enums.ValueType.Unknown)
                        rightValue = string.Format("\"{0}\"", (object)rightValue);
                    valueType3 = Enums.ValueType.String;
                }
            }
            switch (valueType1)
            {
                case Enums.ValueType.Numeric:
                    Decimal result1;
                    Decimal.TryParse(leftValue, out result1);
                       
                    Decimal result2;
                    Decimal.TryParse(rightValue, out result2);
                    switch (signalType)
                    {
                        case SignalType.Equal:
                            return result1 == result2;
                        case SignalType.NotEqual:
                            return result1 != result2;
                        case SignalType.GreaterThanOrEqual:
                            return result1 >= result2;
                        case SignalType.LessThanOrEqual:
                            return result1 <= result2;
                        case SignalType.GreaterThan:
                            return result1 > result2;
                        case SignalType.LessThan:
                            return result1 < result2;
                    }
                    break;
                case Enums.ValueType.String:
                    leftValue = leftValue.ToLower();
                    rightValue = rightValue.ToLower();
                    switch (signalType)
                    {
                        case SignalType.Equal:
                            return leftValue == rightValue;
                        case SignalType.NotEqual:
                            return leftValue != rightValue;
                        case SignalType.GreaterThanOrEqual:
                        case SignalType.LessThanOrEqual:
                        case SignalType.GreaterThan:
                        case SignalType.LessThan:
                            if (leftValue.StartsWith("\"") && leftValue.EndsWith("\"") && leftValue.Length > 2)
                                leftValue = leftValue.Substring(1, leftValue.Length - 2);
                            if (rightValue.StartsWith("\"") && rightValue.EndsWith("\"") && rightValue.Length > 2)
                                rightValue = rightValue.Substring(1, rightValue.Length - 2);
                            Decimal result3;
                            if (!Decimal.TryParse(leftValue, out result3))
                                result3 = new Decimal(0);
                            Decimal result4;
                            if (!Decimal.TryParse(rightValue, out result4))
                                result4 = new Decimal(0);
                            switch (signalType)
                            {
                                case SignalType.GreaterThanOrEqual:
                                    if (result3 >= result4)
                                        return true;
                                    break;
                                case SignalType.LessThanOrEqual:
                                    if (result3 <= result4)
                                        return true;
                                    break;
                                case SignalType.GreaterThan:
                                    if (result3 > result4)
                                        return true;
                                    break;
                                case SignalType.LessThan:
                                    if (result3 < result4)
                                        return true;
                                    break;
                            }
                            return false;
                        case SignalType.Contains:
                            return leftValue.IndexOf(rightValue) > 0;
                        case SignalType.StartsWith:
                            return leftValue.StartsWith(rightValue);
                        case SignalType.EndsWith:
                            return leftValue.EndsWith(rightValue);
                    }
                    break;
                case Enums.ValueType.DateTime:
                    DateTime result5;
                    if (leftValue == "##" || leftValue == "#//#")
                        result5 = DateTime.MinValue;
                    else if (!DateTime.TryParse(leftValue, out result5))
                        throw new ArgumentException("The left value is invalid date time");
                    DateTime result6;
                    if (rightValue == "##" || rightValue == "#//#")
                        result6 = DateTime.MinValue;
                    else if (!DateTime.TryParse(rightValue, out result6))
                        throw new ArgumentException("The left value is invalid date time");
                    switch (signalType)
                    {
                        case SignalType.Equal:
                            return result5 == result6;
                        case SignalType.NotEqual:
                            return result5 != result6;
                        case SignalType.GreaterThanOrEqual:
                            return result5 >= result6;
                        case SignalType.LessThanOrEqual:
                            return result5 <= result6;
                        case SignalType.GreaterThan:
                            return result5 > result6;
                        case SignalType.LessThan:
                            return result5 < result6;
                    }
                    break;
            }
            return false;
        }

        protected virtual string ConvertOperations(string criterionPart, IMapping mapping)
        {
            if (criterionPart == null)
                throw new ArgumentNullException(nameof(criterionPart));
            if (criterionPart == string.Empty)
                return string.Empty;
            bool flag = false;
            string str1 = string.Empty;
            char[] chArray = new char[5]
            {
        '+',
        '-',
        '*',
        '/',
        '&'
            };
            string str2 = criterionPart.Replace(" ", string.Empty);
            string empty = string.Empty;
            List<string> stringList1 = new List<string>();
            List<string> stringList2 = new List<string>();
            for (int index = 0; index < str2.Length; ++index)
            {
                char ch = str2[index];
                if (((IEnumerable<char>)chArray).Contains<char>(ch))
                {
                    if (ch == '-' && (index == 0 || ((IEnumerable<char>)chArray).Contains<char>(str2[index - 1])))
                    {
                        empty += (string)(object)ch;
                    }
                    else
                    {
                        stringList1.Add(empty);
                        stringList2.Add(ch.ToString());
                        empty = string.Empty;
                    }
                }
                else
                {
                    empty += ch.ToString();
                    if (index == str2.Length - 1)
                    {
                        stringList1.Add(empty);
                        empty = string.Empty;
                    }
                }
            }
            if (!flag && stringList2.Count > 0)
                flag = true;
            if (stringList1.Count != stringList2.Count + 1)
                throw new Exception("Invalid operation");
            for (int index = 0; index < stringList2.Count; ++index)
            {
                string itemPart1 = index == 0 ? stringList1[index] : str1;
                string itemPart2 = stringList1[index + 1];
                switch (stringList2[index])
                {
                    case "+":
                        str1 = string.Format("{0}", (object)(this.GetDecimalValue(itemPart1, mapping) + this.GetDecimalValue(itemPart2, mapping)));
                        break;
                    case "-":
                        str1 = string.Format("{0}", (object)(this.GetDecimalValue(itemPart1, mapping) - this.GetDecimalValue(itemPart2, mapping)));
                        break;
                    case "*":
                        str1 = string.Format("{0}", (object)(this.GetDecimalValue(itemPart1, mapping) * this.GetDecimalValue(itemPart2, mapping)));
                        break;
                    case "/":
                        str1 = string.Format("{0}", (object)(this.GetDecimalValue(itemPart1, mapping) / this.GetDecimalValue(itemPart2, mapping)));
                        break;
                    case "&":
                        str1 = this.preplacer.ReplaceValue(string.Format("\"{0}{1}\"", (object)this.GetStringValue(itemPart1, mapping), (object)this.GetStringValue(itemPart2, mapping)));
                        break;
                }
            }
            return !flag ? criterionPart : str1;
        }

        protected double GetDoubleValue(string itemPart, IMapping mapping)
        {
            double result;
            return double.TryParse(this.GetItemValue(itemPart, mapping), out result) ? result : 0.0;
        }

        protected Decimal GetDecimalValue(string itemPart, IMapping mapping)
        {
            Decimal result;
            return Decimal.TryParse(this.GetItemValue(itemPart, mapping), out result) ? result : new Decimal(0);
        }

        protected virtual string ConvertValues(string criterionPart, IMapping mapping)
        {
            if (criterionPart == null)
                throw new ArgumentNullException(nameof(criterionPart));
            if (criterionPart == string.Empty)
                return string.Empty;
            int startIndex1 = 0;
            Stack<int> intStack = new Stack<int>(16);
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < criterionPart.Length; ++index)
            {
                if (criterionPart[index] == '@')
                {
                    if (intStack.Count == 0)
                    {
                        intStack.Push(index);
                        stringBuilder.Append(criterionPart.Substring(startIndex1, index - startIndex1));
                        startIndex1 = index;
                    }
                    else
                    {
                        int startIndex2 = intStack.Pop();
                        int num = index;
                        startIndex1 = num;
                        if (startIndex1 < criterionPart.Length)
                            ++startIndex1;
                        string itemValue = this.GetItemValue(criterionPart.Substring(startIndex2, num - startIndex2 + 1), mapping);
                        stringBuilder.Append(itemValue);
                    }
                }
            }
            stringBuilder.Append(criterionPart.Substring(startIndex1));
            string str = stringBuilder.ToString();
            if (str.StartsWith("\"") && !str.EndsWith("\"") || !str.StartsWith("\"") && str.EndsWith("\""))
                throw new CriterionException(string.Format("Invalid string result:{0}", (object)criterionPart), criterionPart);
            if (str.StartsWith("#") && !str.EndsWith("#") || !str.StartsWith("#") && str.EndsWith("#"))
                throw new CriterionException(string.Format("Invalid date time result:{0}", (object)criterionPart), criterionPart);
            return str;
        }

        protected string RemoveQuotes(string criterionPart)
        {
            return criterionPart.StartsWith("\"") && criterionPart.EndsWith("\"") && criterionPart.Length >= 2 ? criterionPart.Substring(1, criterionPart.Length - 2) : criterionPart;
        }

        protected string RemoveParentheses(string criterionPart)
        {
            if (criterionPart.StartsWith("(") && criterionPart.EndsWith(")"))
            {
                foreach (int num in criterionPart)
                    ;
            }
            return criterionPart;
        }

        private bool IsFunction(string funName)
        {
            if (string.IsNullOrEmpty(funName))
                throw new ArgumentNullException(nameof(funName));
            for (int index = 0; index < funName.Length; ++index)
            {
                if (!char.IsLetter(funName[index]) && !char.IsDigit(funName[index]) && (funName[index] != '_' && funName[index] != ':'))
                    return false;
            }
            return true;
        }

        private bool IsBoolean(string criterionPart)
        {
            if (string.IsNullOrEmpty(criterionPart))
                throw new ArgumentNullException(nameof(criterionPart));
            string[] strArray = new Regex("And|Or|Xor", RegexOptions.IgnoreCase).Split(criterionPart);
            if (strArray.Length == 0)
                return criterionPart.Trim().ToLower() == "true" || criterionPart.Trim().ToLower() == "false";
            for (int index = 0; index < strArray.Length; ++index)
            {
                strArray[index] = strArray[index].Trim().ToLower();
                if (strArray[index] != "true" && strArray[index] != "false")
                    return false;
            }
            return true;
        }

        private bool IsEmpty(string criterionPart)
        {
            return string.IsNullOrEmpty(criterionPart) || criterionPart == "\"\"" || (criterionPart == "//" || criterionPart == "##") || (criterionPart == "#//#" || criterionPart == string.Format("#{0}#", (object)CriterionParser.EmptyDate) || criterionPart.ToLower() == "blank");
        }

        protected virtual string ProcessCurrentDateTime(string criterionString)
        {
            if (criterionString == null)
                throw new ArgumentNullException(nameof(criterionString));
            if (criterionString == string.Empty)
                return string.Empty;
            DateTime currentTime = this.GetCurrentTime();
            criterionString = criterionString.Replace("$today", string.Format("#{0}#", (object)currentTime.Date.ToString("MM/dd/yyyy")));
            criterionString = criterionString.Replace("$now", string.Format("#{0}#", (object)currentTime.ToString()));
            return criterionString;
        }

        protected virtual DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        protected string GetItemValue(string itemPart, IMapping mapping)
        {
            return this.GetItemValue(itemPart, true, mapping);
        }

        protected virtual string GetItemValue(string itemPart, bool quote, IMapping mapping)
        {
            if (itemPart == null)
                throw new ArgumentNullException(nameof(itemPart));
            if (string.IsNullOrEmpty(itemPart.Trim()))
                return quote ? string.Format("\"{0}\"", (object)itemPart.Trim()) : itemPart;
            itemPart = string.Format("{0}", (object)itemPart).Trim();
            if (this.preplacer.IsString(itemPart))
                return quote ? string.Format("\"{0}\"", (object)this.preplacer.GetString(itemPart)) : this.preplacer.GetString(itemPart);
            if (this.preplacer.IsDateTime(itemPart))
                return quote ? string.Format("#{0}#", (object)this.preplacer.GetDateTime(itemPart)) : this.preplacer.GetDateTime(itemPart);
            if (this.preplacer.IsFieldID(itemPart))
                return this.GetFieldValue(this.preplacer.GetFieldId(itemPart), mapping);
            itemPart = this.ConvertOperations(itemPart, mapping);
            if (itemPart.Contains(" "))
            {
                string[] strArray = itemPart.Split(' ');
                for (int index = 0; index < strArray.Length; ++index)
                {
                    if (this.preplacer.IsFieldID(strArray[index]))
                        strArray[index] = string.Format("[{0}]", (object)this.preplacer.GetFieldId(strArray[index]));
                    if (this.preplacer.IsString(strArray[index]))
                        strArray[index] = string.Format("\"{0}\"", (object)this.preplacer.GetString(strArray[index]));
                    if (this.preplacer.IsDateTime(strArray[index]))
                        strArray[index] = string.Format("#{0}#", (object)this.preplacer.GetDateTime(strArray[index]));
                }
                //throw new CriterionException(string.Format("Invalid expression of item:{0}", (object)string.Join(" ", strArray)), itemPart);
            }
            return itemPart;
        }

        protected virtual string GetFieldValue(string fieldID, IMapping mapping)
        {
            throw new NotSupportedException("Invalid method of GetFieldValue in super class.");
        }

        protected virtual string GetStringValue(string itemPart, IMapping mapping)
        {
            itemPart = this.GetItemValue(itemPart, mapping);
            itemPart = this.RemoveQuotes(itemPart);
            return itemPart;
        }

        protected SignalType GetSignalType(string signal)
        {
            switch (signal)
            {
                case "==":
                case "=":
                    return SignalType.Equal;
                case "<>":
                case "!=":
                    return SignalType.NotEqual;
                case ">=":
                    return SignalType.GreaterThanOrEqual;
                case "<=":
                    return SignalType.LessThanOrEqual;
                case ">":
                    return SignalType.GreaterThan;
                case "<":
                    return SignalType.LessThan;
                case "*?*":
                    return SignalType.Contains;
                case "**?":
                    return SignalType.StartsWith;
                case "?**":
                    return SignalType.EndsWith;
                default:
                    return SignalType.Unknown;
            }
        }

        protected Enums.ValueType GetValueType(string itemValue)
        {
            if (itemValue.StartsWith("\"") && itemValue.EndsWith("\""))
                return Enums.ValueType.String;
            if (itemValue.StartsWith("#") && itemValue.EndsWith("#"))
            {
                if (itemValue == "##" || DateTime.TryParse(itemValue.Substring(1, itemValue.Length - 2), out DateTime _))
                    return Enums.ValueType.DateTime;
            }
            else if (Decimal.TryParse(itemValue, out Decimal _))
                return Enums.ValueType.Numeric;
            return Enums.ValueType.Unknown;
        }

        public class Replacer
        {
            private const string SymbolOfToday = "$today";
            private const string SymbolOfNow = "$now";
            private const string SymbolOfBlank = "$blank";
            private const string SymbolOfBlank2 = "$Blank";
            private const string SymbolOfSpace = "$space";
            private const string SymbolOfEmptyDate = "$empty_date";
            private int _cacheIndex;
            private IDictionary<string, string> _stringCache;
            private IDictionary<string, string> _dateTimeCache;
            private IDictionary<string, string> _fieldCache;

            internal Replacer()
            {
                this._cacheIndex = 0;
                this._stringCache = (IDictionary<string, string>)new Dictionary<string, string>(32);
                this._dateTimeCache = (IDictionary<string, string>)new Dictionary<string, string>(32);
                this._fieldCache = (IDictionary<string, string>)new Dictionary<string, string>(128);
            }

            public bool IsString(string mask)
            {
                if (mask == null)
                    throw new ArgumentNullException(nameof(mask));
                return !(mask == string.Empty) && this._stringCache.ContainsKey(mask);
            }

            public bool IsDateTime(string mask)
            {
                if (mask == null)
                    throw new ArgumentNullException(nameof(mask));
                return !(mask == string.Empty) && this._dateTimeCache.ContainsKey(mask);
            }

            public bool IsFieldID(string mask)
            {
                if (mask == null)
                    throw new ArgumentNullException(nameof(mask));
                return !(mask == string.Empty) && this._fieldCache.ContainsKey(mask);
            }

            public IList<string> GetAllFields()
            {
                return (IList<string>)new List<string>((IEnumerable<string>)this._fieldCache.Values);
            }

            public string GetString(string mask)
            {
                if (mask == null)
                    throw new ArgumentNullException(nameof(mask));
                if (mask == string.Empty)
                    return string.Empty;
                if (!this._stringCache.ContainsKey(mask))
                    throw new ArgumentException(string.Format("Can't find mask:{0} in cache.", (object)mask));
                return string.Format("{0}", (object)this._stringCache[mask]);
            }

            public string GetDateTime(string mask)
            {
                if (mask == null)
                    throw new ArgumentNullException(nameof(mask));
                if (mask == string.Empty)
                    return string.Empty;
                if (!this._dateTimeCache.ContainsKey(mask))
                    throw new ArgumentException(string.Format("Can't find mask:{0} in cache.", (object)mask));
                return string.Format("{0}", (object)this._dateTimeCache[mask]);
            }

            public string GetFieldId(string mask)
            {
                if (string.IsNullOrEmpty(mask))
                    throw new ArgumentNullException(nameof(mask));
                if (!this._fieldCache.ContainsKey(mask))
                    throw new ArgumentException(string.Format("Can't find mask:{0} in cache.", (object)mask));
                return this._fieldCache[mask];
            }

            public string ReplaceValue(string criterionString)
            {
                if (criterionString == null)
                    throw new ArgumentNullException(nameof(criterionString));
                if (criterionString == string.Empty)
                    return string.Empty;
                criterionString = this.ReplaceConstValue(criterionString);
                criterionString = this.ReplaceStringValue(criterionString);
                criterionString = this.ReplaceDateTimeValue(criterionString);
                criterionString = this.ReplaceFieldID(criterionString);
                return criterionString;
            }

            private string ReplaceStringValue(string criterionString)
            {
                if (string.IsNullOrEmpty(criterionString))
                    throw new ArgumentNullException(nameof(criterionString));
                StringBuilder stringBuilder = new StringBuilder();
                int num1 = -1;
                int num2 = -1;
                for (int index = 0; index < criterionString.Length; ++index)
                {
                    if (criterionString[index] == '"')
                    {
                        if (num1 < 0)
                        {
                            num1 = index;
                            continue;
                        }
                        if (num1 >= 0)
                        {
                            if (criterionString[index - 1] != '\\')
                                num2 = index;
                            else
                                continue;
                        }
                        if (num2 > 0)
                        {
                            string key = string.Format("@STRING{0}@", (object)++this._cacheIndex);
                            string str = num1 + 1 != num2 ? criterionString.Substring(num1 + 1, num2 - num1 - 1) : string.Empty;
                            this._stringCache.Add(key, str);
                            stringBuilder.Append(string.Format("{0}", (object)key));
                            num1 = -1;
                            num2 = -1;
                            continue;
                        }
                    }
                    if (num1 < 0)
                        stringBuilder.Append(criterionString[index]);
                }
                return stringBuilder.ToString();
            }

            private string ReplaceDateTimeValue(string criterionString)
            {
                if (string.IsNullOrEmpty(criterionString))
                    throw new ArgumentNullException(nameof(criterionString));
                bool flag = false;
                StringBuilder stringBuilder = new StringBuilder();
                int num1 = -1;
                int num2 = -1;
                for (int index = 0; index < criterionString.Length; ++index)
                {
                    if (criterionString[index] == '[')
                        flag = true;
                    if (criterionString[index] == ']' && flag)
                        flag = false;
                    if (criterionString[index] == '#' && !flag)
                    {
                        if (num1 < 0)
                        {
                            num1 = index;
                            continue;
                        }
                        if (num1 >= 0)
                            num2 = index;
                        if (num2 > 0)
                        {
                            string key = string.Format("@DATE{0}@", (object)++this._cacheIndex);
                            string str = num1 + 1 != num2 ? criterionString.Substring(num1 + 1, num2 - num1 - 1) : string.Empty;
                            this._dateTimeCache.Add(key, str);
                            stringBuilder.Append(string.Format("{0}", (object)key));
                            num1 = -1;
                            num2 = -1;
                            continue;
                        }
                    }
                    if (num1 < 0)
                        stringBuilder.Append(criterionString[index]);
                }
                return stringBuilder.ToString();
            }

            private string ReplaceFieldID(string criterionString)
            {
                if (string.IsNullOrEmpty(criterionString))
                    throw new ArgumentNullException(nameof(criterionString));
                if (criterionString.IndexOf('[') < 0 && criterionString.IndexOf(']') < 0)
                    return criterionString;
                StringBuilder stringBuilder = new StringBuilder();
                int num1 = -1;
                int num2 = -1;
                for (int index = 0; index < criterionString.Length; ++index)
                {
                    if (criterionString[index] == '[' && num1 < 0)
                        num1 = index;
                    else if (criterionString[index] == ']')
                    {
                        int num3 = index;
                        string str = num1 + 1 != num3 ? criterionString.Substring(num1 + 1, num3 - num1 - 1) : string.Empty;
                        string key = string.Format("@FIELD_{0}@", (object)++this._cacheIndex);
                        if (!this._fieldCache.ContainsKey(key))
                            this._fieldCache.Add(key, str);
                        else
                            this._fieldCache[key] = str;
                        stringBuilder.Append(key);
                        num1 = -1;
                        num2 = -1;
                    }
                    else if (num1 < 0)
                        stringBuilder.Append(criterionString[index]);
                }
                return stringBuilder.ToString();
            }

            private string ReplaceConstValue(string criterionString)
            {
                if (string.IsNullOrEmpty(criterionString))
                    throw new ArgumentNullException(nameof(criterionString));
                if (criterionString.Contains("$blank"))
                    criterionString = criterionString.Replace("$blank", "\"\"");
                if (criterionString.Contains("$Blank"))
                    criterionString = criterionString.Replace("$Blank", "\"\"");
                if (criterionString.Contains("$space"))
                    criterionString = criterionString.Replace("$space", "\" \"");
                if (criterionString.Contains("$today"))
                    criterionString = criterionString.Replace("$today", string.Format("#{0}#", (object)DateTime.Today.ToString("MM/dd/yyyy")));
                if (criterionString.Contains("$now"))
                    criterionString = criterionString.Replace("$now", string.Format("#{0}#", (object)DateTime.Now.ToString()));
                if (criterionString.Contains("$empty_date"))
                    criterionString = criterionString.Replace("$empty_date", string.Format("#{0}#", (object)CriterionParser.EmptyDate));
                return criterionString;
            }

            public string[] GetFields()
            {
                return this._fieldCache.Values.ToArray<string>();
            }

            public void Reset()
            {
                this._cacheIndex = 0;
                this._stringCache.Clear();
                this._dateTimeCache.Clear();
                this._fieldCache.Clear();
            }
        }
    }
}
