using CommunityPlugin.Objects.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityPlugin.Objects.Models
{
    public class LoanFolderRule
    {
        public string FolderName { get; set; }
        public bool Active { get; set; }
        public string Expression { get; set; }
        public string Milestone { get; set; }
        public int Order { get; set; }

        public bool Calculate()
        {
            bool ExpressionTrue = false;
            if (!string.IsNullOrEmpty(Expression))
            {
                try
                {
                    ExpressionTrue = (bool)EncompassHelper.ParseExpression(Expression, true);
                }
                catch(Exception ex)
                {
                    Logger.HandleError(ex, nameof(LoanFolderRule));
                }
            }

            bool milestonComplete = string.IsNullOrEmpty(Milestone) ? false : EncompassHelper.CheckMSComplete(Milestone);
            return milestonComplete || ExpressionTrue;
        }
    }
}
