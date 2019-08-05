//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

namespace SolidWorks.Interop.sldworks
{
    internal static class SldWorksEx
    {
        internal static bool SupportsHighResIcons(this ISldWorks app)
        {
            const int SW_2017_REV = 25;

            var majorRev = int.Parse(app.RevisionNumber().Split('.')[0]);

            return majorRev >= SW_2017_REV;
        }
    }
}
