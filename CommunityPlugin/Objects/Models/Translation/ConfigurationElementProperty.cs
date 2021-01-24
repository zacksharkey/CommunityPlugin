using CommunityPlugin.Objects.BaseClasses;
using System;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class ConfigurationElementProperty
    {
        private ConfigurationValidatorBase _validator;

        /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationElementProperty" /> class, based on a supplied parameter.</summary>
        /// <param name="validator">A <see cref="T:System.Configuration.ConfigurationValidatorBase" /> object.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="validator" /> is <see langword="null" />.</exception>
        public ConfigurationElementProperty(ConfigurationValidatorBase validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            this._validator = validator;
        }

        /// <summary>Gets a <see cref="T:System.Configuration.ConfigurationValidatorBase" /> object used to validate the <see cref="T:System.Configuration.ConfigurationElementProperty" /> object.</summary>
        /// <returns>A <see cref="T:System.Configuration.ConfigurationValidatorBase" /> object.</returns>
        public ConfigurationValidatorBase Validator
        {
            get
            {
                return this._validator;
            }
        }
    }
}
