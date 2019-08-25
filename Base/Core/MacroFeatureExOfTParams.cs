//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.MacroFeature.Base;
using CodeStack.SwEx.MacroFeature.Helpers;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CodeStack.SwEx.MacroFeature.Data;
using SolidWorks.Interop.swpublished;

namespace CodeStack.SwEx.MacroFeature
{
    /// <inheritdoc cref="MacroFeatureEx"/>
    /// <summary>Represents macro feature which stores additional user parameters</summary>
    /// <typeparam name="TParams">Type of class representing parameters data model</typeparam>
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISwComFeature))]
    public abstract class MacroFeatureEx<TParams> : MacroFeatureEx
        where TParams : class, new()
    {
        private readonly MacroFeatureParametersParser m_ParamsParser;

        /// <summary>
        /// Base constructor. Should be called from the derived class as it contains required initialization
        /// </summary>
        public MacroFeatureEx()
        {
            m_ParamsParser = new MacroFeatureParametersParser(this.GetType());
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            Logger.Log("Rebuilding. Getting parameters");

            var featDef = feature.GetDefinition() as IMacroFeatureData;

            IDisplayDimension[] dispDims;
            IBody2[] editBodies;

            MacroFeatureOutdateState_e state;
            string[] dispDimParams;
            var parameters = m_ParamsParser.GetParameters<TParams>(feature, featDef, model, out dispDims,
                out dispDimParams, out editBodies, out state);
            
            Logger.Log("Rebuilding. Generating bodies");

            var rebuildRes = OnRebuild(app, model, feature, parameters);

            Logger.Log("Rebuilding. Updating dimensions");

            UpdateDimensions(app, model, feature, rebuildRes, dispDims, dispDimParams, parameters);

            Logger.Log("Rebuilding. Releasing dimensions");

            if (dispDims != null)
            {
                for (int i = 0; i < dispDims.Length; i++)
                {
                    dispDims[i] = null;
                }
            }

            return rebuildRes;
        }

        /// <inheritdoc cref="MacroFeatureEx.OnRebuild(ISldWorks, IModelDoc2, IFeature)"/>
        /// <param name="parameters">Current instance of parameters of this macro feature</param>
        protected virtual MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model, IFeature feature, TParams parameters)
        {
            return null;
        }

        /// <summary>
        /// Override this function to configure the dimensions of macro feature (i.e. position, style, etc.)
        /// </summary>
        /// <param name="app">Pointer to application</param>
        /// <param name="model">Pointer to current model</param>
        /// <param name="feature">Pointer to macro feature</param>
        /// <param name="dims">Pointer to dimensions of macro feature</param>
        /// <param name="parameters">Current instance of parameters (including the values of dimensions)</param>
        /// <remarks>Use the <see cref="DimensionDataExtension.SetOrientation(DimensionData, Point, Vector)"/>
        /// helper method to set the dimension orientation and position based on its values</remarks>
        protected virtual void OnSetDimensions(ISldWorks app, IModelDoc2 model, IFeature feature,
            MacroFeatureRebuildResult rebuildResult, DimensionDataCollection dims, TParams parameters)
        {
            OnSetDimensions(app, model, feature, dims, parameters);
        }

        /// <inheritdoc cref="OnSetDimensions(ISldWorks, IModelDoc2, IFeature, MacroFeatureRebuildResult, DimensionDataCollection, TParams)"/>
        protected virtual void OnSetDimensions(ISldWorks app, IModelDoc2 model, IFeature feature,
            DimensionDataCollection dims, TParams parameters)
        {
        }

        /// <summary>
        /// Returns the current instance of parameters data model for the feature
        /// </summary>
        /// <param name="feat">Pointer to feature</param>
        /// <param name="featData">Pointer to feature data</param>
        /// <param name="model">Pointer to model</param>
        /// <returns>Current instance of parameters</returns>
        protected TParams GetParameters(IFeature feat, IMacroFeatureData featData, IModelDoc2 model)
        {
            IDisplayDimension[] dispDims;
            IBody2[] editBodies;
            var parameters = GetParameters(feat, featData, model, out dispDims, out editBodies);

            return parameters;
        }

        /// <summary>
        /// Assigns the instance of data model to the macro feature parameters
        /// </summary>
        /// <param name="model">Pointer to model</param>
        /// <param name="feat">Pointer to feature</param>
        /// <param name="featData">Pointer to feature data</param>
        /// <param name="parameters">Parameters data model</param>
        /// <remarks>Call this method before calling the <see href="http://help.solidworks.com/2016/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.ifeature~modifydefinition.html">IFeature::ModifyDefinition</see></remarks>
        protected void SetParameters(IModelDoc2 model, IFeature feat, IMacroFeatureData featData, TParams parameters)
        {
            MacroFeatureOutdateState_e state;
            SetParameters(model, feat, featData, parameters, out state);
        }

        /// <inheritdoc cref="SetParameters(IModelDoc2, IFeature, IMacroFeatureData, TParams)"/>
        /// <param name="state">Current state of the parameters</param>
        protected void SetParameters(IModelDoc2 model, IFeature feat, IMacroFeatureData featData, TParams parameters, out MacroFeatureOutdateState_e state)
        {
            m_ParamsParser.SetParameters(model, feat, featData, parameters, out state);
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected TParams GetParameters(IFeature feat, IMacroFeatureData featData, IModelDoc2 model,
            out IDisplayDimension[] dispDims, out IBody2[] editBodies)
        {
            MacroFeatureOutdateState_e state;
            string[] dispDimParams;
            return m_ParamsParser.GetParameters<TParams>(feat, featData, model, out dispDims,
                out dispDimParams, out editBodies, out state);
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected void ParseParameters(TParams parameters,
            out string[] paramNames, out int[] paramTypes,
            out string[] paramValues, out object[] selection,
            out int[] dimTypes, out double[] dimValues, out IBody2[] editBodies)
        {
            m_ParamsParser.Parse(parameters, out paramNames, out paramTypes,
                out paramValues, out selection, out dimTypes, out dimValues, out editBodies);
        }

        private void UpdateDimensions(ISldWorks app, IModelDoc2 model, IFeature feature,
            MacroFeatureRebuildResult rebuildRes, IDisplayDimension[] dispDims, string[] dispDimParams, TParams parameters)
        {
            using (var dimsColl = new DimensionDataCollection(dispDims, dispDimParams))
            {
                if (dimsColl.Any())
                {
                    OnSetDimensions(app, model, feature, rebuildRes, dimsColl, parameters);
                }
            }
        }
    }
}
