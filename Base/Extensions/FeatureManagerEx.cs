//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.Common.Icons;
using CodeStack.SwEx.Common.Reflection;
using CodeStack.SwEx.MacroFeature;
using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.SwEx.MacroFeature.Base;
using CodeStack.SwEx.MacroFeature.Helpers;
using CodeStack.SwEx.MacroFeature.Icons;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SolidWorks.Interop.sldworks
{
    /// <summary>
    /// Extensions methods of <see href="http://help.solidworks.com/2016/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.ifeaturemanager_members.html">IFeatureManager</see> interface
    /// </summary>
    public static class FeatureManagerEx
    {
        private static readonly MacroFeatureParametersParser m_ParamsParser;
        private static readonly IconsConverter m_IconsConverter;
        private static readonly ISldWorks m_App;

        static FeatureManagerEx()
        {
            m_ParamsParser = new MacroFeatureParametersParser();
            m_IconsConverter = new IconsConverter();
            m_App = Context.CurrentApp;
        }

        /// <summary>
        /// Inserts new macro feature
        /// </summary>
        /// <typeparam name="TMacroFeature">Definition of COM macro feature</typeparam>
        /// <param name="featMgr">Pointer to feature manager</param>
        /// <returns>Newly created feature</returns>
        public static IFeature InsertComFeature<TMacroFeature>(this IFeatureManager featMgr)
            where TMacroFeature : MacroFeatureEx
        {
            if (typeof(TMacroFeature).IsAssignableToGenericType(typeof(MacroFeatureEx<>)))
            {
                var paramsType = typeof(TMacroFeature).GetArgumentsOfGenericType(
                    typeof(MacroFeatureEx<>)).First();

                var parameters = Activator.CreateInstance(paramsType);

                return InsertComFeatureWithParameters(featMgr, typeof(TMacroFeature), parameters);
            }
            else
            {
                return InsertComFeatureBase(
                    featMgr, typeof(TMacroFeature), null, null, null, null, null, null, null);
            }
        }

        
        /// <inheritdoc cref="InsertComFeature{TMacroFeature}(IFeatureManager)"/>
        /// <typeparam name="TParams">Type of parameters to serialize to macro feature</typeparam>
        /// <param name="parameters">Parameters to serialize to macro feature</param>
        public static IFeature InsertComFeature<TMacroFeature, TParams>(this IFeatureManager featMgr, TParams parameters)
            where TMacroFeature : MacroFeatureEx<TParams>
            where TParams : class, new()
        {
            return InsertComFeatureWithParameters(featMgr, typeof(TMacroFeature), parameters);
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static IFeature InsertComFeature(this IFeatureManager featMgr, Type macroFeatType, object parameters)
        {
            return InsertComFeatureWithParameters(featMgr, macroFeatType, parameters);
        }

        /// <summary>
        /// Replaces existing macro feature with a new one preserving the parameters
        /// </summary>
        /// <typeparam name="TMacroFeature">Type of macro feature</typeparam>
        /// <param name="featMgr">Pointer to feature manager</param>
        /// <param name="feat">Pointer to feature to replace</param>
        /// <returns>Ne replaced feature</returns>
        public static IFeature ReplaceComFeature<TMacroFeature>(this IFeatureManager featMgr, IFeature feat)
            where TMacroFeature : MacroFeatureEx
        {
            if (feat == null)
            {
                throw new ArgumentNullException(nameof(feat));
            }

            var featData = feat.GetDefinition() as IMacroFeatureData;

            if (featData == null)
            {
                throw new NullReferenceException("Specified feature not a macro feature");
            }

            var model = featMgr.Document;

            object parameters = null;

            if (typeof(TMacroFeature).IsAssignableToGenericType(typeof(MacroFeatureEx<>)))
            {
                featData.AccessSelections(model, null);

                var paramsType = typeof(TMacroFeature).GetArgumentsOfGenericType(typeof(MacroFeatureEx<>)).First();
                IDisplayDimension[] dispDims;
                IBody2[] editBodies;
                MacroFeatureOutdateState_e state;
                parameters = m_ParamsParser.GetParameters(feat, featData, model, paramsType, out dispDims, out editBodies, out state);
                MacroFeatureParametersParser.ReleaseDisplayDimensions(dispDims);
            }

            return featMgr.ReplaceComFeatureBase<TMacroFeature>(feat, parameters);
        }

        /// <inheritdoc cref="ReplaceComFeature{TMacroFeature}(IFeatureManager, IFeature)"/>
        /// <typeparam name="TParams">Type of parameters</typeparam>
        /// <param name="parameters">Parameters to assign to replaced feature</param>
        public static IFeature ReplaceComFeature<TMacroFeature, TParams>(this IFeatureManager featMgr, IFeature feat, TParams parameters)
            where TMacroFeature : MacroFeatureEx<TParams>
            where TParams : class, new()
        {
            return featMgr.ReplaceComFeatureBase<TMacroFeature>(feat, parameters);
        }
        
        private static IFeature ReplaceComFeatureBase<TMacroFeature>(this IFeatureManager featMgr, IFeature feat, object parameters)
            where TMacroFeature : MacroFeatureEx
        {
            if (feat == null)
            {
                throw new ArgumentNullException(nameof(feat));
            }

            var model = featMgr.Document;

            if (featMgr.EditRollback((int)swMoveRollbackBarTo_e.swMoveRollbackBarToAfterFeature, feat.Name))
            {
                IFeature newFeat = null;

                var name = feat.Name;
                
                if (parameters != null)
                {
                    newFeat = InsertComFeatureWithParameters(featMgr, typeof(TMacroFeature), parameters);
                }
                else
                {
                    newFeat = InsertComFeature<TMacroFeature>(featMgr);
                }
                
                if (newFeat != null)
                {
                    if (feat.Select2(false, -1))
                    {
                        int DEFAULT_DEL_OPTS = 0;
                        if (model.Extension.DeleteSelection2(DEFAULT_DEL_OPTS))
                        {
                            newFeat.Name = name;
                        }
                        else
                        {
                            Debug.Assert(false, "Failed to delete feature");
                        }
                    }
                    else
                    {
                        Debug.Assert(false, "Failed to select feature");
                    }
                }

                featMgr.EditRollback((int)swMoveRollbackBarTo_e.swMoveRollbackBarToEnd, "");

                return newFeat;
            }
            else
            {
                Debug.Assert(false, "Failed to rollback the feature");
                return null;
            }
        }

        private static IFeature InsertComFeatureWithParameters(
            IFeatureManager featMgr, Type macroFeatType, object parameters)
        {
            string[] paramNames;
            int[] paramTypes;
            string[] paramValues;
            object[] selection;
            int[] dimTypes;
            double[] dimValues;
            IBody2[] editBodies;

            m_ParamsParser.Parse(parameters,
                out paramNames, out paramTypes, out paramValues, out selection,
                out dimTypes, out dimValues, out editBodies, true);
            
            return InsertComFeatureBase(featMgr, macroFeatType, paramNames,
                paramTypes, paramValues, dimTypes, dimValues, selection, editBodies);
        }

        private static IFeature InsertComFeatureBase(IFeatureManager featMgr, Type macroFeatType,
            string[] paramNames, int[] paramTypes, string[] paramValues,
            int[] dimTypes, double[] dimValues, object[] selection, object[] editBodies)
        {
            if (!typeof(MacroFeatureEx).IsAssignableFrom(macroFeatType))
            {
                throw new InvalidCastException($"{macroFeatType.FullName} must inherit {typeof(MacroFeatureEx).FullName}");
            }

            var options = swMacroFeatureOptions_e.swMacroFeatureByDefault;
            var provider = "";

            macroFeatType.TryGetAttribute<OptionsAttribute>(a =>
            {
                options = a.Flags;
                provider = a.Provider;
            });

            var baseName = MacroFeatureInfo.GetBaseName(macroFeatType);

            var progId = MacroFeatureInfo.GetProgId(macroFeatType);

            if (string.IsNullOrEmpty(progId))
            {
                throw new NullReferenceException("Prog id for macro feature cannot be extracted");
            }

            var icons = MacroFeatureIconInfo.GetIcons(macroFeatType, m_App.SupportsHighResIcons());

            using (var selSet = new SelectionGroup(featMgr.Document.ISelectionManager))
            {
                if (selection != null && selection.Any())
                {
                    var selRes = selSet.AddRange(selection);

                    Debug.Assert(selRes);
                }
                
                var feat = featMgr.InsertMacroFeature3(baseName,
                    progId, null, paramNames, paramTypes,
                    paramValues, dimTypes, dimValues, editBodies, icons, (int)options) as IFeature;
                
                return feat;
            }
        }
    }
}