using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.SwEx.MacroFeature.Example.Properties;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CodeStack.SwEx.MacroFeature.Base;
using SolidWorks.Interop.swconst;
using CodeStack.SwEx.MacroFeature.Data;

namespace CodeStack.SwEx.MacroFeature.Example
{
    public class BoundingCylinderMacroFeatureParams
    {
        [ParameterEditBody(0)]
        public IBody2 InputBody { get; set; }

        [ParameterDimension(swDimensionType_e.swLinearDimension, 0)]
        public double ExtraHeight { get; set; }
    }

    [ComVisible(true)]
    [Icon(typeof(Resources), nameof(Resources.codestack), "CodeStack\\MacroFeatureExample\\Icons")]
    public class BoundingCylinderMacroFeature : MacroFeatureEx<BoundingCylinderMacroFeatureParams>
    {
        protected override MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model,
            IFeature feature, BoundingCylinderMacroFeatureParams parameters)
        {
            var modeler = app.IGetModeler();

            var box = parameters.InputBody.GetBodyBox() as double[];

            var center = new Point ((box[0] + box[3]) / 2, box[1], (box[2] + box[5]) / 2);
            var axis = new Vector(0, 1, 0);
            var radius = (box[3] - box[0]) / 2;
            var height = box[4] - box[1];

            var cyl = modeler.CreateCylinder(center, axis, radius, height);

            return MacroFeatureRebuildResult.FromBody(cyl, feature.GetDefinition() as IMacroFeatureData);
        }
    }
}
