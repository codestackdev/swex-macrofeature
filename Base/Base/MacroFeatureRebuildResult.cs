//**********************
//SwEx - development tools for SOLIDWORKS
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
            return new MacroFeatureRebuildBodyResult(body, featData, updateEntityIds);
        }
        
        public static MacroFeatureRebuildResult FromPattern(IBody2[] bodiesPattern)
        {
            return new MacroFeatureRebuildPatternResult(bodiesPattern);
        }

        public static MacroFeatureRebuildResult FromStatus(bool status)
        {
            return new MacroFeatureRebuldStatusResult(status);
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
