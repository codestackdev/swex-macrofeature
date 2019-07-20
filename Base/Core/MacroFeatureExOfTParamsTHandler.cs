//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.MacroFeature.Base;
using CodeStack.SwEx.MacroFeature.Helpers;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeStack.SwEx.MacroFeature
{
    /// <inheritdoc cref="MacroFeatureEx{TParams}"/>
    /// <summary>Represents macro feature which stores additional user parameters and provides per feature handler.
    /// This version of macro feature is useful where it is required to track the lifecycle of
    /// an individual feature as <see cref="MacroFeatureEx"/> behaves as a service and it creates
    /// one instance per application session</summary>
    /// <typeparam name="THandler">Handler of macro feature</typeparam>
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISwComFeature))]
    public abstract class MacroFeatureEx<TParams, THandler> : MacroFeatureEx<TParams>
        where THandler : class, IMacroFeatureHandler, new()
        where TParams : class, new()
    {
        private MacroFeatureRegister<THandler> m_Register;

        public MacroFeatureEx() : base()
        {
            m_Register = new MacroFeatureRegister<THandler>(
                MacroFeatureInfo.GetBaseName(this.GetType()), this);
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed bool OnEditDefinition(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            return OnEditDefinition(GetHandler(app, model, feature));
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed swMacroFeatureSecurityOptions_e OnUpdateState(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            return OnUpdateState(GetHandler(app, model, feature));
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected override sealed MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model, IFeature feature, TParams parameters)
        {
            return OnRebuild(GetHandler(app, model, feature), parameters);
        }

        /// <inheritdoc cref="MacroFeatureEx.OnUpdateState(ISldWorks, IModelDoc2, IFeature)"/>
        /// <param name="handler">Pointer to the macro feature handler of the feature being updated</param>
        protected virtual swMacroFeatureSecurityOptions_e OnUpdateState(THandler handler)
        {
            return swMacroFeatureSecurityOptions_e.swMacroFeatureSecurityByDefault;
        }
        
        /// <inheritdoc cref="MacroFeatureEx{TParams}.OnRebuild(ISldWorks, IModelDoc2, IFeature, TParams)"/>
        /// <param name="handler">Pointer to the macro feature handler of the feature being rebuilt</param>
        protected virtual MacroFeatureRebuildResult OnRebuild(THandler handler, TParams parameters)
        {
            return null;
        }

        /// <inheritdoc cref="MacroFeatureEx.OnEditDefinition(ISldWorks, IModelDoc2, IFeature)"/>
        /// <param name="handler">Pointer to the macro feature handler of the feature being edited</param>
        protected virtual bool OnEditDefinition(THandler handler)
        {
            return true;
        }

        private THandler GetHandler(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            bool isNew = true;
            var handler = m_Register.EnsureFeatureRegistered(app, model, feature, out isNew);

            Logger.Log($"Getting macro feature handler. New={isNew}");

            return handler;
        }
    }
}
