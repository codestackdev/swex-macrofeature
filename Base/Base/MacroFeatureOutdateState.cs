//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using System;
using SolidWorks.Interop.sldworks;

namespace CodeStack.SwEx.MacroFeature.Base
{
    /// <summary>
    /// State of macro feature parameters returned from <see cref="MacroFeatureEx{TParams}.SetParameters(IModelDoc2, IFeature, IMacroFeatureData, TParams, out MacroFeatureOutdateState_e)"/>
    /// or <see cref="MacroFeatureEx{TParams}.SetParameters(IModelDoc2, IFeature, IMacroFeatureData, TParams, out MacroFeatureOutdateState_e)"/> in cases
    /// where new version of macro feature's parameters could not be fully upgraded via <see cref="IParametersVersionConverter"/>
    /// </summary>
    /// <remarks>Some of the macro feature parameters such as icons or dimensions cannot be upgraded, in this case
    /// the specific state will be returned. It is recommended to warn the user and use <see cref="FeatureManagerEx.ReplaceComFeature{TMacroFeature}(IFeatureManager, IFeature)"/>
    /// to properly upgrade feature</remarks>
    [Flags]
    public enum MacroFeatureOutdateState_e
    {
        /// <summary>
        /// All parameters are up-to-date
        /// </summary>
        UpToDate = 0,

        /// <summary>
        /// Macro feature icon has changed and cannot be updated
        /// </summary>
        Icons = 1 << 0,

        /// <summary>
        /// Macro feature dimensions have changed and cannot be upgraded
        /// </summary>
        Dimensions = 1 << 1
    }
}
