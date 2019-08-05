//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.Common.Icons;
using CodeStack.SwEx.Common.Reflection;
using CodeStack.SwEx.MacroFeature.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Icons
{
    internal static class MacroFeatureIconInfo
    {
        internal const string DEFAULT_ICON_FOLDER = "CodeStack\\SwEx.MacroFeature\\{0}\\Icons";
        internal const string RegularName = "Regular";
        internal const string SuppressedName = "Suppressed";
        internal const string HighlightedName = "Highlighted";

        internal static Size Size { get; private set; } = new Size(16, 18);
        internal static Size SizeHighResSmall { get; private set; } = new Size(20, 20);
        internal static Size SizeHighResMedium { get; private set; } = new Size(32, 32);
        internal static Size SizeHighResLarge { get; private set; } = new Size(40, 40);

        internal static string GetLocation(Type macroFeatType)
        {
            var iconFolderName = "";

            macroFeatType.TryGetAttribute<FeatureIconAttribute>(a => iconFolderName = a.IconFolderName);

            if (string.IsNullOrEmpty(iconFolderName))
            {
                iconFolderName = string.Format(DEFAULT_ICON_FOLDER, macroFeatType.FullName);
            }

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                iconFolderName);
        }

        internal static string[] GetIcons(Type macroFeatType, bool highRes)
        {
            var loc = GetLocation(macroFeatType);

            if (highRes)
            {
                return new string[]
                {
                    Path.Combine(loc, IconSizeInfo.CreateFileName(RegularName, SizeHighResSmall)),
                    Path.Combine(loc, IconSizeInfo.CreateFileName(SuppressedName, SizeHighResSmall)),
                    Path.Combine(loc, IconSizeInfo.CreateFileName(HighlightedName, SizeHighResSmall)),
                    Path.Combine(loc, IconSizeInfo.CreateFileName(RegularName, SizeHighResMedium)),
                    Path.Combine(loc, IconSizeInfo.CreateFileName(SuppressedName, SizeHighResMedium)),
                    Path.Combine(loc, IconSizeInfo.CreateFileName(HighlightedName, SizeHighResMedium)),
                    Path.Combine(loc, IconSizeInfo.CreateFileName(RegularName, SizeHighResLarge)),
                    Path.Combine(loc, IconSizeInfo.CreateFileName(SuppressedName, SizeHighResLarge)),
                    Path.Combine(loc, IconSizeInfo.CreateFileName(HighlightedName, SizeHighResLarge))
                };
            }
            else
            {
                return new string[]
                {
                    Path.Combine(loc, IconSizeInfo.CreateFileName(RegularName, Size)),
                    Path.Combine(loc, IconSizeInfo.CreateFileName(SuppressedName, Size)),
                    Path.Combine(loc, IconSizeInfo.CreateFileName(HighlightedName, Size))
                };
            }
        }
    }
}
