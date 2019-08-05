//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Attributes
{
    /// <summary>
    /// Specifies that the current property is a dimension of the macro feature.
    /// The value if the property is the current value of the dimension.
    /// This property is bi-directional: it will update the value of the dimension
    /// when changed within the <see cref="MacroFeatureEx.OnRebuild(SolidWorks.Interop.sldworks.ISldWorks, SolidWorks.Interop.sldworks.IModelDoc2, SolidWorks.Interop.sldworks.IFeature)"/> 
    /// as well as will contain the current value of the dimension when it got modified by the user in the 
    /// graphics area
    /// </summary>
    /// <remarks>This should only be used for numeric properties</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterDimensionAttribute : Attribute
    {
        private static readonly swDimensionType_e[] m_SupportedTypes = new swDimensionType_e[]
        {
            swDimensionType_e.swAngularDimension,
            swDimensionType_e.swLinearDimension,
            swDimensionType_e.swRadialDimension
        };
        
        internal swDimensionType_e DimensionType { get; private set; }

        /// <summary>
        /// Marks this property as dimension and assigns the dimension type
        /// </summary>
        /// <param name="dimType">Type of the dimension as defined in <see href="http://help.solidworks.com/2016/English/api/swconst/SolidWorks.Interop.swconst~SolidWorks.Interop.swconst.swDimensionType_e.html">swDimensionType_e enumeration</see></param>
        public ParameterDimensionAttribute(swDimensionType_e dimType)
        {
            if (!m_SupportedTypes.Contains(dimType))
            {
                throw new NotSupportedException($"Dimension {dimType} is not supported");
            }

            DimensionType = dimType;
        }
    }
}
