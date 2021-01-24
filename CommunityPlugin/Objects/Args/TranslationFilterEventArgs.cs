using CommunityPlugin.Objects.Interface;
using System;

namespace CommunityPlugin.Objects.Args
{
    public class TranslationFilterEventArgs
    {
        public IMapping Mapping { get; internal set; }

        public string Transalation { get; internal set; }

        [Obsolete]
        public string FiltedTranslation
        {
            get
            {
                return this.FilteredTranslation;
            }
            set
            {
                this.FilteredTranslation = value;
            }
        }

        public string FilteredTranslation { get; set; }

        internal TranslationFilterEventArgs()
        {
            this.FilteredTranslation = string.Empty;
        }
    }
}
