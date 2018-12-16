//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.MacroFeature.Base;
using System;

namespace CodeStack.SwEx.MacroFeature.Attributes
{
    /// <summary>
    /// Attributes specifies the current version of the macro feature parameters data model.
    /// This allows to implement backward compatibility for the macro feature parameters 
    /// for future versions of macro feature
    /// </summary>
    public class ParametersVersionAttribute : Attribute
    {
        internal Version Version { get; private set; }
        internal IParametersVersionConverter VersionConverter { get; private set; }

        /// <summary>
        /// Specifies the current version of the parameters data model
        /// </summary>
        /// <param name="major">Major version</param>
        /// <param name="minor">Minor version</param>
        /// <param name="versionConverterType">Type of the parameters converter between versions which implements 
        /// <see cref="IParametersVersionConverter"/> interface</param>
        public ParametersVersionAttribute(int major, int minor, Type versionConverterType)
            : this(versionConverterType)
        {
            Version = new Version(major, minor);
        }

        /// <inheritdoc cref="ParametersVersionAttribute(int, int, Type)"/>
        /// <param name="version">Full version</param>
        public ParametersVersionAttribute(string version, Type versionConverterType)
            : this(versionConverterType)
        {
            Version = new Version(version);
        }

        private ParametersVersionAttribute(Type versionConverterType)
        {
            if (!typeof(IParametersVersionConverter).IsAssignableFrom(versionConverterType))
            {
                throw new InvalidCastException(
                    $"{versionConverterType.FullName} must implement {typeof(IParameterConverter).FullName}");
            }

            VersionConverter = (IParametersVersionConverter)Activator.CreateInstance(versionConverterType);
        }
    }
}
