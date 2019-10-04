//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.Common.Attributes;
using CodeStack.SwEx.Common.Base;
using CodeStack.SwEx.Common.Icons;
using CodeStack.SwEx.Common.Reflection;
using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.SwEx.MacroFeature.Base;
using CodeStack.SwEx.MacroFeature.Icons;
using CodeStack.SwEx.MacroFeature.Properties;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CodeStack.SwEx.Common.Diagnostics;
using CodeStack.SwEx.Common.Enums;

namespace CodeStack.SwEx.MacroFeature
{
    /// <summary>
    /// Represents basic macro feature class
    /// </summary>
    /// <remarks>Mark the class as COM visible with <see cref="ComVisibleAttribute"/></remarks>
    [ModuleInfo("SwEx.MacroFeature")]
    public abstract class MacroFeatureEx : ISwComFeature, IModule
    {
        #region Initiation

        private readonly string m_Provider;
        private readonly ILogger m_Logger;

        public ILogger Logger
        {
            get
            {
                return m_Logger;
            }
        }

        public MacroFeatureEx()
        {
            string provider = "";
            this.GetType().TryGetAttribute<OptionsAttribute>(a =>
            {
                provider = a.Provider;
            });

            m_Provider = provider;
            m_Logger = LoggerFactory.Create(this);
            TryCreateIcons();
        }
        
        private void TryCreateIcons()
        {
            var iconsConverter = new IconsConverter(
                MacroFeatureIconInfo.GetLocation(this.GetType()), false);

            MacroFeatureIcon regIcon = null;
            MacroFeatureIcon highIcon = null;
            MacroFeatureIcon suppIcon = null;

            this.GetType().TryGetAttribute<FeatureIconAttribute>(a =>
            {
                regIcon = a.Regular;
                highIcon = a.Highlighted;
                suppIcon = a.Suppressed;
            });

            if (regIcon == null)
            {
                Image icon = null;

                this.GetType().TryGetAttribute<Common.Attributes.IconAttribute>(a =>
                {
                    icon = a.Icon;
                });

                if (icon == null)
                {
                    icon = Resources.default_icon;
                }

                regIcon = new MasterIcon(MacroFeatureIconInfo.RegularName)
                {
                    Icon = icon
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
            catch(Exception ex)
            {
                Logger.Log(ex);
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
            LogOperation("Editing feature", app as ISldWorks, modelDoc as IModelDoc2, feature as IFeature);

            return OnEditDefinition(app as ISldWorks, modelDoc as IModelDoc2, feature as IFeature);
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object Regenerate(object app, object modelDoc, object feature)
        {
            LogOperation("Regenerating feature", app as ISldWorks, modelDoc as IModelDoc2, feature as IFeature);

            SetProvider(app as ISldWorks, feature as IFeature);

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

        private void SetProvider(ISldWorks app, IFeature feature)
        {
            if (!string.IsNullOrEmpty(m_Provider))
            {
                if (app.IsVersionNewerOrEqual(SwVersion_e.Sw2016))
                {
                    var featData = feature.GetDefinition() as IMacroFeatureData;

                    if (featData.Provider != m_Provider)
                    {
                        featData.Provider = m_Provider;
                    }
                }
            }
        }

        private void LogOperation(string operName, ISldWorks app, IModelDoc2 modelDoc, IFeature feature)
        {
            Logger.Log($"{operName}: {feature?.Name} in {modelDoc?.GetTitle()} of SOLIDWORKS session: {app?.GetProcessID()}");
        }

        #endregion

        /// <summary>
        /// Called when the Edit feature menu is clicked from the feature manager tree
        /// </summary>
        /// <param name="app">Pointer to the application</param>
        /// <param name="model">Pointer to the current model where the feature resided</param>
        /// <param name="feature">Pointer to the feature being edited</param>
        /// <returns>Result of the editing</returns>
        /// <remarks>Use this handler to display property manager page or any other user interface to edit feature.
        /// Refer the <see href="https://www.codestack.net/labs/solidworks/swex/pmpage/">SwEx.PMPage Framework</see> for advanced way of creating property pages.
        /// Usually feature needs to be rolled back in the feature tree when edited. Use <see href="http://help.solidworks.com/2016/english/api/sldworksapi/SolidWorks.interop.sldworks~SolidWorks.interop.sldworks.IMacroFeatureData~AccessSelections.html">IMacroFeatureData::AccessSelections</see>
        /// to start editing of the feature. Call <see href="http://help.solidworks.com/2016/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.ifeature~modifydefinition.html">IFeature::ModifyDefinition</see> to submit the changes
        /// or <see href="http://help.solidworks.com/2016/english/api/sldworksapi/SolidWorks.interop.sldworks~SolidWorks.interop.sldworks.IMacroFeatureData~ReleaseSelectionAccess.html">IMacroFeatureData::ReleaseSelectionAccess</see> to cancel the editing.
        /// It is important to use the same pointer to <see href="http://help.solidworks.com/2016/english/api/sldworksapi/SolidWorks.interop.sldworks~SolidWorks.interop.sldworks.IMacroFeatureData.html">IMacroFeatureData</see> in all of the above methods.
        /// Use <see href="http://help.solidworks.com/2016/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.ifeature~getdefinition.html">IFeature::GetDefinition</see> to get the pointer and store it in a variable.
        /// </remarks>
        protected virtual bool OnEditDefinition(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            return true;
        }

        /// <summary>
        /// Called when macro feature is rebuilding
        /// </summary>
        /// <param name="app">Pointer to the SOLIDWORKS application</param>
        /// <param name="model">Pointer to the document where the macro feature being rebuild</param>
        /// <param name="feature">Pointer to the feature</param>
        /// <returns>Result of the operation. Use static methods of <see cref="MacroFeatureRebuildResult"/>
        /// class to generate results</returns>
        protected virtual MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            return null;
        }

        /// <summary>
        /// Called when state of the feature is changed (i.e. feature is selected, moved, updated etc.)
        /// Use this method to provide additional dynamic security options on your feature (i.e. do not allow dragging, editing etc.)
        /// </summary>
        /// <param name="app">Pointer to the application</param>
        /// <param name="model">Pointer to the model where the feature resides</param>
        /// <param name="feature">Pointer to the feature to updated state</param>
        /// <returns>State of feature as defined in <see href="http://help.solidworks.com/2016/english/api/swconst/SolidWorks.interop.swconst~SolidWorks.interop.swconst.swMacroFeatureSecurityOptions_e.html">swMacroFeatureSecurityOptions_e enumeration</see></returns>
        protected virtual swMacroFeatureSecurityOptions_e OnUpdateState(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            return swMacroFeatureSecurityOptions_e.swMacroFeatureSecurityByDefault;
        }
    }
}
