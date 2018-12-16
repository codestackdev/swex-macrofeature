//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Base
{
    [Obsolete("Deprecated. Use MacroFeatureRebuildBodyResult instead")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public class MacroFeatureRebuildPatternResult : MacroFeatureRebuildResult
    {
        internal MacroFeatureRebuildPatternResult(IBody2[] bodiesPattern) : base(bodiesPattern)
        {
        }
    }
}
