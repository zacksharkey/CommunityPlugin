using CommunityPlugin.Objects.Models;
using System.Collections.Generic;

namespace CommunityPlugin.Objects.CustomDataObjects
{
    public class PipelineFilterCDO
    {
        public static string Key = $"{nameof(PipelineFilterCDO)}.json";

        public List<PipelineFilter> Filters { get; set; }

        public PipelineFilterCDO()
        {
            Filters = new List<PipelineFilter>();
        }
    }
}
