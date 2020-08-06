using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityPlugin.Objects.Models
{
    public class InputFormEvent
    {
        public string EncompassFormName { get; set; }
        public string EventType { get; set; }
        public string EventLocationId { get; set; }
        public string CustomCode { get; set; }
    }
}
