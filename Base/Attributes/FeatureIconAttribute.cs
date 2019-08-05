//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.Common.Reflection;
using CodeStack.SwEx.MacroFeature.Icons;
using System;
using System.Drawing;

namespace CodeStack.SwEx.MacroFeature.Attributes
{
    /// <summary>
    /// Specifies the icon for macro feature to be displayed in the Feature Manager Tree
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureIconAttribute : Attribute
    {
        internal MacroFeatureIcon Regular { get; private set; }
        internal MacroFeatureIcon Suppressed { get; private set; }
        internal MacroFeatureIcon Highlighted { get; private set; }

        internal string IconFolderName { get; private set; }

        /// <summary>
        /// Specifies the master icon for all macro feature states
        /// </summary>
        /// <param name="resType">Resource type</param>
        /// <param name="resName">Name of the resource representing the icon</param>
        /// <param name="iconFolderName">Folder name to store the icons.
        /// This folder will be created in the Program Data.
        /// If empty value is specified icon will be created at the default location</param>
        public FeatureIconAttribute(Type resType, string resName, string iconFolderName = "")
        {
            Regular = CreateIcon(resType, resName, MacroFeatureIconInfo.RegularName);
            Suppressed = CreateIcon(resType, resName, MacroFeatureIconInfo.SuppressedName);
            Highlighted = CreateIcon(resType, resName, MacroFeatureIconInfo.HighlightedName);

            IconFolderName = iconFolderName;
        }

        ///<inheritdoc cref="FeatureIconAttribute(Type, string, string)"/>
        /// <param name="smallResName">Name of the resource representing the small size icon</param>
        /// <param name="mediumResName">Name of the resource representing the medium size icon</param>
        /// <param name="largeResName">Name of the resource representing the large size icon</param>
        public FeatureIconAttribute(Type resType, string smallResName, string mediumResName,
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
