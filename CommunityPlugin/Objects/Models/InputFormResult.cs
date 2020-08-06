using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityPlugin.Objects.Models
{
    public class InputFormResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Html { get; set; }
        public InputFormResult(string ID, string Name, string Html)
        {
            this.Id = Id;
            this.Name = Name;
            this.Html = Html;
        }
    }
}
