//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.Common.Icons;
using CodeStack.SwEx.Common.Reflection;
using CodeStack.SwEx.MacroFeature.Icons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IconAttribute : Attribute
    {
        internal IIcon Regular { get; private set; }
        internal IIcon Suppressed { get; private set; }
        internal IIcon Highlighted { get; private set; }

        internal string IconFolderName { get; private set; }

        public IconAttribute(Type resType, string resName, string iconFolderName = "")
        {
            Regular = CreateIcon(resType, resName, MacroFeatureIconInfo.RegularName);
            Suppressed = CreateIcon(resType, resName, MacroFeatureIconInfo.SuppressedName);
            Highlighted = CreateIcon(resType, resName, MacroFeatureIconInfo.HighlightedName);

            IconFolderName = iconFolderName;
        }
        
        public IconAttribute(Type resType, string smallResName, string mediumResName,
            string largeResName, string iconFolderName = "")
        {
            Regular = CreateIcon(resType, smallResName, mediumResName, largeResName, MacroFeatureIconInfo.RegularName);
            Suppressed = CreateIcon(resType, smallResName, mediumResName, largeResName, MacroFeatureIconInfo.SuppressedName);
            Highlighted = CreateIcon(resType, smallResName, mediumResName, largeResName, MacroFeatureIconInfo.HighlightedName);

            IconFolderName = iconFolderName;
        }

        //TODO: add another 2 constructors for specifying suppressed and highlighted

        private MasterIcon CreateIcon(Type resType, string regName, string baseName)
        {
            return new MasterIcon(baseName)
            {
                Icon = ResourceHelper.GetResource<Image>(resType, regName)
            };
        }

        private HighResIcon CreateIcon(Type resType, string smallResName, string mediumResName,
            string largeResName, string baseName)
        {
            return new HighResIcon(baseName)
            {
                Small = ResourceHelper.GetResource<Image>(resType, smallResName),
                Medium = ResourceHelper.GetResource<Image>(resType, mediumResName),
                Large = ResourceHelper.GetResource<Image>(resType, largeResName)
            };
        }
    }
}
