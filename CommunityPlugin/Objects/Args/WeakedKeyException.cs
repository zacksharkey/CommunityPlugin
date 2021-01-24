using System;

namespace CommunityPlugin.Objects.Args
{
    public class WeakedKeyException : Exception
    {
        internal WeakedKeyException(string message)
          : base(message)
        {
        }
    }
}
