//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.Common.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeStack.SwEx.MacroFeature.Icons
{
    internal abstract class MacroFeatureIcon : IIcon
    {
        public Color TransparencyKey
        {
            get
            {
                return Color.White;
            }
        }

        internal string BaseName { get; private set; }

        protected MacroFeatureIcon(string baseName)
        {
            BaseName = baseName;
        }

        public abstract IEnumerable<IconSizeInfo> GetHighResolutionIconSizes();

        public abstract IEnumerable<IconSizeInfo> GetIconSizes();

        public abstract MacroFeatureIcon Clone(string baseName);
    }
}
