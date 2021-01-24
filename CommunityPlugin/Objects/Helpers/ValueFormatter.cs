using CommunityPlugin.Objects.Interface;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CommunityPlugin.Objects.Helpers
{
    public class ValueFormatter : IValueFormatter
    {
        protected virtual string EmptyDateTime
        {
            get
            {
                string appSetting = GlobalConfiguration.AppSettings["ValueOfEmptyDate"];
                return string.IsNullOrEmpty(appSetting) ? string.Empty : appSetting;
            }
        }

        protected ValueFormatter()
        {
        }

        public static string FormatValue(string result, string formatting)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            return string.IsNullOrEmpty(formatting) ? result : new ValueFormatter().FormatStandardValue(result, formatting);
        }

        public virtual string Format(string result, string formatting)
        {
            return this.FormatStandardValue(result, formatting);
        }

        protected virtual string FormatStandardValue(string result, string formatting)
        {
            if (string.IsNullOrWhiteSpace(formatting))
                return result;
            int num1 = formatting.IndexOf("(");
            int num2 = formatting.LastIndexOf(")");
            string str1 = formatting.Substring(0, 1).ToLower();
            string mask = formatting.Substring(num1 + 1, num2 - num1 - 1);
            if (str1.Equals("s"))
            {
                string str2 = GlobalConfiguration.AppSettings["ReplaceParttern"];
                if (string.IsNullOrWhiteSpace(str2))
                    str2 = ",!@#$%^&*():;'\"<>/?|\\\\";
                result = Regex.Replace(result, string.Format("[{0}]", (object)str2), string.Empty);
                str1 = "c";
            }
            bool flag1 = false;
            bool flag2 = false;
            if (str1 == "u" || str1 == "l")
            {
                flag1 = str1 == "u";
                flag2 = str1 == "l";
                str1 = "c";
            }
            if (mask == "*")
            {
                if (flag1)
                    result = result.ToUpper();
                else if (flag2)
                    result = result.ToLower();
                return result;
            }
            switch (str1)
            {
                case "c":
                    string lower1 = mask.ToLower();
                    if (lower1.IndexOf(":") > 0)
                    {
                        result = this.FormatSatndardDigital(result, false, lower1);
                        break;
                    }
                    int result1;
                    if (!int.TryParse(lower1, out result1) || result1 < 0)
                        throw new Exception(string.Format("Invalid length:{0} of chars.", (object)lower1));
                    result = result.Length <= result1 ? result.PadRight(result1, ' ') : result.Substring(0, result1);
                    break;
                case "n":
                    string lower2 = mask.ToLower();
                    if (lower2.IndexOf(":") > 0)
                    {
                        result = this.FormatSatndardDigital(result, true, lower2);
                        break;
                    }
                    int l2 = -1;
                    bool decimalPoint = true;
                    bool leadingZero = lower2.StartsWith("0");
                    int l1;
                    if (lower2.IndexOf("(v)") >= 0 || lower2.IndexOf("v") >= 0)
                    {
                        string[] strArray;
                        if (lower2.IndexOf("(v)") >= 0)
                        {
                            decimalPoint = false;
                            strArray = lower2.Split(new string[1] { "(v)" }, StringSplitOptions.RemoveEmptyEntries);
                        }
                        else
                            strArray = lower2.Split('v');
                        if (strArray.Length != 2)
                            throw new Exception(string.Format("Invalid format:{0}", (object)lower2));
                        l1 = !(strArray[0] == "*") ? int.Parse(strArray[0]) : -1;
                        l2 = int.Parse(strArray[1]);
                    }
                    else
                        l1 = int.Parse(lower2);
                    result = this.FormatStandardDecimal(result, l1, l2, leadingZero, decimalPoint);
                    break;
                case "m":
                    result = this.FormatStandardValue(result, string.Format("N({0})", (object)mask));
                    result = this.FormatMoney(result.Trim());
                    break;
                case "d":
                    result = this.FormatSatndardDateTime(result, mask);
                    break;
            }
            if (flag1)
                result = result.ToUpper();
            else if (flag2)
                result = result.ToLower();
            return result;
        }

        protected virtual string FormatStandardDecimal(
          string result,
          int l1,
          int l2,
          bool leadingZero,
          bool decimalPoint)
        {
            if (string.IsNullOrEmpty(result))
            {
                if (!leadingZero)
                    return string.Empty;
                result = "0";
            }
            int num = l2 >= 0 ? l1 + l2 + 1 : l1;
            string format = l2 <= 0 ? "0" : string.Format("0.{0}", (object)new string('0', l2));
            result = Decimal.Parse(result).ToString(format);
            if (l1 == -1)
                return result;
            if (!decimalPoint)
            {
                result = result.Replace(".", string.Empty);
                if (l2 >= 0)
                    --num;
            }
            if (result.Length > num)
                result = result.Substring(0, num);
            else if (result.Length < num)
                result = result.PadLeft(num, leadingZero ? '0' : ' ');
            return result;
        }

        protected virtual string FormatSatndardDigital(string result, bool onlyNumber, string mask)
        {
            int length = mask.IndexOf(":");
            if (length < 0)
                throw new Exception(string.Format("Invalid format:{0} for digital.", (object)mask));
            int startIndex = length + 1;
            if (mask.IndexOf("::") > 0)
                startIndex = length + 1;
            string str1 = mask.Substring(0, length);
            string str2 = mask.Substring(startIndex);
            int num1 = str1.IndexOf("(");
            int num2 = str1.LastIndexOf(")");
            bool leadingZero = str1.StartsWith("0");
            int num3 = int.Parse(str1.Substring(0, num1 >= 0 || num2 >= 0 ? num1 : str1.Length - 1));
            if (num1 < 0 && num2 < 0)
                num1 = str1.Length - 2;
            char c = str1.Substring(num1 + 1, 1).ToCharArray()[0];
            result = result.Replace(new string(c, 1), "");
            if (num1 > 0 && num2 > 0)
            {
                if (onlyNumber)
                    return this.FormatStandardDecimal(result, num3, -1, leadingZero, false);
                result = result.Length <= num3 ? result.PadRight(num3, ' ') : result.Substring(0, num3);
                return result;
            }
            string[] strArray = str2.Split(c);
            int[] numArray = new int[strArray.Length - 1];
            for (int index = 0; index < strArray.Length - 1; ++index)
            {
                numArray[index] = int.Parse(strArray[index]);
                if (index > 0)
                    numArray[index] += numArray[index - 1];
            }
            char[] charArray = result.ToCharArray();
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < charArray.Length; ++index)
            {
                if (onlyNumber && !char.IsNumber(charArray[index]))
                    throw new Exception("Invalid digital number");
                if (index < num3)
                    stringBuilder.Append(charArray[index]);
                else
                    break;
            }
            for (int index = 0; index < numArray.Length; ++index)
                stringBuilder.Insert(numArray[index] + index, c);
            return stringBuilder.ToString();
        }

        protected virtual string FormatSatndardDateTime(string result, string mask)
        {
            if (string.IsNullOrEmpty(result) || result == "//")
                return this.EmptyDateTime;
            DateTime result1;
            if (!DateTime.TryParse(result, out result1))
                throw new Exception(string.Format("Invalid date value:{0}", (object)result));
            if (mask.StartsWith("CYY"))
            {
                mask = mask.Substring(1, mask.Length - 1);
                return string.Format("{0}{1}", result1.Year >= 2000 ? (object)"1" : (object)"0", (object)this.FormatSatndardDateTime(result, mask));
            }
            mask = mask.Replace("D", "d");
            mask = mask.Replace("Y", "y");
            mask = mask.Replace("C", "y");
            mask = mask.Replace("c", "y");
            return result1.ToString(mask);
        }

        protected virtual string FormatMoney(string result)
        {
            string[] strArray = result.Split('.');
            string str1 = strArray[0];
            string str2 = strArray.Length > 1 ? strArray[1] : string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < str1.Length; ++index)
            {
                char ch = str1[str1.Length - index - 1];
                if (index > 0 && index % 3 == 0)
                    stringBuilder.Insert(0, ',');
                stringBuilder.Insert(0, ch);
            }
            string str3 = stringBuilder.ToString();
            result = !string.IsNullOrEmpty(str2) ? string.Format("{0}.{1}", (object)str3, (object)str2) : str3;
            return result;
        }
    }
}
