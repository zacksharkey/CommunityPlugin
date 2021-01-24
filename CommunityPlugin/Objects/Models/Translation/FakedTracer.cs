using CommunityPlugin.Objects.BaseClasses;
using CommunityPlugin.Objects.Enums;
using System.Diagnostics;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class FakedTracer : Tracer
    {
        internal FakedTracer()
        {
        }

        public override void Log(TraceLevel level, string message, string className)
        {
        }
    }
}
