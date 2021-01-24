using System.Collections.Generic;

namespace CommunityPlugin.Objects.Interface
{
    public interface ISettings
    {
        IDictionary<string, string> Settings { get; }

        bool GetBoolSetting(string key);

        bool GetBoolSetting(string key, bool defaultValue);

        string GetSetting(string key);

        string GetSetting(string key, string defaultValue);
    }
}
