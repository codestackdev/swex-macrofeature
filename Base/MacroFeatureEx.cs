//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.Common.Icons;
using CodeStack.SwEx.Common.Reflection;
using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.SwEx.MacroFeature.Base;
using CodeStack.SwEx.MacroFeature.Helpers;
using CodeStack.SwEx.MacroFeature.Icons;
using CodeStack.SwEx.MacroFeature.Properties;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeStack.SwEx.MacroFeature
{
    public abstract class MacroFeatureEx : ISwComFeature
    {
        #region Initiation

        private string m_Provider;

        public MacroFeatureEx()
        {
            this.GetType().TryGetAttribute<OptionsAttribute>(a =>
            {
                m_Provider = a.Provider;
            });

            TryCreateIcons();
        }
        
        private void TryCreateIcons()
        {
            var iconsConverter = new IconsConverter(
                MacroFeatureIconInfo.GetLocation(this.GetType()), false);

            MacroFeatureIcon regIcon = null;
            MacroFeatureIcon highIcon = null;
            MacroFeatureIcon suppIcon = null;

            this.GetType().TryGetAttribute<IconAttribute>(a =>
            {
                regIcon = a.Regular;
                highIcon = a.Highlighted;
                suppIcon = a.Suppressed;
            });

            if (regIcon == null)
            {
                regIcon = new MasterIcon(MacroFeatureIconInfo.RegularName)
                {
                    Icon = Resources.default_icon
                };
            }

            if (highIcon == null)
            {
                highIcon = regIcon.Clone(MacroFeatureIconInfo.HighlightedName);
            }

            if (suppIcon == null)
            {
                suppIcon = regIcon.Clone(MacroFeatureIconInfo.SuppressedName);
            }

            //Creation of icons may fail if user doesn't have write permissions or icon is locked
            try
            {
                iconsConverter.ConvertIcon(regIcon, true);
                iconsConverter.ConvertIcon(suppIcon, true);
                iconsConverter.ConvertIcon(highIcon, true);
                iconsConverter.ConvertIcon(regIcon, false);
                iconsConverter.ConvertIcon(suppIcon, false);
                iconsConverter.ConvertIcon(highIcon, false);
            }
            catch
            {
            }
        }

        //this method crashes SOLIDWORKS - need to research
        private void UpdateIconsIfRequired(ISldWorks app, IModelDoc2 model, IFeature feat)
        {
            var data = (feat as IFeature).GetDefinition() as IMacroFeatureData;
            data.AccessSelections(model, null);
            var icons = data.IconFiles as string[];

            if (icons != null)
            {
                if (icons.Any(i =>
                {
                    string iconPath = "";

                    if (Path.IsPathRooted(i))
                    {
                        iconPath = i;
                    }
                    else
                    {
                        iconPath = Path.Combine(
                            Path.GetDirectoryName(this.GetType().Assembly.Location), i);
                    }

                    return !File.Exists(iconPath);
                }))
                {
                    data.IconFiles = MacroFeatureIconInfo.GetIcons(this.GetType(), app.SupportsHighResIcons());
                    feat.ModifyDefinition(data, model, null);
                }
            }
        }

        #endregion

        #region Overrides

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object Edit(object app, object modelDoc, object feature)
        {
            return OnEditDefinition(app as ISldWorks, modelDoc as IModelDoc2, feature as IFeature);
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object Regenerate(object app, object modelDoc, object feature)
        {
            SetProvider(feature);

            var res = OnRebuild(app as ISldWorks, modelDoc as IModelDoc2, feature as IFeature);

            if (res != null)
            {
                return res.GetResult();
            }
            else
            {
                return null;
            }
        }

        private void SetProvider(object feature)
        {
            if (!string.IsNullOrEmpty(m_Provider))
            {
                var featData = (feature as IFeature).GetDefinition() as IMacroFeatureData;

                if (featData.Provider != m_Provider)
                {
                    featData.Provider = m_Provider;
                }
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object Security(object app, object modelDoc, object feature)
        {
            return OnUpdateState(app as ISldWorks, modelDoc as IModelDoc2, feature as IFeature);
        }

        #endregion
        
        protected virtual bool OnEditDefinition(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            return true;
        }

        protected virtual MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            return null;
        }

        protected virtual swMacroFeatureSecurityOptions_e OnUpdateState(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            return swMacroFeatureSecurityOptions_e.swMacroFeatureSecurityByDefault;
        }
    }

    public abstract class MacroFeatureEx<TParams> : MacroFeatureEx
        where TParams : class, new()
    {
        protected class DimensionData : IDisposable
        {
            public IDisplayDimension DisplayDimension { get; private set; }
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

        protected virtual MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model, IFeature feature, TParams parameters)
        {
            return null;
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

        protected virtual void OnSetDimensions(ISldWorks app, IModelDoc2 model, IFeature feature, 
            DimensionDataCollection dims, TParams parameters)
        {
        }

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
        
        protected void SetParameters(IModelDoc2 model, IFeature feat, IMacroFeatureData featData, TParams parameters)
        {
            MacroFeatureOutdateState_e state;
            SetParameters(model, feat, featData, parameters, out state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="feat"></param>
        /// <param name="parameters"></param>
        /// <param name="isOutdated">Indicates that parameters version is outdated and feature needs to be replaced</param>
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
    }

    public abstract class MacroFeatureEx<TParams, THandler> : MacroFeatureEx<TParams>
        where THandler : class, IMacroFeatureHandler, new()
        where TParams : class, new()
    {
        private MacroFeatureRegister<THandler> m_Register;

        public MacroFeatureEx() : base()
        {
            m_Register = new MacroFeatureRegister<THandler>(MacroFeatureInfo.GetBaseName(this.GetType()));
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

        protected virtual swMacroFeatureSecurityOptions_e OnUpdateState(THandler handler)
        {
            return swMacroFeatureSecurityOptions_e.swMacroFeatureSecurityByDefault;
        }

        protected virtual MacroFeatureRebuildResult OnRebuild(THandler handler, TParams parameters)
        {
            return null;
        }

        protected virtual bool OnEditDefinition(THandler handler)
        {
            return true;
        }

        private THandler GetHandler(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            bool isNew = true;
            var handler = m_Register.EnsureFeatureRegistered(app, model, feature, out isNew);
            return handler;
        }
    }
}
