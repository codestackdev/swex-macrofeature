//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
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

namespace CodeStack.SwEx.MacroFeature
{
    /// <inheritdoc cref="MacroFeatureEx"/>
    /// <summary>Represents macro feature which stores additional user parameters</summary>
    /// <typeparam name="TParams">Type of class representing parameters data model</typeparam>
    public abstract class MacroFeatureEx<TParams> : MacroFeatureEx
        where TParams : class, new()
    {
        /// <summary>
        /// Structure representing macro feature dimension
        /// </summary>
        /// <remarks>This is passed to <see cref="OnSetDimensions(ISldWorks, IModelDoc2, IFeature, DimensionDataCollection, TParams)"/>
        /// in the rebuild operation</remarks>
        protected class DimensionData : IDisposable
        {
            /// <summary>
            /// Pointer to display dimension
            /// </summary>
            public IDisplayDimension DisplayDimension { get; private set; }

            /// <summary>
            /// Pointer to dimension
            /// </summary>
            public IDimension Dimension { get; private set; }

            internal DimensionData(IDisplayDimension dispDim)
            {
                DisplayDimension = dispDim;
                Dimension = dispDim.GetDimension2(0);
            }

            public void Dispose()
            {
                if (Marshal.IsComObject(Dimension))
                {
                    Marshal.ReleaseComObject(Dimension);
                }

                if (Marshal.IsComObject(DisplayDimension))
                {
                    Marshal.ReleaseComObject(DisplayDimension);
                }

                Dimension = null;
                DisplayDimension = null;
            }
        }

        /// <summary>
        /// Collection of dimensions associated with this macro feature
        /// </summary>
        protected class DimensionDataCollection : ReadOnlyCollection<DimensionData>, IDisposable
        {
            internal DimensionDataCollection(IDisplayDimension[] dispDims)
                : base(new List<DimensionData>())
            {
                if (dispDims != null)
                {
                    for (int i = 0; i < dispDims.Length; i++)
                    {
                        this.Items.Add(new DimensionData(dispDims[i] as IDisplayDimension));
                    }
                }
            }

            public void Dispose()
            {
                if (Count > 0)
                {
                    foreach (var item in this.Items)
                    {
                        item.Dispose();
                    }

                    GC.Collect();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }

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
            var featDef = feature.GetDefinition() as IMacroFeatureData;

            IDisplayDimension[] dispDims;
            var parameters = GetParameters(feature, featDef, model, out dispDims);

            var res = OnRebuild(app, model, feature, parameters);

            UpdateDimensions(app, model, feature, dispDims, parameters);

            if (dispDims != null)
            {
                for (int i = 0; i < dispDims.Length; i++)
                {
                    dispDims[i] = null;
                }
            }

            return res;
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
        /// <remarks>Use the <see cref="DimensionEx.SetDirection(IDimension, Data.Point, Data.Vector, double, Data.Vector)"/>
        /// helper method to set the dimension orientation and position based on its values</remarks>
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
            var parameters = GetParameters(feat, featData, model, out dispDims);

            if (dispDims != null)
            {
                for (int i = 0; i < dispDims.Length; i++)
                {

                }
            }

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

        private TParams GetParameters(IFeature feat, IMacroFeatureData featData, IModelDoc2 model, out IDisplayDimension[] dispDims)
        {
            MacroFeatureOutdateState_e state;
            return GetParameters(feat, featData, model, out dispDims, out state);
        }

        private TParams GetParameters(IFeature feat, IMacroFeatureData featData, IModelDoc2 model, out IDisplayDimension[] dispDims, out MacroFeatureOutdateState_e state)
        {
            return m_ParamsParser.GetParameters<TParams>(feat, featData, model, out dispDims, out state);
        }

        private void UpdateDimensions(ISldWorks app, IModelDoc2 model, IFeature feature,
            IDisplayDimension[] dispDims, TParams parameters)
        {
            using (var dimsColl = new DimensionDataCollection(dispDims))
            {
                if (dimsColl.Any())
                {
                    OnSetDimensions(app, model, feature, dimsColl, parameters);
                }
            }
        }
    }
}
