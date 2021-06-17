using System;
using System.Collections.Generic;
using System.Linq;

namespace CommunityFunction.Objects
{
    public class InterfaceHelper
    {
        public List<Type> GetAll(Type ClassType)
        {
            try
            {
                return ((IEnumerable<Type>)this.GetType().Assembly.GetTypes()).Where<Type>((Func<Type, bool>)(type => type.IsSubclassOf(ClassType))).ToList<Type>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
