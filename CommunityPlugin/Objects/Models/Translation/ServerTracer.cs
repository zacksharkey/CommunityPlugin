using CommunityPlugin.Objects.BaseClasses;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using EllieMae.Encompass.BusinessObjects;
using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class ServerTracer : Tracer, ITracer, IDisposable
    {
        public const int DefaultLogLevel = 2;

        public Session Session { get; private set; }

        public string FileName { get; private set; }

        public string FileMaskName { get; private set; }

        public DateTime Now { get; private set; }

        public Encoding Encoding { get; set; }

        public int LogLevel { get; set; }

        private bool Initialize { get; set; }

        protected virtual bool IsDisposed { get; set; }

        public ServerTracer(Session session, string fileName)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            this.Session = session;
            this.Encoding = Encoding.Default;
            this.Initialize = false;
            this.Now = this.Session.GetServerTime();
            this.FileMaskName = fileName;
            this.FileName = this.convertFileName(this.Now, fileName);
            this.LogLevel = 2;
            this.IsDisposed = false;
        }

        private string convertFileName(DateTime date, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            Assembly assembly = GlobalConfiguration.GetAssembly();
            if (fileName.Contains("<AssemblyName>"))
                fileName = fileName.Replace("<AssemblyName>", assembly.GetName().Name);
            if (fileName.Contains("$assembly"))
                fileName = fileName.Replace("$assembly", assembly.GetName().Name);
            if (fileName.Contains("$year"))
                fileName = fileName.Replace("$year", string.Format("{0:yyyy}", (object)date));
            if (fileName.Contains("$month"))
                fileName = fileName.Replace("$month", string.Format("{0:MM}", (object)date));
            if (fileName.Contains("$day"))
                fileName = fileName.Replace("$day", string.Format("{0:dd}", (object)date));
            return fileName;
        }

        private IList<string> getPurgedFiles(DateTime date, string fileName, out bool isToday)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            isToday = false;
            IList<string> stringList = (IList<string>)new List<string>(8);
            int result1;
            if (!int.TryParse(GlobalConfiguration.AppSettings["PurgeLogAge"], out result1))
                result1 = 0;
            if (result1 == 0)
                return stringList;
            int result2;
            if (!int.TryParse(GlobalConfiguration.AppSettings["MaxCheckDays"], out result2))
                result2 = 60;
            DateTime result3;
            if (!DateTime.TryParse(GlobalConfiguration.AppSettings["ServerLogPurgeDate"], out result3))
                result3 = DateTime.MinValue;
            if (result3.Equals(date) || result3.AddDays(1.0) > date && result3.Day == date.Day)
            {
                isToday = true;
                return stringList;
            }
            int num1 = result3 == DateTime.MinValue ? 0 : (date - result3).Days;
            if (num1 > 0 && result2 > num1 + result1)
                result2 = num1 + result1;
            string str1 = this.convertFileName(date, this.FileMaskName);
            int num2 = result1;
            while (num2 != result2)
            {
                string str2 = this.convertFileName(date.AddDays((double)-num2), this.FileMaskName);
                ++num2;
                if (!str1.Equals(str2, StringComparison.CurrentCultureIgnoreCase) && !stringList.Contains(str2))
                    stringList.Add(str2);
            }
            return stringList;
        }

        public override void DebugInfo(string message)
        {
        }

        public override void DebugInfo(string message, string className)
        {
        }

        public override void Log(TraceLevel level, string message, string className)
        {
            if (!this.Initialize)
            {
                lock (this)
                {
                    if (!this.Initialize)
                    {
                        bool isToday = false;
                        try
                        {
                            IList<string> purgedFiles = this.getPurgedFiles(this.Now, this.FileName, out isToday);
                            if (purgedFiles.Count > 0)
                            {
                                foreach (string fileName in (IEnumerable<string>)purgedFiles)
                                {
                                    try
                                    {
                                        this.Session.DataExchange.DeleteCustomDataObject(fileName);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                            }
                        }
                        finally
                        {
                            try
                            {
                                if (!isToday)
                                {
                                    GlobalConfiguration.AppSettings["ServerLogPurgeDate"] = DateTime.Today.ToString();
                                    GlobalConfiguration.Save((object)this.Session, true);
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                            this.Initialize = true;
                        }
                    }
                }
            }
            if (level > (TraceLevel)this.LogLevel)
                return;
            GlobalConfiguration.GetAssembly();
            object[] objArray = new object[7]
            {
        (object) this.GetLevelText(level),
        (object) this.Session.GetServerTime(),
        (object) this.Session.UserID,
        (object) this.Session.ID,
        (object) Environment.MachineName,
        (object) message,
        (object) className
            };
            this.WriteToServer(string.Format(string.Format("{0}\r\n", (object)string.Join("|", ((IEnumerable<object>)objArray).Select<object, string>((Func<object, int, string>)((o, i) => "{" + i.ToString() + "}")).ToArray<string>())), objArray));
        }

        protected virtual void WriteToServer(string line)
        {
            if (this.IsDisposed || this.Session == null)
                return;
            byte[] bytes = this.Encoding.GetBytes(line);
            DataObject data = (DataObject)null;
            try
            {
                data = new DataObject(bytes);
                this.Session.DataExchange.AppendToCustomDataObject(this.FileName, data);
            }
            catch (Exception ex1)
            {
                try
                {
                    if (data == null)
                        return;
                    this.Session.DataExchange.SaveCustomDataObject(this.FileName, data);
                }
                catch (Exception ex2)
                {
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed)
                return;
            try
            {
                base.Dispose(disposing);
            }
            finally
            {
                if (disposing)
                {
                    this.Session = (Session)null;
                    this.IsDisposed = true;
                }
            }
        }
    }
}
