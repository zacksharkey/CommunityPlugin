using CommunityPlugin.Objects.BaseClasses;
using System;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class DefaultValidator : ConfigurationValidatorBase
    {
        /// <summary>Determines whether an object can be validated, based on type.</summary>
        /// <param name="type">The object type.</param>
        /// <returns>
        /// <see langword="true" /> for all types being validated. </returns>
        public override bool CanValidate(Type type)
        {
            return true;
        }

        /// <summary>Determines whether the value of an object is valid. </summary>
        /// <param name="value">The object value.</param>
        public override void Validate(object value)
        {
        }
    }
}
