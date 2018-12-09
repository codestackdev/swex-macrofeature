//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.MacroFeature.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Attributes
{
    public class ParametersVersionAttribute : Attribute
    {
        internal Version Version { get; private set; }
        internal IParametersVersionConverter VersionConverter { get; private set; }

        public ParametersVersionAttribute(int major, int minor, Type versionConverterType)
            : this(versionConverterType)
        {
            Version = new Version(major, minor);
        }

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
