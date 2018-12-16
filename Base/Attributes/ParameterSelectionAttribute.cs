//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.sldworks;
using System;
using System.ComponentModel;

namespace CodeStack.SwEx.MacroFeature.Attributes
{
    /// <summary>
    /// Specifies that the current property is a selection of the macro feature.
    /// Selections allows to provide the relations between existing objects and this macro feature.
    /// Not only macro feature can access those selections to update itself, but the <see cref="MacroFeatureEx.OnRebuild(ISldWorks, IModelDoc2, IFeature)"/>
    /// will be automatically triggered every time the geometry of related selections is changed enabling the parametric nature of the macro feature
    /// </summary>
    /// <remarks>Supported property types are <see cref="object"/> (when selections can be of different entities) or a specific entity type of SOLIDWORKS (e.g. <see href="http://help.solidworks.com/2012/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.iface2.html">IFace2</see>)
    /// To specify multiple entities set the property type of <see cref="System.Collections.Generic.List{T}"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterSelectionAttribute : System.Attribute
    {
        /// <summary>
        /// Index of the selection in the selection array
        /// </summary>
        [Obsolete("This property is deprecated")]
        internal int SelectionIndex { get; private set; } = -1;

        /// <summary>
        /// Marks this property as a selection
        /// </summary>
        public ParameterSelectionAttribute()
        {
        }

        [Obsolete("This constructor is deprecated")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public ParameterSelectionAttribute(int selIndex)
        {
            SelectionIndex = selIndex;
        }
    }
}
