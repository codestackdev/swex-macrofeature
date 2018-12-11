//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Attributes
{
    public class ParameterDimensionAttribute : Attribute
    {
        private static readonly swDimensionType_e[] m_SupportedTypes = new swDimensionType_e[]
        {
            swDimensionType_e.swAngularDimension,
            swDimensionType_e.swLinearDimension,
            swDimensionType_e.swRadialDimension
        };

        [Obsolete("This property is deprecated")]
        public int DimensionIndex { get; private set; } = -1;

        internal swDimensionType_e DimensionType { get; private set; }

        public ParameterDimensionAttribute(swDimensionType_e dimType)
        {
            if (!m_SupportedTypes.Contains(dimType))
            {
                throw new NotSupportedException($"Dimension {dimType} is not supported");
            }

            DimensionType = dimType;
        }

        [Obsolete("This constructor is deprecated")]
        public ParameterDimensionAttribute(swDimensionType_e dimType, int dimIndex) 
            : this(dimType)
        {
            DimensionIndex = dimIndex;
        }
    }
}
