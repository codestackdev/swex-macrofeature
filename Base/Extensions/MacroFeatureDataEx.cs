using CodeStack.SwEx.MacroFeature.Base;
using CodeStack.SwEx.MacroFeature.Helpers;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolidWorks.Interop.sldworks
{
    /// <summary>
    /// Extension methods of <see href="http://help.solidworks.com/2017/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IMacroFeatureData.html">IMacroFeatureData</see> interface
    /// </summary>
    public static class MacroFeatureDataEx
    {
        private static readonly MacroFeatureParametersParser m_ParamsParser;

        static MacroFeatureDataEx()
        {
            m_ParamsParser = new MacroFeatureParametersParser();
        }

        /// <summary>
        /// Deserializes the parameters of the macro feature to a structure
        /// </summary>
        /// <typeparam name="TParams">Type of parameters structure</typeparam>
        /// <param name="featData">Pointer to macro feature data</param>
        /// <param name="feat">Pointer to a feature</param>
        /// <param name="model">Pointer to model document</param>
        /// <returns>Parameters</returns>
        public static TParams GetParameters<TParams>(this IMacroFeatureData featData, IFeature feat, IModelDoc2 model)
            where TParams : class, new()
        {
            IDisplayDimension[] dispDims;
            IBody2[] bodies;
            MacroFeatureOutdateState_e state;
            string[] dispDimParams;
            return m_ParamsParser.GetParameters<TParams>(feat, featData, model,
                out dispDims, out dispDimParams, out bodies, out state);
        }

        /// <summary>
        /// Serializes the parameters of the macro feature to a structure
        /// </summary>
        /// <typeparam name="TParams">Type of parameters structure</typeparam>
        /// <param name="featData">Pointer to macro feature data</param>
        /// <param name="feat">Pointer to a feature</param>
        /// <param name="model">Pointer to model document</param>
        /// <param name="parameters">Parameters to serialize</param>
        public static void SetParameters<TParams>(this IMacroFeatureData featData, IFeature feat, IModelDoc2 model, TParams parameters)
            where TParams : class, new()
        {
            MacroFeatureOutdateState_e state;
            m_ParamsParser.SetParameters(model, feat, featData, parameters, out state);
        }
    }
}
