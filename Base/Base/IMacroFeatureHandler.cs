//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.sldworks;
using System;

namespace CodeStack.SwEx.MacroFeature.Base
{
    /// <summary>
    /// Reason of macro feature handler unloading. Used in <see cref="IMacroFeatureHandler.Unload(MacroFeatureUnloadReason_e)"/>
    /// </summary>
    public enum MacroFeatureUnloadReason_e
    {
        /// <summary>
        /// Model containing this macro feature is closed
        /// </summary>
        ModelClosed,

        /// <summary>
        /// This macro feature is deleted
        /// </summary>
        Deleted
    }

    /// <summary>
    /// Handler of each macro feature used in <see cref="MacroFeatureEx{TParams, THandler}"/>
    /// </summary>
    /// <remarks>The instance of the specific class will be created for each macro feature</remarks>
    public interface IMacroFeatureHandler
    {
        /// <summary>
        /// Called when macro feature is created or loaded first time
        /// </summary>
        /// <param name="app">Pointer to application</param>
        /// <param name="model">Pointer to model</param>
        /// <param name="feat">Pointer to feature</param>
        void Init(ISldWorks app, IModelDoc2 model, IFeature feat);
        
        /// <summary>
        /// Called when macro feature is deleted or model is closed
        /// </summary>
        /// <param name="reason">Reason of macro feature unload</param>
        void Unload(MacroFeatureUnloadReason_e reason);
    }
}
