using System;

namespace CommunityPlugin.Objects.Interface
{
    public interface ICriterionParser
    {
        bool Test(string criterionString);

        bool Test(string criterionString, IMapping mapping);

        string GetResult(string criterionString);

        string GetResult(string criterionString, IMapping mapping);

        string ExecuteDefinition(string definition);

        string ExecuteDefinition(string definition, IMapping mapping);

        Func<string, string[], string> CustomFunctions { get; set; }
    }
}