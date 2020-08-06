using System.Reflection;

namespace CommunityPlugin.Objects.Helpers
{
    public static class Reflect
    {
        public static object GetField(string Name, object obj)
        {
            FieldInfo field = obj.GetType().GetField(Name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field.GetValue(obj);
        }
    }
}
