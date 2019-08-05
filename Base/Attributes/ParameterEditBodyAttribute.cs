//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using System;
using System.ComponentModel;

namespace CodeStack.SwEx.MacroFeature.Attributes
{
    /// <summary>
    /// Specifies that the current property is an edit body of the macro feature.
    /// Edit bodies are used by macro feature if it is required to modify or replace any existing bodies.
    /// Edit bodies will be acquire by macro feature and replaced by the <see cref="Base.MacroFeatureRebuildBodyResult"/>
    /// returned from <see cref="MacroFeatureEx.OnRebuild(SolidWorks.Interop.sldworks.ISldWorks, SolidWorks.Interop.sldworks.IModelDoc2, SolidWorks.Interop.sldworks.IFeature)"/>.
    /// Multiple bodies are supported
    /// </summary>
    /// <remarks>Supported property type is <see href="http://help.solidworks.com/2012/English/api/sldworksapi/SOLIDWORKS.Interop.sldworks~SOLIDWORKS.Interop.sldworks.IBody2.html">IBody2</see>
    /// or <see cref="System.Collections.Generic.List{T}"/> of bodies</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterEditBodyAttribute : Attribute
    {
    }
}
