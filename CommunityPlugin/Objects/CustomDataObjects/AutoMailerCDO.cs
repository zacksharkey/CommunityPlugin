using CommunityPlugin.Objects.Models;
using System.Collections.Generic;

namespace CommunityPlugin.Objects.Helpers
{
    public class AutoMailerCDO
    {
        public List<MailTrigger> Triggers { get; set; }

        public AutoMailerCDO()
        {
            Triggers = new List<MailTrigger>();
        }
    }
}
