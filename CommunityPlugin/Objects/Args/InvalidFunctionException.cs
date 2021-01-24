using System;

namespace CommunityPlugin.Objects.Args
{
    public class InvalidFunctionException : Exception
    {
        public string Name { get; set; }

        public InvalidFunctionException(string message)
          : base(message)
        {
        }
    }
}
