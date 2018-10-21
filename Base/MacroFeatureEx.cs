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
            //UpdateIconsIfRequired(modelDoc as IModelDoc2, feature as IFeature);
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

        private void InitAndValidateFeature(ISldWorks app, IModelDoc2 model, IFeature feat, bool validateIcons = false)
        {
            bool isNew = true;
            var handler = m_Register.EnsureFeatureRegistered(app, model, feat, out isNew);

            //if (validateIcons && isNew)
            //{
            //    UpdateIconsIfRequired(model, feat);
            //}
        }

        private void UpdateIconsIfRequired(IModelDoc2 model, IFeature feat)
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
                    data.IconFiles = MacroFeatureIconInfo.GetIcons(this.GetType(), true);
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
        private readonly MacroFeatureParametersParser m_ParamsParser;

        public MacroFeatureEx()
        {
            m_ParamsParser = new MacroFeatureParametersParser();
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
        where TParams : class, IMacroFeatureHandler, new()
    {
        //private Dictionary<IMacroFeatureData, THandler> m_Handlers;

        //protected virtual void OnLoad(THandler handler)
        //{
        //}

        protected override Type MacroFeatureHandlerType
        {
            get
            {
                return typeof(THandler);
            }
        }
    }
}
