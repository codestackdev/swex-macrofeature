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

        internal static string GetBaseName<TMacroFeature>()
            where TMacroFeature : MacroFeatureEx
        {
            return GetBaseName(typeof(TMacroFeature));
        }

        internal static string GetBaseName(Type macroFeatType)
        {
            if (!typeof(MacroFeatureEx).IsAssignableFrom(macroFeatType))
            {
                throw new InvalidCastException(
                    $"{macroFeatType.FullName} must inherit {typeof(MacroFeatureEx).FullName}");
            }

            string baseName = "";

            macroFeatType.TryGetAttribute<OptionsAttribute>(a =>
            {
                baseName = a.BaseName;
            });

            if (string.IsNullOrEmpty(baseName))
            {
                baseName = macroFeatType.Name;
            }

            return baseName;
        }

        internal static string GetProgId<TMacroFeature>()
            where TMacroFeature : MacroFeatureEx
        {
            string progId = "";

            if (!typeof(TMacroFeature).TryGetAttribute<ProgIdAttribute>(a => progId = a.Value))
            {
                progId = typeof(TMacroFeature).FullName;
            }

            return progId;
        }

        private MacroFeatureRegister m_Register;

        public MacroFeatureEx()
        {
            m_Register = new MacroFeatureRegister(GetBaseName(this.GetType()), MacroFeatureHandlerType);
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

            UpdateDimensions(feature as IFeature);

            if (res != null)
            {
                return res.GetResult();
            }
            else
            {
                return null;
            }
        }

        private void UpdateDimensions(IFeature feat)
        {
            var featData = feat.GetDefinition() as IMacroFeatureData;

            var dispDims = featData.GetDisplayDimensions() as object[];

            if (dispDims != null)
            {
                for (int i = 0; i < dispDims.Length; i++)
                {
                    var dispDim = dispDims[i] as IDisplayDimension;
                    var dim = dispDim.GetDimension2(0);

                    var val = (dim.GetSystemValue3(
                        (int)swInConfigurationOpts_e.swSpecifyConfiguration,
                        new string[] { featData.CurrentConfiguration.Name }) as double[])[0];

                    OnSetDimension(dispDim, dim, i, val);

                    Marshal.ReleaseComObject(dim);
                    Marshal.ReleaseComObject(dispDim);
                    dim = null;
                    dispDim = null;
                    dispDims[i] = null;
                }

                dispDims = null;
                GC.Collect();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        protected virtual void OnSetDimension(IDisplayDimension dispDim, IDimension dim, int index, double value)
        {
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object Security(object app, object modelDoc, object feature)
        {
            InitAndValidateFeature(app as ISldWorks, modelDoc as IModelDoc2,
                feature as IFeature, true);

            return OnUpdateState(app as ISldWorks, modelDoc as IModelDoc2, feature as IFeature);
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

    //public abstract class MacroFeatureEx<TParams> : MacroFeatureEx
    //    where TParams : class, new()
    //{
    //    protected override MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model, IFeature feature)
    //    {
    //        var res = base.OnRebuild(app, model, feature);

    //        UpdateDimensions(feature as IFeature);

    //        return res;
    //    }

    //    private void UpdateDimensions(IFeature feat)
    //    {
    //        var featData = feat.GetDefinition() as IMacroFeatureData;

    //        var dispDims = featData.GetDisplayDimensions() as object[];

    //        if (dispDims != null)
    //        {
    //            for (int i = 0; i < dispDims.Length; i++)
    //            {
    //                var dispDim = dispDims[i] as IDisplayDimension;
    //                var dim = dispDim.GetDimension2(0);

    //                var val = (dim.GetSystemValue3(
    //                    (int)swInConfigurationOpts_e.swSpecifyConfiguration,
    //                    new string[] { featData.CurrentConfiguration.Name }) as double[])[0];

    //                OnSetDimension(dispDim, dim, i, val);

    //                Marshal.ReleaseComObject(dim);
    //                Marshal.ReleaseComObject(dispDim);
    //                dim = null;
    //                dispDim = null;
    //                dispDims[i] = null;
    //            }

    //            dispDims = null;
    //            GC.Collect();
    //            GC.Collect();
    //            GC.WaitForPendingFinalizers();
    //        }
    //    }
    //    protected virtual void OnSetDimension(IDisplayDimension dispDim, IDimension dim, int index, double value)
    //    {
    //    }
    //}

    public abstract class MacroFeatureEx<THandler> : MacroFeatureEx
        where THandler : class, IMacroFeatureHandler, new()
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
