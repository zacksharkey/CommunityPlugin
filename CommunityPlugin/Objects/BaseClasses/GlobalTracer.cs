using CommunityPlugin.Objects.Enums;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models.Translation;
using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace CommunityPlugin.Objects.BaseClasses
{
    public class GlobalTracer
    {
        public static string CustomLogFileName = string.Empty;
        private static IDictionary<Assembly, ITracer> __cache = (IDictionary<Assembly, ITracer>)new Dictionary<Assembly, ITracer>(16);
        private const int DefaultLogLevel = 2;
        private const int DefaultDebugLogLevel = 4;
        private const string ConfigKeyOfLogErrorsToEventLog = "LogErrorsToEventLog";
        private const string ConfigKeyOfEventLogSource = "EventLogSource";
        private const string DefaultEventLogSource = "Utility4";
        private static ITracer __instance;

        public static ITracer Instance
        {
            get
            {
                switch (GlobalConfiguration.Mode)
                {
                    case RunMode.EncompassServer:
                        Assembly assembly = GlobalConfiguration.GetAssembly();
                        if (!GlobalTracer.__cache.ContainsKey(assembly))
                        {
                            GlobalTracer.Init();
                            if (!GlobalTracer.__cache.ContainsKey(assembly))
                                throw new Exception(string.Format("The assembly:{0} can't be initialized.", (object)assembly.GetName().Name));
                        }
                        return GlobalTracer.__cache[assembly];
                    case RunMode.WebServer:
                    case RunMode.Client:
                        if (GlobalTracer.__instance == null)
                        {
                            lock (typeof(GlobalTracer))
                            {
                                if (GlobalTracer.__instance == null)
                                {
                                    GlobalTracer.__instance = GlobalTracer.createTracer(GlobalConfiguration.GetAssembly());
                                    GlobalTracer.initTrace(GlobalTracer.__instance);
                                }
                            }
                        }
                        return GlobalTracer.__instance;
                    case RunMode.Test:
                        if (GlobalTracer.__instance == null)
                            GlobalTracer.__instance = (ITracer)new FakedTracer();
                        return GlobalTracer.__instance;
                    default:
                        throw new NotSupportedException("Current mode not support trace.");
                }
            }
        }

        public static void Init()
        {
            Assembly assembly = GlobalConfiguration.GetAssembly();
            if (GlobalTracer.__cache.ContainsKey(assembly))
                return;
            lock (typeof(GlobalTracer))
            {
                if (GlobalTracer.__cache.ContainsKey(assembly))
                    return;
                ITracer tracer = GlobalTracer.createTracer(assembly);
                GlobalTracer.__cache.Add(assembly, tracer);
            }
        }

        private static ITracer createTracer(Assembly assembly)
        {
            if (assembly == (Assembly)null)
                throw new ArgumentNullException(nameof(assembly));
            string traceFileName = GlobalTracer.getTraceFileName(assembly);
            int result;
            if (!int.TryParse(GlobalConfiguration.AppSettings["LogLevel"], out result))
                result = !GlobalConfiguration.Debug ? 2 : 4;
            ITracer tracer = (ITracer)new FileTracer(traceFileName)
            {
                LogLevel = result
            };
            if (GlobalConfiguration.GetBoolean("EnableServerLog") && GlobalConfiguration.CurrentSession != null && tracer is FileTracer)
                (tracer as FileTracer).ServerTracer = GlobalTracer.createServerTracer(GlobalConfiguration.CurrentSession, assembly);
            if (tracer == null)
                throw new Exception("Init tracer failed.");
            GlobalTracer.initTrace(tracer);
            try
            {
                tracer.Info(string.Format("The assembly:{0}, version:{1} was initialized.", (object)assembly.GetName().Name, (object)assembly.GetName().Version));
                tracer.Info(string.Format("The Utility4, version:{0} was loaded.", (object)typeof(GlobalTracer).Assembly.GetName().Version));
            }
            catch (Exception ex)
            {
            }
            return tracer;
        }

        private static ServerTracer createServerTracer(Session session, Assembly assembly)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));
            if (assembly == (Assembly)null)
                throw new ArgumentNullException(nameof(assembly));
            string fileName = GlobalConfiguration.AppSettings["ServerLogFileName"];
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = string.Format("{0}.log", (object)assembly.GetName().Name);
            int result;
            if (!int.TryParse(GlobalConfiguration.AppSettings["ServerLogLevel"], out result))
                result = 2;
            if (result > 2)
                result = 2;
            return new ServerTracer(session, fileName)
            {
                LogLevel = result
            };
        }

        private static void initTrace(ITracer tracer)
        {
            if (tracer == null)
                throw new ArgumentNullException(nameof(tracer));
            if (!(tracer is Tracer))
                return;
            (tracer as Tracer).IsDebug = GlobalConfiguration.Debug;
        }

        private static string getTraceFileName(Assembly assembly)
        {
            string empty = string.Empty;
            string str1;
            if (!string.IsNullOrEmpty(GlobalTracer.CustomLogFileName))
            {
                str1 = GlobalTracer.CustomLogFileName;
            }
            else
            {
                str1 = GlobalConfiguration.AppSettings["LogFilePath"];
                if (string.IsNullOrEmpty(str1))
                    str1 = string.Format("{0}.log", (object)GlobalConfiguration.GetAssembly().GetName().Name);
            }
            string str2;
            switch (GlobalConfiguration.Mode)
            {
                case RunMode.EncompassServer:
                    str2 = string.Format("{0}Settings\\Logs\\{1}", (object)string.Format("{0}{1}", (object)Session.EncompassDataDirectory, Session.EncompassDataDirectory.EndsWith("\\") ? (object)string.Empty : (object)"\\"), (object)str1);
                    break;
                case RunMode.WebServer:
                case RunMode.Client:
                    if (string.IsNullOrEmpty(str1))
                    {
                        str1 = string.Format("{0}.log", (object)GlobalConfiguration.GetAssembly().GetName().Name);
                    }
                    else
                    {
                        FakedDataCollection fakedDataCollection = new FakedDataCollection();
                        fakedDataCollection.Silent = true;
                        string result;
                        if (fakedDataCollection.ExecuteTranslation(str1, out result, out string _))
                            str1 = result;
                    }
                    FileInfo fileInfo = new FileInfo(str1);
                    if (string.IsNullOrEmpty(fileInfo.DirectoryName) || !str1.StartsWith(fileInfo.DirectoryName))
                    {
                        string fileName;
                        switch (GlobalConfiguration.Mode)
                        {
                            case RunMode.WebServer:
                                fileName = HttpContext.Current.Server.MapPath(str1);
                                break;
                            case RunMode.Client:
                                fileName = string.Format("{0}\\{1}", (object)Environment.CurrentDirectory, (object)str1);
                                break;
                            default:
                                fileName = string.Format("{0}\\{1}", (object)AppDomain.CurrentDomain.BaseDirectory, (object)str1);
                                break;
                        }
                        fileInfo = new FileInfo(fileName);
                    }
                    str2 = fileInfo.FullName;
                    break;
                default:
                    str2 = string.Format("{0}.log", (object)GlobalConfiguration.GetAssembly().GetName().Name);
                    break;
            }
            return str2;
        }

        public static void TraceVerboseFormat(string format, params object[] args)
        {
            GlobalTracer.TraceVerbose(string.Format(format, args));
        }

        public static void TraceVerbose(string message)
        {
            GlobalTracer.Instance.Verbose(message, GlobalTracer.GetClassName());
        }

        public static void TraceVerbose(string message, string className)
        {
            GlobalTracer.Instance.Verbose(message, className);
        }

        public static void TraceDebugFormat(string format, params object[] args)
        {
            GlobalTracer.TraceDebug(string.Format(format, args));
        }

        public static void TraceDebug(string message)
        {
            GlobalTracer.Instance.DebugInfo(message, GlobalTracer.GetClassName());
        }

        public static void TraceDebug(string message, string className)
        {
            GlobalTracer.Instance.DebugInfo(message, className);
        }

        public static void TraceInfoFormat(string format, params object[] args)
        {
            GlobalTracer.TraceInfo(string.Format(format, args));
        }

        public static void TraceInfo(string message)
        {
            GlobalTracer.Instance.Info(message, GlobalTracer.GetClassName());
        }

        public static void TraceInfo(string message, string className)
        {
            GlobalTracer.Instance.Info(message, className);
        }

        public static void TraceWarningFormat(string format, params object[] args)
        {
            GlobalTracer.TraceWarning(string.Format(format, args));
        }

        public static void TraceWarning(string message)
        {
            GlobalTracer.Instance.Warning(message, GlobalTracer.GetClassName());
        }

        public static void TraceWarning(string message, string className)
        {
            GlobalTracer.Instance.Warning(message, className);
        }

        public static void TraceErrorFormat(string format, params object[] args)
        {
            GlobalTracer.TraceError(string.Format(format, args));
        }

        public static void TraceError(string message)
        {
            GlobalTracer.Instance.Error(message, GlobalTracer.GetClassName());
            if (!GlobalConfiguration.GetBoolean("LogErrorsToEventLog"))
                return;
            string source = GlobalConfiguration.AppSettings["EventLogSource"];
            if (string.IsNullOrEmpty(source))
                source = ".NET Runtime";
            try
            {
                if (!EventLog.SourceExists(source))
                    EventLog.CreateEventSource(new EventSourceCreationData(source, GlobalTracer.GetLogName()));
            }
            catch (Exception ex)
            {
                source = ".NET Runtime";
            }
            EventLog.WriteEntry(source, string.Format("{0} ({1})", (object)message, (object)GlobalTracer.GetClassName()), EventLogEntryType.Error, 1026);
        }

        public static void TraceError(string message, string className)
        {
            GlobalTracer.Instance.Error(message, GlobalTracer.GetClassName());
        }

        private static string GetClassName()
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = Enumerable.Range(0, stackTrace.FrameCount).Select<int, MethodBase>((Func<int, MethodBase>)(i => stackTrace.GetFrame(i).GetMethod())).Where<MethodBase>((Func<MethodBase, bool>)(m => m.DeclaringType != typeof(GlobalTracer))).FirstOrDefault<MethodBase>();
            return !(methodBase == (MethodBase)null) ? string.Format("{0}.{1}", (object)methodBase.DeclaringType.FullName, (object)methodBase.Name) : string.Empty;
        }

        private static string GetLogName()
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = Enumerable.Range(0, stackTrace.FrameCount).Select<int, MethodBase>((Func<int, MethodBase>)(i => stackTrace.GetFrame(i).GetMethod())).Where<MethodBase>((Func<MethodBase, bool>)(m => m.DeclaringType != typeof(GlobalTracer))).FirstOrDefault<MethodBase>();
            return !(methodBase == (MethodBase)null) ? methodBase.DeclaringType.Assembly.GetName().Name : string.Empty;
        }
    }
}
