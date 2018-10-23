using CodeStack.SwEx.MacroFeature.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolidWorks.Interop.sldworks
{
    public static class MacroFeatureDataExtension
    {
        private static readonly MacroFeatureParametersParser m_ParamsParser;

        static MacroFeatureDataExtension()
        {
            m_ParamsParser = new MacroFeatureParametersParser();
        }

        public static TParams GetParameters<TParams>(this IMacroFeatureData featData)
            where TParams : class, new()
        {
            return m_ParamsParser.GetParameters<TParams>(featData);
        }

        public static void SetParameters<TParams>(this IMacroFeatureData featData, TParams parameters)
            where TParams : class, new()
        {
            m_ParamsParser.SetParameters(featData, parameters);
        }
    }
}
