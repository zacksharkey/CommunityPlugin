using CommunityPlugin.Objects.Enums;
using System;

namespace CommunityPlugin.Objects.Models
{
    public class IFBLog
    {
        public string UserID { get; set; }
        public DateTime Timestamp { get; set; }
        public IFBAction Action { get; set; }

        public object NewObject { get; set; }
    }
}
