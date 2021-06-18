﻿using CommunityPlugin.Objects.Models;
using System.Collections.Generic;

namespace CommunityPlugin.Objects.CustomDataObjects
{
    public class LoanFolderRules
    {
        public List<LoanFolderRule> Rules { get; set; }

        public LoanFolderRules()
        {
            Rules = new List<LoanFolderRule>();
        }
    }
}
