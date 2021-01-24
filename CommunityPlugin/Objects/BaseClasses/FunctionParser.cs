
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using EllieMae.Encompass.Configuration;
using EllieMae.Encompass.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunityPlugin.Objects.BaseClasses
{
    public abstract class FunctionParser : IFunctionParser
    {
        private const string TrueValue = "true";
        private const string FalseValue = "false";

        public static FunctionParser Default
        {
            get
            {
                return (FunctionParser)new FunctionParser.DefaultFunctionParser();
            }
        }

        public Func<string, string[], string> CustomFunctions { get; set; }

        internal FunctionParser()
        {
        }

        public virtual string GetResult(string funName, string[] parameters)
        {
            switch (funName.ToLower())
            {
                case "iif":
                    return this.IIf(parameters);
                case "format":
                    return this.Format(parameters);
                case "cstr":
                    return this.ReturnString(this.CStr(parameters));
                case "cnum":
                    return this.CNum(parameters);
                case "cdate":
                    return this.CDate(parameters);
                case "left":
                    return this.ReturnString(this.Left(parameters));
                case "padleft":
                    return this.ReturnString(this.PadLeft(parameters));
                case "right":
                    return this.ReturnString(this.Right(parameters));
                case "padright":
                    return this.ReturnString(this.PadRight(parameters));
                case "replace":
                    return this.ReturnString(this.Replace(parameters));
                case "rtrim":
                    return this.ReturnString(this.RTrim(parameters));
                case "ltrim":
                    return this.ReturnString(this.LTrim(parameters));
                case "trim":
                    return this.ReturnString(this.Trim(parameters));
                case "indexof":
                    return this.IndexOf(parameters);
                case "lastindexof":
                    return this.LastIndexOf(parameters);
                case "substring":
                    return this.ReturnString(this.SubString(parameters));
                case "len":
                case "length":
                    return this.Length(parameters);
                case "contains":
                    return this.Contains(parameters);
                case "notcontains":
                    return this.NotContains(parameters);
                case "startswith":
                    return this.StartsWith(parameters);
                case "endswith":
                    return this.EndsWith(parameters);
                case "sum":
                    return this.Sum(parameters);
                case "median":
                    return this.Median(parameters);
                case "avg":
                    return this.Avg(parameters);
                case "max":
                    return this.Max(parameters);
                case "maxdate":
                    return this.ReturnDateTime(this.MaxDate(parameters));
                case "min":
                    return this.Min(parameters);
                case "mindate":
                    return this.ReturnDateTime(this.MinDate(parameters));
                case "mid":
                    return this.Mid(parameters);
                case "array_sum":
                    return this.Array_Sum(parameters);
                case "array_median":
                    return this.Array_Median(parameters);
                case "array_avg":
                    return this.Array_Avg(parameters);
                case "array_max":
                    return this.Array_Max(parameters);
                case "array_maxdate":
                    return this.ReturnDateTime(this.Array_MaxDate(parameters));
                case "array_min":
                    return this.Array_Min(parameters);
                case "array_mindate":
                    return this.ReturnDateTime(this.Array_MinDate(parameters));
                case "array_mid":
                    return this.Array_Mid(parameters);
                case "first":
                case "frist":
                    return this.First(parameters);
                case "last":
                    return this.Last(parameters);
                case "join":
                    return this.ReturnString(this.Join(parameters));
                case "array_first":
                case "array_frist":
                    return this.Array_First(parameters);
                case "array_firststring":
                case "array_friststring":
                    return this.ReturnString(this.Array_First(parameters));
                case "array_last":
                    return this.Array_Last(parameters);
                case "array_laststring":
                    return this.ReturnString(this.Array_Last(parameters));
                case "array_join":
                    return this.ReturnString(this.Array_Join(parameters));
                case "oneisvalue":
                    return this.OneIsValue(parameters);
                case "manyisvalue":
                    return this.ManyIsValue(parameters);
                case "array_oiv":
                case "array_oneisvalue":
                    return this.Array_OneIsValue(parameters);
                case "array_miv":
                case "array_manyisvalue":
                    return this.Array_ManyIsValue(parameters);
                case "year":
                    return this.Year(parameters);
                case "month":
                    return this.Month(parameters);
                case "day":
                    return this.Day(parameters);
                case "dayofyear":
                    return this.DayOfYear(parameters);
                case "dayofweek":
                    return this.DayOfWeek(parameters);
                case "hour":
                    return this.Hour(parameters);
                case "minute":
                    return this.Minute(parameters);
                case "second":
                    return this.Second(parameters);
                case "millisecond":
                    return this.Millisecond(parameters);
                case "firstdayofmonth":
                    return this.FirstDayOfMonth(parameters);
                case "lastdayofmonth":
                    return this.LastDayOfMonth(parameters);
                case "dateadd":
                    return this.DateAdd(parameters);
                case "datediff":
                    return this.DateDiff(parameters);
                case "addbusinessdays":
                    return this.AddBusinessDays(parameters);
                case "isempty":
                    return this.IsEmpty(parameters);
                case "notisempty":
                    return this.NotIsEmpty(parameters);
                case "isnumeric":
                    return this.IsNumeric(parameters);
                case "notisnumeric":
                    return this.NotIsNumeric(parameters);
                case "keepnumeric":
                    return this.KeepNumeric(parameters);
                case "abs":
                    return this.Abs(parameters);
                case "ceiling":
                    return this.Ceiling(parameters);
                case "floor":
                    return this.Floor(parameters);
                case "inverse":
                    return this.Inverse(parameters);
                case "round":
                    return this.Round(parameters);
                case "not":
                    return this.Not(parameters);
                case "getcfgvalue":
                    return this.ReturnString(this.GetCFGValue(parameters));
                case "setcfgvalue":
                    return this.ReturnString(this.SetCFGValue(parameters));
                case "getbooleanflags":
                    return this.GetBooleanFlags(parameters);
                case "getflag":
                    this.CheckParametersType(parameters, 2);
                    return this.GetFlag(parameters);
                case "getaddress":
                    this.CheckParametersType(parameters, 2);
                    return this.getAddress(parameters);
                case "lookup":
                    return this.LookUp(parameters);
                default:
                    if (this.CustomFunctions != null)
                        return this.CustomFunctions(funName, parameters);
                    InvalidFunctionException functionException = new InvalidFunctionException(string.Format("The function:{0} can't be defined.", (object)funName));
                    functionException.Name = funName;
                    functionException.Source = this.GetType().FullName;
                    throw functionException;
            }
        }

        private string NotContains(string[] parameters)
        {
            this.CheckParametersType(parameters, 2);
            string lower1 = this.ConvertString(string.Format("{0}", (object)parameters[0])).ToLower();
            string lower2 = this.ConvertString(string.Format("{0}", (object)parameters[1])).ToLower();
            return string.IsNullOrEmpty(lower1) ? (string.IsNullOrEmpty(lower2) ? "false" : "true") : (lower1.Contains(lower2) ? "false" : "true");
        }

        private string Not(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            parameters[0].Replace("\"", string.Empty);
            bool result;
            if (!bool.TryParse(parameters[0], out result))
                throw new ArgumentNullException(string.Format("Invalid paramemter:{0} for Not function.", (object)parameters[0]));
            return result ? "false" : "true";
        }

        protected virtual void CheckParametersType(string[] parameters, int length)
        {
            this.CheckParametersType(parameters, length, length);
        }

        protected virtual void CheckParametersType(string[] parameters, int minLength, int maxLength)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (parameters.Length < minLength && minLength != -1)
                throw new ArgumentException("Invalid length of parameters.");
            if (parameters.Length > maxLength && maxLength != -1)
                throw new ArgumentException("Invalid length of parameters.");
        }

        protected string IIf(string[] parameters)
        {
            this.CheckParametersType(parameters, 3);
            return string.Compare(parameters[0], "true", true) == 0 ? parameters[1] : parameters[2];
        }

        protected string IsNumeric(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            if (string.IsNullOrEmpty(parameters[0]))
                return "false";
            bool flag = false;
            parameters[0] = this.ConvertString(parameters[0]);
            for (int index = 0; index < parameters[0].Length; ++index)
            {
                if (parameters[0][index] == '.' && !flag)
                    flag = true;
                else if (!char.IsDigit(parameters[0][index]))
                    return "false";
            }
            return "true";
        }

        protected string NotIsNumeric(string[] parameters)
        {
            return !(this.IsNumeric(parameters) == "true") ? "true" : "false";
        }

        protected string KeepNumeric(string[] parameters)
        {
            string parameter = parameters[0];
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < parameter.Length; ++index)
            {
                char c = parameter[index];
                if (char.IsDigit(c))
                    stringBuilder.Append(c);
            }
            return stringBuilder.ToString();
        }

        protected string CStr(string[] parameters)
        {
            if (parameters.Length == 0)
                return this.ConvertString(string.Empty);
            if (parameters.Length == 2)
                throw new NotSupportedException("Invalid length of parameters.");
            this.CheckParametersType(parameters, 1, 3);
            if (parameters.Length == 1)
                return this.ConvertString(parameters[0]);
            string format = "{0:" + this.ConvertString(parameters[2]) + "}";
            switch (parameters[1].ToLower())
            {
                case "d":
                case "date":
                case "datetime":
                    if (parameters[0] == "##" || parameters[0] == "#//#")
                        return string.Empty;
                    DateTime result1;
                    if (!DateTime.TryParse(parameters[0], out result1))
                        throw new ArgumentException(string.Format("Invalid date value:{0}", (object)parameters[0]));
                    return string.Format(format, (object)result1);
                case "decimal":
                case "n":
                case "money":
                case "number":
                case "numeric":
                    Decimal result2;
                    if (!Decimal.TryParse(parameters[0], out result2))
                        throw new ArgumentException(string.Format("Invalid number value:{0}", (object)parameters[0]));
                    return string.Format(format, (object)result2);
                default:
                    throw new ArgumentException(string.Format("Invalid type:{0}", (object)parameters[1]));
            }
        }

        protected string CNum(string[] parameters)
        {
            parameters[0] = this.ConvertString(parameters[0]);
            Decimal result;
            return Decimal.TryParse(parameters[0], out result) ? result.ToString() : "0";
        }

        protected string CDate(string[] parameters)
        {
            string str = this.ConvertString(parameters[0]);
            return str.StartsWith("#") && str.EndsWith("#") && str.Length >= 2 ? str : string.Format("#{0}#", (object)str);
        }

        protected string Round(string[] parameters)
        {
            this.CheckParametersType(parameters, 2);
            double result1;
            int result2;
            return !double.TryParse(parameters[0], out result1) || !int.TryParse(parameters[1], out result2) ? string.Empty : Math.Round(result1, result2).ToString("F" + (object)result2);
        }

        protected string Abs(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            double result;
            if (!double.TryParse(parameters[0], out result))
                throw new ArgumentNullException(string.Format("Invalid paramemter:{0} for ABS function.", (object)parameters[0]));
            return result >= 0.0 ? result.ToString() : (0.0 - result).ToString();
        }

        protected string Ceiling(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            double result;
            if (!double.TryParse(parameters[0], out result))
                throw new ArgumentNullException(string.Format("Invalid paramemter:{0} for Ceiling function.", (object)parameters[0]));
            return Math.Ceiling(result).ToString();
        }

        protected string Floor(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            double result;
            if (!double.TryParse(parameters[0], out result))
                throw new ArgumentNullException(string.Format("Invalid paramemter:{0} for Floor function.", (object)parameters[0]));
            return Math.Floor(result).ToString();
        }

        protected string Inverse(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            double result;
            if (!double.TryParse(parameters[0], out result))
                throw new ArgumentNullException(string.Format("Invalid paramemter:{0} for Inverse function.", (object)parameters[0]));
            return (result * -1.0).ToString();
        }

        protected string Format(string[] parameters)
        {
            this.CheckParametersType(parameters, 2);
            string result = this.ConvertString(parameters[0]);
            if (string.IsNullOrEmpty(result))
                return this.ReturnString(string.Empty);
            string formatting = this.ConvertString(parameters[1]);
            return this.ReturnString(ValueFormatter.FormatValue(result, formatting));
        }

        protected string Contains(string[] parameters)
        {
            this.CheckParametersType(parameters, 2);
            string lower1 = this.ConvertString(string.Format("{0}", (object)parameters[0])).ToLower();
            string lower2 = this.ConvertString(string.Format("{0}", (object)parameters[1])).ToLower();
            return string.IsNullOrEmpty(lower1) ? (string.IsNullOrEmpty(lower2) ? "true" : "false") : (!lower1.Contains(lower2) ? "false" : "true");
        }

        protected string StartsWith(string[] parameters)
        {
            this.CheckParametersType(parameters, 2);
            string lower1 = this.ConvertString(string.Format("{0}", (object)parameters[0])).ToLower();
            string lower2 = this.ConvertString(string.Format("{0}", (object)parameters[1])).ToLower();
            return string.IsNullOrEmpty(lower1) ? (string.IsNullOrEmpty(lower2) ? "true" : "false") : (!lower1.StartsWith(lower2) ? "false" : "true");
        }

        protected string EndsWith(string[] parameters)
        {
            this.CheckParametersType(parameters, 2);
            string lower1 = this.ConvertString(string.Format("{0}", (object)parameters[0])).ToLower();
            string lower2 = this.ConvertString(string.Format("{0}", (object)parameters[1])).ToLower();
            return string.IsNullOrEmpty(lower1) ? (string.IsNullOrEmpty(lower2) ? "true" : "false") : (!lower1.EndsWith(lower2) ? "false" : "true");
        }

        protected string Left(string[] parameters)
        {
            this.CheckParametersType(parameters, 2);
            parameters[0] = this.ConvertString(parameters[0]);
            return this.SubString(new string[3]
            {
        parameters[0],
        "0",
        parameters[1]
            });
        }

        protected string PadLeft(string[] parameters)
        {
            this.CheckParametersType(parameters, 2, 3);
            int result;
            if (!int.TryParse(parameters[1], out result))
                throw new ArgumentException("invalid width");
            parameters[0] = this.ConvertString(parameters[0]);
            string str = this.ConvertString(parameters[2]);
            if (parameters.Length == 2)
                return parameters[0].PadLeft(result);
            if (string.IsNullOrEmpty(str))
                str = " ";
            return parameters[0].PadLeft(result, str.ToCharArray()[0]);
        }

        protected string Right(string[] parameters)
        {
            this.CheckParametersType(parameters, 2);
            parameters[0] = this.ConvertString(parameters[0]);
            return this.SubString(new string[3]
            {
        parameters[0],
        (parameters[0].Length - int.Parse(parameters[1])).ToString(),
        parameters[1]
            });
        }

        protected string PadRight(string[] parameters)
        {
            this.CheckParametersType(parameters, 2, 3);
            int result;
            if (!int.TryParse(parameters[1], out result))
                throw new ArgumentException("invalid width");
            parameters[0] = this.ConvertString(parameters[0]);
            string str = this.ConvertString(parameters[2]);
            if (parameters.Length == 2)
                return parameters[0].PadRight(result);
            if (string.IsNullOrEmpty(str))
                str = " ";
            return parameters[0].PadRight(result, str.ToCharArray()[0]);
        }

        protected string Replace(string[] parameters)
        {
            this.CheckParametersType(parameters, 3);
            parameters[0] = this.ConvertString(parameters[0]);
            string oldValue = this.ConvertString(parameters[1]);
            string newValue = this.ConvertString(parameters[2]);
            switch (oldValue.ToLower())
            {
                case "$cr":
                    oldValue = "\r";
                    break;
                case "$lf":
                    oldValue = "\n";
                    break;
                case "$crlf":
                    oldValue = "\r\n";
                    break;
            }
            return parameters[0].Replace(oldValue, newValue);
        }

        protected string IndexOf(string[] parameters)
        {
            this.CheckParametersType(parameters, 2);
            return string.Format("{0}", (object)this.ConvertString(parameters[0]).IndexOf(this.ConvertString(parameters[1])));
        }

        protected string LastIndexOf(string[] parameters)
        {
            this.CheckParametersType(parameters, 2);
            return string.Format("{0}", (object)this.ConvertString(parameters[0]).LastIndexOf(this.ConvertString(parameters[1])));
        }

        protected string SubString(string[] parameters)
        {
            this.CheckParametersType(parameters, 2, 3);
            int startIndex = int.Parse(parameters[1]);
            int length = parameters.Length == 2 ? -1 : int.Parse(parameters[2]);
            parameters[0] = this.ConvertString(parameters[0]);
            if (length == -1)
                return parameters[0].Length < startIndex ? string.Empty : parameters[0].Substring(startIndex);
            if (parameters[0].Length < startIndex)
                return string.Empty;
            return parameters[0].Length < startIndex + length ? parameters[0].Substring(startIndex) : parameters[0].Substring(startIndex, length);
        }

        protected string Length(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            parameters[0] = this.ConvertString(parameters[0]);
            return parameters[0].Length.ToString();
        }

        protected string RTrim(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            return this.ConvertString(parameters[0]).TrimEnd();
        }

        protected string LTrim(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            return this.ConvertString(parameters[0]).TrimStart();
        }

        protected string Trim(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            return this.ConvertString(parameters[0]).Trim();
        }

        protected string Mid(string[] parameters)
        {
            this.CheckParametersType(parameters, 3);
            for (int index = 0; index < parameters.Length; ++index)
                parameters[index] = this.ConvertString(parameters[index]);
            Decimal num1 = new Decimal(0);
            Decimal num2 = parameters[0] == string.Empty ? new Decimal(0) : Decimal.Parse(parameters[0]);
            Decimal num3 = parameters[1] == string.Empty ? new Decimal(0) : Decimal.Parse(parameters[1]);
            Decimal num4 = parameters[2] == string.Empty ? new Decimal(0) : Decimal.Parse(parameters[2]);
            return (num2 < num3 && num2 > num4 || num2 > num3 && num2 < num4 ? num2 : (num3 < num2 && num3 > num4 || num3 > num2 && num3 < num4 ? num3 : (num4 < num2 && num4 > num3 || num4 > num2 && num4 < num3 ? num4 : (!(num2 == num4) ? (!(num4 == num3) ? (!(num3 == num2) ? new Decimal(0) : num2) : num3) : num4)))).ToString();
        }

        protected string Array_Mid(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            return this.Mid(this.GetArray(parameters[0]));
        }

        protected string Sum(string[] parameters)
        {
            Decimal num = new Decimal(0);
            for (int index = 0; index < parameters.Length; ++index)
            {
                Decimal result;
                if (!Decimal.TryParse(parameters[index], out result))
                    result = new Decimal(0);
                num += result;
            }
            return num.ToString();
        }

        protected string Array_Sum(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            return this.Sum(this.GetArray(parameters[0]));
        }

        protected string Avg(string[] parameters)
        {
            return string.Format("{0}", (object)(Decimal.Parse(this.Sum(parameters)) / (Decimal)parameters.Length));
        }

        protected string Array_Avg(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            return this.Avg(this.GetArray(parameters[0]));
        }

        protected string Median(string[] parameters)
        {
            Decimal[] array = new Decimal[parameters.Length];
            Decimal num1 = Decimal.Parse(this.Avg(parameters));
            for (int index = 0; index < parameters.Length; ++index)
            {
                Decimal result;
                if (!Decimal.TryParse(parameters[index], out result))
                    result = new Decimal(0);
                array[index] = result;
            }
            Array.Sort<Decimal>(array);
            if (parameters.Length % 2 != 0)
                return string.Format("{0}", (object)array[(parameters.Length - 1) / 2]);
            Decimal num2 = array[parameters.Length / 2 - 1];
            Decimal num3 = array[parameters.Length / 2];
            return string.Format("{0}", (object)(num2 - num1 > num3 - num1 ? num3 : num2));
        }

        protected string Array_Median(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            return this.Median(this.GetArray(parameters[0]));
        }

        protected string Min(string[] parameters)
        {
            Decimal num = new Decimal(0);
            for (int index = 0; index < parameters.Length; ++index)
            {
                Decimal result;
                if (string.IsNullOrWhiteSpace(parameters[index]))
                    result = new Decimal(0);
                else if (!Decimal.TryParse(parameters[index], out result))
                    continue;
                if (index == 0)
                    num = result;
                else if (result < num)
                    num = result;
            }
            return num.ToString();
        }

        protected string Array_Min(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            return this.Min(this.GetArray(parameters[0]));
        }

        protected string MinDate(string[] parameters)
        {
            DateTime dateTime = DateTime.MinValue;
            for (int index = 0; index < parameters.Length; ++index)
            {
                DateTime result;
                if (DateTime.TryParse(parameters[index], out result))
                {
                    if (index == 0)
                        dateTime = result;
                    else if (result < dateTime)
                        dateTime = result;
                }
            }
            return dateTime.ToString("MM/dd/yyyy");
        }

        protected string Array_MinDate(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            return this.MinDate(this.GetArray(parameters[0]));
        }

        protected string Max(string[] parameters)
        {
            Decimal num = new Decimal(0);
            for (int index = 0; index < parameters.Length; ++index)
            {
                Decimal result;
                if (string.IsNullOrWhiteSpace(parameters[index]))
                    result = new Decimal(0);
                else if (!Decimal.TryParse(parameters[index], out result))
                    continue;
                if (index == 0)
                    num = result;
                else if (result > num)
                    num = result;
            }
            return num.ToString();
        }

        protected string Array_Max(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            return this.Max(this.GetArray(parameters[0]));
        }

        protected string MaxDate(string[] parameters)
        {
            DateTime dateTime = DateTime.MinValue;
            for (int index = 0; index < parameters.Length; ++index)
            {
                DateTime result;
                if (DateTime.TryParse(parameters[index], out result))
                {
                    if (index == 0)
                        dateTime = result;
                    else if (result > dateTime)
                        dateTime = result;
                }
            }
            return dateTime.ToString("MM/dd/yyyy");
        }

        protected string Array_MaxDate(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            return this.MaxDate(this.GetArray(parameters[0]));
        }

        protected string First(string[] parameters)
        {
            this.CheckParametersType(parameters, 1, -1);
            return parameters[0];
        }

        protected string Array_First(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            return this.First(this.GetArray(parameters[0]));
        }

        protected string Last(string[] parameters)
        {
            this.CheckParametersType(parameters, 1, -1);
            return parameters[parameters.Length - 1];
        }

        protected string Array_Last(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            return this.Last(this.GetArray(parameters[0]));
        }

        protected string Join(string[] parameters)
        {
            this.CheckParametersType(parameters, 1, -1);
            string separator = this.ConvertString(parameters[0]);
            string[] strArray = new string[parameters.Length - 1];
            Array.Copy((Array)parameters, 1, (Array)strArray, 0, strArray.Length);
            string[] array = ((IEnumerable<string>)strArray).Select<string, string>((Func<string, string>)(s => this.ConvertString(s))).ToArray<string>();
            return string.Join(separator, array);
        }

        protected string Array_Join(string[] parameters)
        {
            this.CheckParametersType(parameters, 1, 2);
            return string.Join(parameters.Length == 1 ? "," : this.ConvertString(parameters[0]), ((IEnumerable<string>)this.GetArray(parameters.Length == 1 ? parameters[0] : parameters[1])).Select<string, string>((Func<string, string>)(s => this.ConvertString(s))).ToArray<string>());
        }

        protected string OneIsValue(string[] parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            this.CheckParametersType(parameters, 3, -1);
            string strB = this.ConvertString(parameters[0]);
            string str = this.ConvertString(parameters[1]);
            for (int index = 2; index < parameters.Length; ++index)
            {
                if (string.Compare(this.ConvertString(parameters[index]), strB, true) == 0)
                    return strB;
            }
            return str;
        }

        protected string Array_OneIsValue(string[] parameters)
        {
            this.CheckParametersType(parameters, 3);
            string strB = this.ConvertString(parameters[0]);
            string str = this.ConvertString(parameters[1]);
            foreach (string inputValue in this.GetArray(parameters[2]))
            {
                if (string.Compare(this.ConvertString(inputValue), strB, true) == 0)
                    return strB;
            }
            return str;
        }

        protected string ManyIsValue(string[] parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            this.CheckParametersType(parameters, 3, -1);
            string index1 = this.ConvertString(parameters[0]);
            string str = this.ConvertString(parameters[1]);
            IDictionary<string, int> source = (IDictionary<string, int>)new Dictionary<string, int>();
            for (int index2 = 2; index2 < parameters.Length; ++index2)
            {
                string key = this.ConvertString(parameters[index2]);
                if (source.ContainsKey(key))
                {
                    IDictionary<string, int> dictionary;
                    string index3;
                    (dictionary = source)[index3 = key] = dictionary[index3] + 1;
                }
                else
                    source.Add(key, 1);
            }
            string key1 = source.OrderByDescending<KeyValuePair<string, int>, int>((Func<KeyValuePair<string, int>, int>)(i => i.Value)).First<KeyValuePair<string, int>>().Key;
            return !key1.Equals(index1) && source[index1] != source[key1] ? str : index1;
        }

        protected string Array_ManyIsValue(string[] parameters)
        {
            this.CheckParametersType(parameters, 3);
            string index1 = this.ConvertString(parameters[0]);
            string str = this.ConvertString(parameters[1]);
            IDictionary<string, int> source = (IDictionary<string, int>)new Dictionary<string, int>();
            foreach (string inputValue in this.GetArray(parameters[2]))
            {
                string key = this.ConvertString(inputValue);
                if (source.ContainsKey(key))
                {
                    IDictionary<string, int> dictionary;
                    string index2;
                    (dictionary = source)[index2 = key] = dictionary[index2] + 1;
                }
                else
                    source.Add(key, 1);
            }
            string key1 = source.OrderByDescending<KeyValuePair<string, int>, int>((Func<KeyValuePair<string, int>, int>)(i => i.Value)).First<KeyValuePair<string, int>>().Key;
            return !key1.Equals(index1) && source[index1] != source[key1] ? str : index1;
        }

        protected virtual string[] GetArray(string paratmeter)
        {
            if (string.IsNullOrWhiteSpace(paratmeter))
                return new string[0];
            paratmeter = this.ConvertString(paratmeter);
            return paratmeter.Split(',');
        }

        protected string GetBooleanFlags(string[] parameters)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < parameters.Length && index < 10; ++index)
            {
                if (!string.IsNullOrEmpty(parameters[index]) && (parameters[index].ToLower() == "y" || parameters[index].ToLower() == "\"y\""))
                    stringBuilder.AppendFormat("{0}", (object)(index + 1));
            }
            return this.ReturnString(stringBuilder.ToString());
        }

        protected string GetFlag(string[] parameters)
        {
            int result;
            if (!int.TryParse(parameters[1], out result))
                throw new InvalidCastException(string.Format("Parameter of pos need integer, invalid value:", (object)result));
            parameters[0] = this.ConvertString(parameters[0].Trim());
            if (result < 1 || result > parameters[0].Length)
                return this.ReturnString(string.Empty);
            parameters[0] = this.ConvertString(parameters[0]);
            return this.ReturnString(parameters[0].Substring(result - 1, 1));
        }

        protected string IsEmpty(string[] parameters)
        {
            if (parameters.Length == 0)
                return "true";
            this.CheckParametersType(parameters, 1);
            string str1 = this.ConvertString(parameters[0]);
            if (string.IsNullOrEmpty(str1) || str1.Trim() == string.Empty)
                return "true";
            string str2 = str1.Trim();
            if (str2.StartsWith("#") && str2.EndsWith("#") && str2.Length >= 2)
            {
                string s = str2.Substring(1, str2.Length - 2).Trim();
                DateTime result;
                if (s == string.Empty || s == "//" || DateTime.TryParse(s, out result) && result == DateFieldCriterion.EmptyDate)
                    return "true";
            }
            return "false";
        }

        protected string NotIsEmpty(string[] parameters)
        {
            return this.IsEmpty(parameters) == "false" ? "true" : "false";
        }

        protected string Year(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            DateTime result;
            return parameters[0] == "##" || !DateTime.TryParse(parameters[0], out result) ? "-1" : result.Year.ToString();
        }

        protected string Month(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            DateTime result;
            return parameters[0] == "##" || !DateTime.TryParse(parameters[0], out result) ? "-1" : result.Month.ToString();
        }

        protected string Day(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            DateTime result;
            return parameters[0] == "##" || !DateTime.TryParse(parameters[0], out result) ? "-1" : result.Day.ToString();
        }

        protected string DayOfYear(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            DateTime result;
            return parameters[0] == "##" || !DateTime.TryParse(parameters[0], out result) ? "-1" : result.DayOfYear.ToString();
        }

        protected string DayOfWeek(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            DateTime result;
            return parameters[0] == "##" || !DateTime.TryParse(parameters[0], out result) ? "-1" : result.DayOfWeek.ToString();
        }

        protected string Hour(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            DateTime result;
            return parameters[0] == "##" || !DateTime.TryParse(parameters[0], out result) ? "-1" : result.Hour.ToString();
        }

        protected string Minute(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            DateTime result;
            return parameters[0] == "##" || !DateTime.TryParse(parameters[0], out result) ? "-1" : result.Minute.ToString();
        }

        protected string Second(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            DateTime result;
            return parameters[0] == "##" || !DateTime.TryParse(parameters[0], out result) ? "-1" : result.Second.ToString();
        }

        protected string Millisecond(string[] parameters)
        {
            this.CheckParametersType(parameters, 1);
            DateTime result;
            return parameters[0] == "##" || !DateTime.TryParse(parameters[0], out result) ? "-1" : result.Millisecond.ToString();
        }

        protected string DateAdd(string[] parameters)
        {
            this.CheckParametersType(parameters, 3);
            if (parameters[1] == "##" || parameters[1] == "#//#")
                return "##";
            string str = "##";
            double num = double.Parse(parameters[1]);
            DateTime result;
            if (!DateTime.TryParse(parameters[2], out result))
                return str;
            result = DateTime.Parse(parameters[2]);
            string dateTimeValue;
            switch (this.ConvertString(parameters[0].ToLower()))
            {
                case "s":
                case "second":
                    dateTimeValue = result.AddSeconds(num).ToString();
                    break;
                case "mm":
                case "minute":
                    dateTimeValue = result.AddMinutes(num).ToString();
                    break;
                case "h":
                case "hour":
                    dateTimeValue = result.AddHours(num).ToString();
                    break;
                case "d":
                case "day":
                    dateTimeValue = result.AddDays(num).ToString();
                    break;
                case "m":
                case "month":
                    dateTimeValue = result.AddMonths((int)num).ToString();
                    break;
                case "y":
                case "yy":
                case "year":
                    dateTimeValue = result.AddYears((int)num).ToString();
                    break;
                default:
                    throw new ArgumentException(string.Format("Invalid parameter for DataDiff function, part:{0}, add-value:{1}, date:{2}.", (object)parameters[0], (object)parameters[1], (object)parameters[2]));
            }
            return this.ReturnDateTime(dateTimeValue);
        }

        protected string FirstDayOfMonth(string[] parameters)
        {
            this.CheckParametersType(parameters, 1, 1);
            if (parameters[0] == "##" || parameters[0] == "#//#")
                return "##";
            string str = "##";
            DateTime result;
            return !DateTime.TryParse(parameters[0], out result) ? str : this.ReturnDateTime(new DateTime(result.Year, result.Month, 1).ToString());
        }

        protected string LastDayOfMonth(string[] parameters)
        {
            this.CheckParametersType(parameters, 1, 1);
            if (parameters[0] == "##" || parameters[0] == "#//#")
                return "##";
            string str = "##";
            DateTime result;
            if (!DateTime.TryParse(parameters[0], out result))
                return str;
            int day = DateTime.DaysInMonth(result.Year, result.Month);
            return this.ReturnDateTime(new DateTime(result.Year, result.Month, day).ToString());
        }

        protected string DateDiff(string[] parameters)
        {
            this.CheckParametersType(parameters, 3);
            string str = "-1";
            DateTime result1;
            if (parameters[1] == "##" || parameters[1] == "#//#")
                result1 = DateTime.MinValue;
            else if (!DateTime.TryParse(parameters[1], out result1))
                return str;
            DateTime result2;
            if (parameters[2] == "##" || parameters[2] == "#//#")
                result2 = DateTime.MinValue;
            else if (!DateTime.TryParse(parameters[2], out result2))
                return str;
            TimeSpan timeSpan = new TimeSpan(result1.Ticks - result2.Ticks);
            switch (this.ConvertString(parameters[0].ToLower()))
            {
                case "s":
                case "second":
                    return timeSpan.Seconds.ToString();
                case "mm":
                case "minute":
                    return timeSpan.Minutes.ToString();
                case "h":
                case "hour":
                    return timeSpan.Hours.ToString();
                case "d":
                case "day":
                    return timeSpan.Days.ToString();
                case "m":
                case "month":
                    return (result1.Year * 12 + result1.Month - (result2.Year * 12 + result2.Month)).ToString();
                case "y":
                case "yy":
                case "year":
                    return (result1.Year - result2.Year).ToString();
                default:
                    throw new ArgumentException(string.Format("Invalid parameter for DataDiff function, part:{0}, date1:{1}, data2:{2}.", (object)parameters[0], (object)parameters[1], (object)parameters[2]));
            }
        }

        protected string AddBusinessDays(string[] parameters)
        {
            this.CheckParametersType(parameters, 4);
            BusinessCalendarType result1;
            if (!Enum.TryParse<BusinessCalendarType>(this.ConvertString(parameters[0]), out result1))
                throw new ArgumentException(string.Format("Invaild business calendar type: {0}", (object)parameters[0]));
            DateTime result2;
            if (!DateTime.TryParse(parameters[1], out result2))
                throw new ArgumentException(string.Format("Invaild date: {0}", (object)parameters[1]));
            int result3;
            if (!int.TryParse(parameters[2], out result3))
                throw new ArgumentException(string.Format("Invaild count: {0}", (object)parameters[2]));
            bool boolean = Convert.ToBoolean(parameters[3]);
            return string.Format("#{0:MM/dd/yyyy}#", (object)GlobalConfiguration.CurrentSession.SystemSettings.GetBusinessCalendar(result1).AddBusinessDays(result2, result3, boolean));
        }

        public string LookUp(string[] parameters)
        {
            string fileName = this.ConvertString(parameters[0]);
            IList<string[]> csvCacheFile = FileParser.GetCsvCacheFile(fileName);
            string[] strArray1 = csvCacheFile[0];
            bool flag = false;
            int result = -1;
            if (!int.TryParse(parameters[1], out result))
            {
                parameters[1] = this.ConvertString(parameters[1]);
                for (int index = 0; index < strArray1.Length; ++index)
                {
                    if (parameters[1] == strArray1[index])
                    {
                        result = index;
                        flag = true;
                        break;
                    }
                }
            }
            else
                flag = true;
            int searchIndex = -1;
            if (!int.TryParse(parameters[2], out searchIndex))
            {
                parameters[2] = this.ConvertString(parameters[2]);
                for (int index = 0; index < strArray1.Length; ++index)
                {
                    if (parameters[2] == strArray1[index])
                    {
                        searchIndex = index;
                        break;
                    }
                }
            }
            string searchValue = parameters[3];
            IList<string[]> list = (IList<string[]>)csvCacheFile.Where<string[]>((Func<string[], bool>)(item => searchIndex < item.Length && item[searchIndex] == this.ConvertString(searchValue))).ToList<string[]>();
            string[] strArray2;
            try
            {
                strArray2 = list.First<string[]>();
            }
            catch (Exception ex)
            {
                return "\"\"";
            }
            if (!flag)
            {
                //GlobalTracer.TraceWarningFormat("The column header {0} is not found in the {1} file.", (object)parameters[1], (object)fileName);
                return string.Empty;
            }
            if (strArray2 == null || result >= strArray2.Length)
                return string.Empty;
            return !strArray2[result].StartsWith("\"") || !strArray2[result].EndsWith("\"") ? string.Format("\"{0}\"", (object)strArray2[result]) : strArray2[result];
        }

        public string LookUp2(string[] parameters)
        {
            string parameter = parameters[0];
            int index1 = int.Parse(parameters[1]);
            string[] array = new string[parameters.Length - 2];
            for (int index2 = 0; index2 < array.Length; ++index2)
                array[index2] = parameters[index2 > index1 ? index2 + 3 : index2 + 2];
            IList<string[]> source = FileParser.GetCsvCacheFile(parameter);
            for (int i = 0; i < array.Length; ++i)
            {
                if (i != index1)
                    source = (IList<string[]>)source.Where<string[]>((Func<string[], bool>)(item => i < item.Length && item[i] == array[i])).ToList<string[]>();
            }
            string[] strArray = source.First<string[]>();
            return strArray == null || index1 < strArray.Length ? string.Empty : strArray[index1];
        }

        protected string GetCFGValue(string[] parameters)
        {
            string empty = string.Empty;
            return GlobalConfiguration.AppSettings[this.ConvertString(parameters[0])];
        }

        protected string SetCFGValue(string[] parameters)
        {
            this.CheckParametersType(parameters, 2);
            GlobalConfiguration.AppSettings[this.ConvertString(parameters[0])] = this.ConvertString(parameters[1]);
            GlobalConfiguration.Save();
            return string.Empty;
        }

        private string getAddress(string[] parameters)
        {
            this.CheckParametersType(parameters, 2);
            return this.getAddress2(parameters[0], parameters[1]);
        }

        private string getAddress2(string fieldValue, string transType)
        {
            if (string.IsNullOrEmpty(fieldValue))
                fieldValue = string.Empty;
            if (string.IsNullOrWhiteSpace(transType))
                transType = string.Empty;
            string stringValue = string.Empty;
            if (fieldValue.StartsWith("\"") && fieldValue.EndsWith("\"") && fieldValue.Length >= 2)
                fieldValue = fieldValue.Substring(1, fieldValue.Length - 2);
            if (transType.StartsWith("\"") && transType.EndsWith("\"") && transType.Length >= 2)
                transType = transType.Substring(1, transType.Length - 2);
            string[] strArray1 = new string[12]
            {
        "E",
        "S",
        "W",
        "N",
        "E.",
        "S.",
        "W.",
        "N.",
        "EAST",
        "SOUTH",
        "WEST",
        "NORTH"
            };
            if (!string.IsNullOrEmpty(fieldValue))
            {
                string[] strArray2 = fieldValue.Split(' ');
                switch (transType.ToLower())
                {
                    case "transpropnbr":
                        if (strArray2.Length > 0)
                        {
                            int result;
                            stringValue = !int.TryParse(strArray2[0], out result) ? (!int.TryParse(strArray2[0].Replace("-", string.Empty), out result) ? string.Empty : string.Format("{0}", (object)strArray2[0])) : string.Format("{0}", (object)strArray2[0]);
                            break;
                        }
                        break;
                    case "transpropdir":
                        if (strArray2.Length > 1)
                        {
                            string upper = strArray2[1].ToUpper();
                            if (((IEnumerable<string>)strArray1).Contains<string>(upper))
                            {
                                stringValue = upper[0].ToString();
                                break;
                            }
                            break;
                        }
                        break;
                    case "transunitnumber":
                        if (strArray2.Length > 1)
                        {
                            bool flag = false;
                            for (int index = 0; index < strArray2.Length; ++index)
                            {
                                string str = strArray2[index];
                                if (flag)
                                    stringValue = stringValue + str + " ";
                                else if ("Unit".Equals(str, StringComparison.OrdinalIgnoreCase))
                                {
                                    flag = true;
                                }
                                else
                                {
                                    if (str.StartsWith("Unit#", StringComparison.OrdinalIgnoreCase))
                                    {
                                        stringValue = str.Substring(5);
                                        if (string.IsNullOrEmpty(stringValue) && index + 1 < strArray2.Length)
                                        {
                                            stringValue = strArray2[index + 1].Replace("#", "");
                                            break;
                                        }
                                        break;
                                    }
                                    if (str.Contains("#"))
                                    {
                                        stringValue = str.Replace("#", "");
                                        break;
                                    }
                                }
                            }
                            stringValue = stringValue.Trim();
                            break;
                        }
                        break;
                    case "transpropaddr":
                        if (strArray2.Length > 1)
                        {
                            int num = 0;
                            string str = string.Empty;
                            bool flag = false;
                            for (int index = 0; index < strArray2.Length; ++index)
                            {
                                string s = strArray2[index];
                                if (((IEnumerable<string>)strArray1).Contains<string>(s.ToUpper()))
                                {
                                    str = str + s + " ";
                                    ++num;
                                }
                                else if (int.TryParse(s, out int _) || s.Contains("#"))
                                    str = str + s + " ";
                                else if ("Unit".Equals(s, StringComparison.OrdinalIgnoreCase) || "Unit#".Equals(s, StringComparison.OrdinalIgnoreCase))
                                {
                                    ++index;
                                    flag = true;
                                }
                                else if (s.StartsWith("Unit#", StringComparison.OrdinalIgnoreCase))
                                {
                                    flag = true;
                                    if (s.Length <= 5)
                                        ++index;
                                }
                                else if (s == ",")
                                    stringValue = string.Format("{0},", (object)stringValue);
                                else if (!flag)
                                    stringValue = string.Format("{0} {1}", (object)stringValue, (object)s);
                            }
                            if (num >= 2)
                                stringValue = str.Trim() + stringValue;
                            stringValue = stringValue.Trim();
                            break;
                        }
                        break;
                    default:
                        return string.Empty;
                }
            }
            return this.ReturnString(stringValue);
        }

        protected string ConvertString(string inputValue)
        {
            if (inputValue == null)
                throw new ArgumentNullException(nameof(inputValue));
            return inputValue.StartsWith("\"") && inputValue.EndsWith("\"") && inputValue.Length >= 2 ? inputValue.Substring(1, inputValue.Length - 2) : inputValue;
        }

        protected string ReturnString(string stringValue)
        {
            return string.Format("\"{0}\"", (object)stringValue);
        }

        protected string ReturnDateTime(string dateTimeValue)
        {
            return string.Format("#{0}#", (object)dateTimeValue);
        }

        public class DefaultFunctionParser : FunctionParser
        {
            internal DefaultFunctionParser()
            {
            }
        }
    }
}
