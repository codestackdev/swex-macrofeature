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

namespace CodeStack.SwEx.MacroFeature.Example
{
    public class DimensionMacroFeatureParams
    {
        [ParameterDimension(swDimensionType_e.swLinearDimension, 0)]
        public double RefDimension { get; set; }

        [ParameterDimension(swDimensionType_e.swLinearDimension, 1)]
        public double RefCalcDimension { get; set; }
    }

    [ComVisible(true)]
    [Icon(typeof(Resources), nameof(Resources.codestack), "CodeStack\\MacroFeatureExample\\Icons")]
    public class DimensionMacroFeature : MacroFeatureEx<DimensionMacroFeatureParams>
    {
        private ISldWorks m_App;//temp

        protected override MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            m_App = app;

            var featData = feature.GetDefinition() as IMacroFeatureData;
            var featParams = GetParameters(featData);
            featParams.RefCalcDimension = featParams.RefDimension * 2;
            SetParameters(featData, featParams);
            feature.Name = featParams.RefDimension.ToString();
            
            return MacroFeatureRebuildResult.FromStatus(true);
        }

        protected override void OnSetDimension(IDisplayDimension dispDim, int index, double value)
        {
            if (index == 0)
            {
                dispDim.SetDimensionPosition(new Structs.Point(0, 0, 0), new Structs.Vector(0, 1, 0), value, m_App.IGetMathUtility());
            }
            else if (index == 1)
            {
                dispDim.SetDimensionPosition(new Structs.Point(0, 0, 0), new Structs.Vector(0, 0, 1), value, m_App.IGetMathUtility());
            }
        }
    }
}
