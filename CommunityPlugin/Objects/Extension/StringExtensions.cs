namespace CommunityPlugin.Objects.Extension
{
    public static class StringExtensions
    {
        public static bool Empty(this string Text)
        {
            return string.IsNullOrEmpty(Text);
        }
    }
}
