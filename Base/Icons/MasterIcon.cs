//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.Common.Icons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Icons
{
    internal class MasterIcon : MacroFeatureIcon
    {
        internal MasterIcon(string baseName) : base(baseName)
        {
        }

        internal Image Icon { get; set; }

        public override MacroFeatureIcon Clone(string baseName)
        {
            return new MasterIcon(baseName)
            {
                Icon = Icon
            };
        }

        public override IEnumerable<IconSizeInfo> GetHighResolutionIconSizes()
        {
            yield return new IconSizeInfo(Icon, MacroFeatureIconInfo.SizeHighResSmall, BaseName);
            yield return new IconSizeInfo(Icon, MacroFeatureIconInfo.SizeHighResMedium, BaseName);
            yield return new IconSizeInfo(Icon, MacroFeatureIconInfo.SizeHighResLarge, BaseName);
        }

        public override IEnumerable<IconSizeInfo> GetIconSizes()
        {
            yield return new IconSizeInfo(Icon, MacroFeatureIconInfo.Size, BaseName);
        }
    }
}
