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
using System.Runtime.CompilerServices;

namespace CodeStack.SwEx.MacroFeature.Example
{
    public class DimensionMacroFeatureParams
    {
        [ParameterDimension(swDimensionType_e.swLinearDimension)]
        public double RefDimension { get; set; } = 0.01;

        [ParameterDimension(swDimensionType_e.swLinearDimension)]
        public double RefCalcDimension { get; set; }

        [ParameterDimension(swDimensionType_e.swRadialDimension)]
        public double RefRadDimension { get; set; } = 0.025;

        public long DateTimeStamp { get; set; }
    }
    
    [ComVisible(true)]
    public class DimensionMacroFeature : MacroFeatureEx<DimensionMacroFeatureParams>
    {
        protected override MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model,
            IFeature feature, DimensionMacroFeatureParams parameters)
        {
            parameters.RefCalcDimension = parameters.RefDimension * 2;
            parameters.DateTimeStamp = DateTime.Now.Ticks;

            SetParameters(model, feature, feature.GetDefinition() as IMacroFeatureData, parameters);
            feature.Name = parameters.RefDimension.ToString();

            return MacroFeatureRebuildResult.FromStatus(true);
        }

        protected override void OnSetDimensions(ISldWorks app, IModelDoc2 model, IFeature feature,
            MacroFeatureRebuildResult rebuildResult, DimensionDataCollection dims, DimensionMacroFeatureParams parameters)
        {
            dims[0].SetOrientation(new Point(0, 0, 0), new Vector(0, 1, 0));
            dims[1].SetOrientation(new Point(0, 0, 0), new Vector(0, 0, 1));
            dims[2].SetOrientation(new Point(0, 0, 0), new Vector(1, 0, 0));
        }
    }
}
