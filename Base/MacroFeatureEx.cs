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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeStack.SwEx.MacroFeature
{
    public abstract class MacroFeatureEx : ISwComFeature
    {
        public MacroFeatureEx()
        {
            var iconsConverter = new IconsConverter(System.Drawing.Color.White,
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

            //var isHighRes = true;

            //var icons = new List<string>();

            iconsConverter.ConvertIcon(regIcon, true);
            iconsConverter.ConvertIcon(suppIcon, true);
            iconsConverter.ConvertIcon(highIcon, true);
            iconsConverter.ConvertIcon(regIcon, false);
            iconsConverter.ConvertIcon(suppIcon, false);
            iconsConverter.ConvertIcon(highIcon, false);
            
            //for (int i = 0; i < regIconPahs.Length; i++)
            //{
            //    icons.Add(regIconPahs[i]);
            //    icons.Add(suppIconPahs[i]);
            //    icons.Add(highIconPahs[i]);
            //}

            //return icons.ToArray();
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object Edit(object app, object modelDoc, object feature)
        {
            return OnEditDefinition(app as ISldWorks, modelDoc as IModelDoc2, feature as IFeature);
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object Regenerate(object app, object modelDoc, object feature)
        {
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
            return OnUpdateState(app as ISldWorks, modelDoc as IModelDoc2, feature as IFeature);
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
        where TParams : class, new()
    {
        private Dictionary<IMacroFeatureData, THandler> m_Handlers;

        protected virtual void OnLoad(THandler handler)
        {
        }
    }
}
