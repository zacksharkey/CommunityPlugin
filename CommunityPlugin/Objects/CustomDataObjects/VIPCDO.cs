using System.Collections.Generic;

namespace CommunityPlugin.Objects.CustomDataObjects
{
    public class VIPCDO
    {
        public static string Key = $"{nameof(VIPCDO)}.json";

        public List<string> Loans { get; set; }

        public VIPCDO()
        {
            Loans = new List<string>();
        }
    }
}
