//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex-macrofeature
//**********************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;

namespace CodeStack.SwEx.MacroFeature.Attributes
{
    /// <summary>
    /// Attribute decorates public property to indicate that this parameter needs to be retrieved from macro feature selection
    /// (i.e. <see cref="IMacroFeatureData.GetSelections3(out object, out object, out object, out object, out object)"/> method
    /// </summary>
    /// <remarks>This attribute is extracted in <see cref="MacroFeatureDataExtension.DeserializeParameters{TParams}(IMacroFeatureData)"/>
    /// and <see cref="MacroFeatureDataExtension.SerializeParameters{TParams}(IMacroFeatureData, TParams)"/> methods</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterSelectionAttribute : System.Attribute
    {
        /// <summary>
        /// Index of the selection in the selection array
        /// </summary>
        public int SelectionIndex { get; private set; }

        public ParameterSelectionAttribute(int selIndex)
        {
            SelectionIndex = selIndex;
        }
    }
}
