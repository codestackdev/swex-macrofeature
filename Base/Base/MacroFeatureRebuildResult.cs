//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Base
{
    public abstract class MacroFeatureRebuildResult
    {
        public static MacroFeatureRebuildResult FromBody(IBody2 body)
        {
            return new MacroFeatureRebuildBodyResult(body);
        }

        public static MacroFeatureRebuildResult FromBody(IBody2 body, IMacroFeatureData featData, bool updateEntityIds = true)
        {
            return new MacroFeatureRebuildBodyResult(featData, updateEntityIds, body);
        }

        public static MacroFeatureRebuildResult FromBodies(IBody2[] bodies, IMacroFeatureData featData, bool updateEntityIds = true)
        {
            return new MacroFeatureRebuildBodyResult(featData, updateEntityIds, bodies);
        }

        public static MacroFeatureRebuildResult FromPattern(IBody2[] bodiesPattern)
        {
            return new MacroFeatureRebuildPatternResult(bodiesPattern);
        }

        public static MacroFeatureRebuildResult FromStatus(bool status, string error = "")
        {
            return new MacroFeatureRebuldStatusResult(status, error);
        }

        private readonly object m_Result;

        internal MacroFeatureRebuildResult(object result)
        {
            m_Result = result;
        }

        internal object GetResult()
        {
            return m_Result;
        }
    }
}
