//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Attributes
{
    public class OptionsAttribute : Attribute
    {
        internal swMacroFeatureOptions_e Flags { get; private set; }
        internal string BaseName { get; private set; }
        internal string Provider { get; private set; }

        public OptionsAttribute(string baseName,
            swMacroFeatureOptions_e flags = swMacroFeatureOptions_e.swMacroFeatureByDefault)
            : this(baseName, "", flags)
        {
        }

        public OptionsAttribute(string baseName, string provider,
            swMacroFeatureOptions_e flags = swMacroFeatureOptions_e.swMacroFeatureByDefault)
        {
            Flags = flags;
            BaseName = baseName;
            Provider = provider;
        }
    }
}
