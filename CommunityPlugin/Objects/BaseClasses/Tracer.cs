
using CommunityPlugin.Objects.Interface;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CommunityPlugin.Objects.BaseClasses
{
    public abstract class Tracer : ITracer, IDisposable
    {
        public virtual bool IsDebug { get; internal set; }

        public virtual string MessageFormat { get; set; }

        public void Verbose(string message)
        {
            this.Log(TraceLevel.Verbose, message);
        }

        public void Warning(string message)
        {
            this.Log(TraceLevel.Warning, message);
        }

        public void Info(string message)
        {
            this.Log(TraceLevel.Info, message);
        }

        public virtual void DebugInfo(string message)
        {
            if (!this.IsDebug)
                return;
            this.Log(TraceLevel.Info, message);
        }

        public void Error(string message)
        {
            this.Log(TraceLevel.Error, message);
        }

        public virtual void Verbose(string message, string className)
        {
            this.Log(TraceLevel.Verbose, message, className);
        }

        public virtual void Warning(string message, string className)
        {
            this.Log(TraceLevel.Warning, message, className);
        }

        public virtual void Info(string message, string className)
        {
            this.Log(TraceLevel.Info, message, className);
        }

        public virtual void DebugInfo(string message, string className)
        {
            if (!this.IsDebug)
                return;
            this.Log(TraceLevel.Info, message, className);
        }

        public virtual void Error(string message, string className)
        {
            this.Log(TraceLevel.Error, message, className);
        }

        public virtual void Log(TraceLevel level, string message)
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = Enumerable.Range(0, stackTrace.GetFrames().Length).Select<int, MethodBase>((Func<int, MethodBase>)(i => stackTrace.GetFrame(i).GetMethod())).Where<MethodBase>((Func<MethodBase, bool>)(m => m.DeclaringType != this.GetType())).FirstOrDefault<MethodBase>();
            string className = methodBase == (MethodBase)null ? string.Empty : string.Format("{0}.{1}", (object)methodBase.DeclaringType.FullName, (object)methodBase.Name);
            this.Log(level, message, className);
        }

        public abstract void Log(TraceLevel level, string message, string className);

        protected string GetLevelText(TraceLevel level)
        {
            switch (level)
            {
                case TraceLevel.Error:
                    return "ERROR";
                case TraceLevel.Warning:
                    return "WARNING";
                case TraceLevel.Info:
                    return "INFO";
                case TraceLevel.Verbose:
                    return "VERBOSE";
                default:
                    return string.Empty;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            int num = disposing ? 1 : 0;
        }
    }
}
