using System;

namespace CommunityPlugin.Objects.Args
{
    public class CriterionException : Exception
    {
        private string _criterion;

        public string Criterion
        {
            get
            {
                return this._criterion;
            }
        }

        internal CriterionException(string message)
          : this(message, string.Empty)
        {
        }

        internal CriterionException(string message, string criterion)
          : base(message)
        {
            this._criterion = criterion;
        }
    }
}
