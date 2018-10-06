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
    /// <summary>
    /// Dimension data to add to macro feature
    /// </summary>
    //public class MacroFeatureDimension
    //{
    //    private static readonly swDimensionType_e[] m_SupportedTypes = new swDimensionType_e[]
    //    {
    //        swDimensionType_e.swAngularDimension,
    //        swDimensionType_e.swLinearDimension,
    //        swDimensionType_e.swRadialDimension
    //    };

    //    public swDimensionType_e Type { get; private set; }
    //    public double Value { get; private set; }

    //    public MacroFeatureDimension(swDimensionType_e type, double value)
    //    {
    //        if (!m_SupportedTypes.Contains(type))
    //        {
    //            throw new NotSupportedException($"Dimension {type} is not supported");
    //        }

    //        Type = type;
    //        Value = value;
    //    }
    //}

    /// <summary>
    /// Icons information to add to macro feature
    /// </summary>
    //public class MacroFeatureIcons
    //{
    //    public string Regular { get; private set; }
    //    public string Suppressed { get; private set; }
    //    public string Highlighted { get; private set; }

    //    public MacroFeatureIcons(string regular, string suppressed, string highlighted)
    //    {
    //        Regular = regular;
    //        Suppressed = suppressed;
    //        Highlighted = highlighted;
    //    }

    //    public MacroFeatureIcons(string icon) : this(icon, icon, icon)
    //    {
    //    }
    //}

    public static class FeatureManagerExtension
    {
        private static readonly MacroFeatureParametersParser m_ParamsParser;
        private static readonly IconsConverter m_IconsConverter;

        static FeatureManagerExtension()
        {
            m_ParamsParser = new MacroFeatureParametersParser();
            m_IconsConverter = new IconsConverter(System.Drawing.Color.White);
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
            where TMacroFeature : MacroFeatureEx<TParams>
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
            //MacroFeatureDimension[] dims = null;

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
            string baseName = "";
            var options = swMacroFeatureOptions_e.swMacroFeatureByDefault;

            typeof(TMacroFeature).TryGetAttribute<OptionsAttribute>(a =>
            {
                baseName = a.BaseName;
                options = a.Flags;
            });

            if (string.IsNullOrEmpty(baseName))
            {
                baseName = typeof(TMacroFeature).Name;
            }

            var progId = GetProgId<TMacroFeature>();

            if (string.IsNullOrEmpty(progId))
            {
                throw new NullReferenceException("Prog id for macro feature cannot be extracted");
            }

            //var icons = GetIcons<TMacroFeature>();
            var icons = MacroFeatureIconInfo.GetIcons(typeof(TMacroFeature), true);

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

        private static string GetProgId<TMacroFeature>()
            where TMacroFeature : MacroFeatureEx
        {
            string progId = "";

            if (!typeof(TMacroFeature).TryGetAttribute<ProgIdAttribute>(a => progId = a.Value))
            {
                progId = typeof(TMacroFeature).FullName;
            }

            return progId;
        }

        private static string[] GetIcons<TMacroFeature>()
            where TMacroFeature : MacroFeatureEx
        {
            IIcon regIcon = null;
            IIcon highIcon = null;
            IIcon suppIcon = null;

            typeof(TMacroFeature).TryGetAttribute<IconAttribute>(a =>
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

            var isHighRes = true;

            var icons = new List<string>();

            var regIconPahs = m_IconsConverter.ConvertIcon(regIcon, isHighRes);
            var suppIconPahs = m_IconsConverter.ConvertIcon(suppIcon, isHighRes);
            var highIconPahs = m_IconsConverter.ConvertIcon(highIcon, isHighRes);

            for (int i = 0; i < regIconPahs.Length; i++)
            {
                icons.Add(regIconPahs[i]);
                icons.Add(suppIconPahs[i]);
                icons.Add(highIconPahs[i]);
            }

            return icons.ToArray();
        }
    }
}