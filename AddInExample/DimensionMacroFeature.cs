using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.SwEx.MacroFeature.Example.Properties;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CodeStack.SwEx.MacroFeature.Base;
using CodeStack.SwEx.MacroFeature.Data;

namespace CodeStack.SwEx.MacroFeature.Example
{
    public class DimensionMacroFeatureParams
    {
        [ParameterDimension(swDimensionType_e.swLinearDimension, 0)]
        public double RefDimension { get; set; } = 0.01;

        [ParameterDimension(swDimensionType_e.swLinearDimension, 1)]
        public double RefCalcDimension { get; set; }
    }

    [ComVisible(true)]
    [Icon(typeof(Resources), nameof(Resources.codestack), "CodeStack\\MacroFeatureExample\\Icons")]
    public class DimensionMacroFeature : MacroFeatureEx<DimensionMacroFeatureParams>
    {
        protected override MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model,
            IFeature feature, DimensionMacroFeatureParams parameters)
        {
            parameters.RefCalcDimension = parameters.RefDimension * 2;
            var featData = feature.GetDefinition() as IMacroFeatureData;
            SetParameters(model, feature, featData, parameters);
            feature.Name = parameters.RefDimension.ToString();

            return MacroFeatureRebuildResult.FromStatus(true);
        }

        protected override void OnSetDimensions(ISldWorks app, IModelDoc2 model, IFeature feature, DimensionDataCollection dims, DimensionMacroFeatureParams parameters)
        {
            dims[0].Dimension.SetDirection(new Point(0, 0, 0), new Vector(0, 1, 0),
                parameters.RefDimension);

            dims[1].Dimension.SetDirection(new Point(0, 0, 0), new Vector(0, 0, 1),
                parameters.RefCalcDimension);
        }
    }
}
