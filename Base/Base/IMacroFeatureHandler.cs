//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.sldworks;

namespace CodeStack.SwEx.MacroFeature.Base
{
    public interface IMacroFeatureHandler
    {
        void Init(ISldWorks app, IModelDoc2 model, IFeature feat);
        void Unload();
    }
}
