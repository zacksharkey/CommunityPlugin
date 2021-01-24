using CommunityPlugin.Objects.Interface;

namespace CommunityPlugin.Objects.Args
{
    public class ItemFilterEventArgs
    {
        public IMapping Mapping { get; internal set; }

        public string FieldID { get; internal set; }

        public object Result { get; set; }

        public Enums.ValueType ValueType { get; set; }

        public ItemFilterEventArgs()
        {
            this.ValueType = Enums.ValueType.Unknown;
        }
    }
}
