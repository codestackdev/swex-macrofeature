//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.Common.Reflection;
using CodeStack.SwEx.MacroFeature.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Helpers
{
    internal static class MacroFeatureInfo
    {
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
    }
}
