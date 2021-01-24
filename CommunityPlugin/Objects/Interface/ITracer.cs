using CommunityPlugin.Objects.Enums;
using System.Diagnostics;

namespace CommunityPlugin.Objects.Interface
{
    public interface ITracer
    {
        void Info(string message);

        void Info(string message, string className);

        void DebugInfo(string message);

        void DebugInfo(string message, string className);

        void Warning(string message);

        void Warning(string message, string className);

        void Error(string message);

        void Error(string message, string className);

        void Verbose(string message);

        void Verbose(string message, string className);

        void Log(TraceLevel level, string message);

        void Log(TraceLevel level, string message, string className);

        string MessageFormat { get; set; }

        bool IsDebug { get; }
    }
}
