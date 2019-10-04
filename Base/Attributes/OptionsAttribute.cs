//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.swconst;
using System;

namespace CodeStack.SwEx.MacroFeature.Attributes
{
    /// <summary>
    /// Provides additional options for macro feature
    /// </summary>
    public class OptionsAttribute : Attribute
    {
        internal swMacroFeatureOptions_e Flags { get; private set; }
        internal string BaseName { get; private set; }
        internal string Provider { get; private set; }

        /// <summary>
        /// Options for macro feature
        /// </summary>
        /// <param name="baseName">Base name of the macro feature.
        /// This is a default name assigned to the feature when created followed by the index</param>
        /// <param name="flags">Additional options for macro feature as defined in <see href="http://help.solidworks.com/2016/english/api/swconst/solidworks.interop.swconst~solidworks.interop.swconst.swmacrofeatureoptions_e.html">swMacroFeatureOptions_e enumeration</see></param>
        public OptionsAttribute(string baseName,
            swMacroFeatureOptions_e flags = swMacroFeatureOptions_e.swMacroFeatureByDefault)
            : this(baseName, "", flags)
        {
        }

        /// <inheritdoc cref="OptionsAttribute(string, swMacroFeatureOptions_e)"/>
        /// <param name="provider">Default message to display when the COM server is not registered for this feature.
        /// The provided text is displayed in the What's Wrong dialog of SOLIDWORKS after the 'Add-in not found. Please contact'. This feature is only available in SOLIDWORKS 2016 or newer (revision 24.0)</param>
        public OptionsAttribute(string baseName, string provider,
            swMacroFeatureOptions_e flags = swMacroFeatureOptions_e.swMacroFeatureByDefault)
        {
            Flags = flags;
            BaseName = baseName;
            Provider = provider;
        }
    }
}
