//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex-macrofeature
//**********************

using CodeStack.SwEx.Common.Icons;
using CodeStack.SwEx.Common.Reflection;
using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.SwEx.MacroFeature.Base;
using CodeStack.SwEx.MacroFeature.Helpers;
using CodeStack.SwEx.MacroFeature.Icons;
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
        private class DefaultMacroFeatureHandler : IMacroFeatureHandler
        {
            public void Init(ISldWorks app, IModelDoc2 model, IFeature feat)
            {
            }

            public void Unload()
            {
            }
        }

        private MacroFeatureRegister m_Register;

        #region Initiation

        public MacroFeatureEx()
        {
            m_Register = new MacroFeatureRegister(MacroFeatureInfo.GetBaseName(this.GetType()), MacroFeatureHandlerType);
            TryCreateIcons();
        }
        
        private void TryCreateIcons()
        {
            var iconsConverter = new IconsConverter(
                MacroFeatureIconInfo.GetLocation(this.GetType()), false);

            IIcon regIcon = null;
            IIcon highIcon = null;
            IIcon suppIcon = null;

            this.GetType().TryGetAttribute<IconAttribute>(a =>
            {
                regIcon = a.Regular;
                highIcon = a.Highlighted;
                suppIcon = a.Suppressed;
            });

            if (regIcon == null)
            {
                //TODO: load default
            }

            if (highIcon == null)
            {
                highIcon = regIcon;
            }

            if (suppIcon == null)
            {
                suppIcon = regIcon;
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

        private void InitAndValidateFeature(ISldWorks app, IModelDoc2 model, IFeature feat, bool validateIcons = false)
        {
            bool isNew = true;
            var handler = m_Register.EnsureFeatureRegistered(app, model, feat, out isNew);

            //if (validateIcons && isNew)
            //{
            //    UpdateIconsIfRequired(model, feat);
            //}
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
            InitAndValidateFeature(app as ISldWorks, modelDoc as IModelDoc2,
                feature as IFeature);
            
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
        
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object Security(object app, object modelDoc, object feature)
        {
            InitAndValidateFeature(app as ISldWorks, modelDoc as IModelDoc2,
                feature as IFeature, true);

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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual Type MacroFeatureHandlerType
        {
            get
            {
                return typeof(DefaultMacroFeatureHandler);
            }
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
                Marshal.ReleaseComObject(Dimension);
                Marshal.ReleaseComObject(DisplayDimension);
                Dimension = null;
                DisplayDimension = null;
            }
        }

        protected class DimensionDataCollection : ReadOnlyCollection<DimensionData>, IDisposable
        {
            internal DimensionDataCollection(IMacroFeatureData featData)
                : base(new List<DimensionData>())
            {
                var dispDims = featData.GetDisplayDimensions() as object[];

                if (dispDims != null)
                {
                    for (int i = 0; i < dispDims.Length; i++)
                    {
                        this.Items.Add(new DimensionData(dispDims[i] as IDisplayDimension));
                        dispDims[i] = 0;
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
            m_ParamsParser = new MacroFeatureParametersParser();
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            var featDef = feature.GetDefinition() as IMacroFeatureData;

            var parameters = GetParameters(featDef);

            var res = OnRebuild(app, model, feature, parameters);

            UpdateDimensions(featDef, parameters);

            return res;
        }

        protected virtual MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model, IFeature feature, TParams parameters)
        {
            return null;
        }

        private void UpdateDimensions(IMacroFeatureData featData, TParams parameters)
        {
            using (var dimsColl = new DimensionDataCollection(featData))
            {
                if (dimsColl.Any())
                {
                    OnSetDimensions(dimsColl, parameters);
                }
            }
        }
        protected virtual void OnSetDimensions(DimensionDataCollection dims, TParams parameters)
        {
        }

        protected TParams GetParameters(IMacroFeatureData featData)
        {
            return m_ParamsParser.GetParameters<TParams>(featData);
        }

        protected void SetParameters(IMacroFeatureData featData, TParams parameters)
        {
            m_ParamsParser.SetParameters(featData, parameters);
        }
    }

    public abstract class MacroFeatureEx<TParams, THandler> : MacroFeatureEx<TParams>
        where THandler : class, IMacroFeatureHandler, new()
        where TParams : class, new()
    {
        //private Dictionary<IMacroFeatureData, THandler> m_Handlers;

        //protected virtual void OnLoad(THandler handler)
        //{
        //}

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected override Type MacroFeatureHandlerType
        {
            get
            {
                return typeof(THandler);
            }
        }
    }
}
