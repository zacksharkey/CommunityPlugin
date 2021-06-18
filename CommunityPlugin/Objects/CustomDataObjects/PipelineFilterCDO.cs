using CommunityPlugin.Objects.Models;
using System.Collections.Generic;

namespace CommunityPlugin.Objects.CustomDataObjects
{
    public class PipelineFilterCDO
    {

        public List<PipelineFilter> Filters { get; set; }

        public PipelineFilterCDO()
        {
            Filters = new List<PipelineFilter>();
        }
    }
}
