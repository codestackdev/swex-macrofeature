//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex-macrofeature
//**********************

using CodeStack.SwEx.Common.Icons;
using CodeStack.SwEx.Common.Reflection;
using CodeStack.SwEx.MacroFeature;
using CodeStack.SwEx.MacroFeature.Attributes;
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
    public static class FeatureManagerExtension
    {
        private static readonly MacroFeatureParametersParser m_ParamsParser;
        private static readonly IconsConverter m_IconsConverter;
        private static readonly ISldWorks m_App;

        static FeatureManagerExtension()
        {
            m_ParamsParser = new MacroFeatureParametersParser();
            m_IconsConverter = new IconsConverter();
            m_App = Context.CurrentApp;
        }

        /// <summary>
        /// Inserts new macro feature
        /// </summary>
        /// <typeparam name="TMacroFeature">Definition of COM macro feature</typeparam>
        /// <typeparam name="TParams">Type of parameters to serialize to macro feature</typeparam>
        /// <param name="featMgr">Pointer to Feature manager</param>
        /// <param name="parameters">Parameters to serialize to macro feature</param>
        /// <returns>Newly created feature</returns>
        public static IFeature InsertComFeature<TMacroFeature, TParams>(this IFeatureManager featMgr, TParams parameters)
            where TMacroFeature : MacroFeatureEx
            where TParams : class, new()
        {
            return InsertComFeatureWithParameters<TMacroFeature>(featMgr, parameters);
        }

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
                return InsertComFeature<TMacroFeature>(featMgr);
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

            m_ParamsParser.Parse(parameters,
                out paramNames, out paramTypes, out paramValues, out selection, out dimTypes, out dimValues);
            
            return InsertComFeatureBase<TMacroFeature>(featMgr, paramNames,
                paramTypes, paramValues, dimTypes, dimValues, selection);
        }

        private static IFeature InsertComFeatureBase<TMacroFeature>(this IFeatureManager featMgr,
            string[] paramNames = null, int[] paramTypes = null, string[] paramValues = null,
            int[] dimTypes = null, double[] dimValues = null, object[] selection = null)
            where TMacroFeature : MacroFeatureEx
        {
            var options = swMacroFeatureOptions_e.swMacroFeatureByDefault;

            typeof(TMacroFeature).TryGetAttribute<OptionsAttribute>(a =>
            {
                options = a.Flags;
            });

            var baseName = MacroFeatureEx.GetBaseName<TMacroFeature>();

            var progId = MacroFeatureEx.GetProgId<TMacroFeature>();

            if (string.IsNullOrEmpty(progId))
            {
                throw new NullReferenceException("Prog id for macro feature cannot be extracted");
            }

            var icons = MacroFeatureIconInfo.GetIcons(typeof(TMacroFeature), m_App.SupportsHighResIcons());

            using (var selSet = new SelectionGroup(featMgr.Document.ISelectionManager))
            {
                if (selection.Any())
                {
                    var selRes = selSet.AddRange(selection);

                    Debug.Assert(selRes);
                }

                var res = featMgr.InsertMacroFeature3(baseName,
                    progId, null, paramNames, paramTypes,
                    paramValues, dimTypes, dimValues, null, icons, (int)options) as IFeature;

                return res;
            }
        }
    }
}