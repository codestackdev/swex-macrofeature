//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using System;

namespace CodeStack.SwEx.MacroFeature.Base
{
    [Flags]
    public enum MacroFeatureOutdateState_e
    {
        UpToDate = 0,
        Icons = 1 << 0,
        Dimensions = 1 << 1
    }
}
