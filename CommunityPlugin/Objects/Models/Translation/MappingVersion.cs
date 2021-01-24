namespace CommunityPlugin.Objects.Models.Translation
{
    public class MappingVersion
    {
        private const string DefaultVersion = "1.0";

        public string Version { get; private set; }

        public static MappingVersion Default
        {
            get
            {
                return new MappingVersion("1.0");
            }
        }

        protected internal MappingVersion(string version)
        {
            this.Version = version;
        }
    }
}
