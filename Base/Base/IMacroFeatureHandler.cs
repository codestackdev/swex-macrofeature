//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.sldworks;

namespace CodeStack.SwEx.MacroFeature.Base
{
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
        void Unload();
    }
}
