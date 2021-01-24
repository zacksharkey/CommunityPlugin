using System;

namespace CommunityPlugin.Objects.Interface
{
    public interface IFunctionParser
    {
        Func<string, string[], string> CustomFunctions { get; set; }

        string GetResult(string funName, string[] parameters);
    }
}
