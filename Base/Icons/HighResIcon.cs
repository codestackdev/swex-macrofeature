//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
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
    internal class HighResIcon : MacroFeatureIcon
    {
        internal HighResIcon(string baseName) : base(baseName)
        {
        }

        public Image Small { get; set; }
        public Image Medium { get; set; }
        public Image Large { get; set; }
                
        public override IEnumerable<IconSizeInfo> GetHighResolutionIconSizes()
        {
            yield return new IconSizeInfo(Small, MacroFeatureIconInfo.SizeHighResSmall, BaseName);
            yield return new IconSizeInfo(Medium, MacroFeatureIconInfo.SizeHighResMedium, BaseName);
            yield return new IconSizeInfo(Large, MacroFeatureIconInfo.SizeHighResLarge, BaseName);
        }

        public override IEnumerable<IconSizeInfo> GetIconSizes()
        {
            yield return new IconSizeInfo(Small, MacroFeatureIconInfo.Size, BaseName);
        }

        public override MacroFeatureIcon Clone(string baseName)
        {
            return new HighResIcon(baseName)
            {
                Large = Large,
                Medium = Medium,
                Small = Small
            };
        }
    }
}
