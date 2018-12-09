//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
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
