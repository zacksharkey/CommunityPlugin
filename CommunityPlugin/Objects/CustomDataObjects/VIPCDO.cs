using System.Collections.Generic;

namespace CommunityPlugin.Objects.CustomDataObjects
{
    public class VIPCDO
    {
        public List<string> Loans { get; set; }

        public VIPCDO()
        {
            Loans = new List<string>();
        }
    }
}
