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

                return InsertComFeatureWithParameters<TMacroFeature>(featMgr, parameters);
            }
            else
            {
                return InsertComFeatureBase<TMacroFeature>(
                    featMgr, null, null, null, null, null, null, null);
            }
        }

        
        /// <inheritdoc cref="InsertComFeature{TMacroFeature}(IFeatureManager)"/>
        /// <typeparam name="TParams">Type of parameters to serialize to macro feature</typeparam>
        /// <param name="parameters">Parameters to serialize to macro feature</param>
        public static IFeature InsertComFeature<TMacroFeature, TParams>(this IFeatureManager featMgr, TParams parameters)
            where TMacroFeature : MacroFeatureEx<TParams>
            where TParams : class, new()
        {
            return InsertComFeatureWithParameters<TMacroFeature>(featMgr, parameters);
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
                var paramsType = typeof(TMacroFeature).GetArgumentsOfGenericType(typeof(MacroFeatureEx<>)).First();
                IDisplayDimension[] dispDims;
                MacroFeatureOutdateState_e state;
                parameters = m_ParamsParser.GetParameters(feat, featData, model, paramsType, out dispDims, out state);
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

                if (feat.Select2(false, -1))
                {
                    int DEFAULT_DEL_OPTS = 0;
                    if (!model.Extension.DeleteSelection2(DEFAULT_DEL_OPTS))
                    {
                        Debug.Assert(false, "Failed to delete feature");
                    }
                }
                else
                {
                    Debug.Assert(false, "Failed to select feature");
                }

                if (parameters != null)
                {
                    newFeat = InsertComFeatureWithParameters<TMacroFeature>(featMgr, parameters);
                }
                else
                {
                    newFeat = InsertComFeature<TMacroFeature>(featMgr);
                }

                featMgr.EditRollback((int)swMoveRollbackBarTo_e.swMoveRollbackBarToEnd, "");

                newFeat.Name = name;

                return newFeat;
            }
            else
            {
                Debug.Assert(false, "Failed to rollback the feature");
                return null;
            }
        }

        private static IFeature InsertComFeatureWithParameters<TMacroFeature>(
            IFeatureManager featMgr, object parameters)
            where TMacroFeature : MacroFeatureEx
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
                out dimTypes, out dimValues, out editBodies);
            
            return InsertComFeatureBase<TMacroFeature>(featMgr, paramNames,
                paramTypes, paramValues, dimTypes, dimValues, selection, editBodies);
        }

        private static IFeature InsertComFeatureBase<TMacroFeature>(this IFeatureManager featMgr,
            string[] paramNames, int[] paramTypes, string[] paramValues,
            int[] dimTypes, double[] dimValues, object[] selection, object[] editBodies)
            where TMacroFeature : MacroFeatureEx
        {
            var options = swMacroFeatureOptions_e.swMacroFeatureByDefault;
            var provider = "";

            typeof(TMacroFeature).TryGetAttribute<OptionsAttribute>(a =>
            {
                options = a.Flags;
                provider = a.Provider;
            });

            var baseName = MacroFeatureInfo.GetBaseName<TMacroFeature>();

            var progId = MacroFeatureInfo.GetProgId<TMacroFeature>();

            if (string.IsNullOrEmpty(progId))
            {
                throw new NullReferenceException("Prog id for macro feature cannot be extracted");
            }

            var icons = MacroFeatureIconInfo.GetIcons(typeof(TMacroFeature), m_App.SupportsHighResIcons());

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