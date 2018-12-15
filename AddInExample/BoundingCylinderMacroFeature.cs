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
        [ParameterEditBody]
        public IBody2 InputBody { get; set; }

        [ParameterDimension(swDimensionType_e.swLinearDimension)]
        public double ExtraHeight { get; set; } = 0.01;

        public bool AddHeight { get; set; }
    }

    [ComVisible(true)]
    [Icon(typeof(Resources), nameof(Resources.codestack), "CodeStack\\MacroFeatureExample\\Icons")]
    public class BoundingCylinderMacroFeature : MacroFeatureEx<BoundingCylinderMacroFeatureParams>
    {
        protected override bool OnEditDefinition(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            var featData = feature.GetDefinition() as IMacroFeatureData;

            featData.AccessSelections(model, (feature as IEntity).GetComponent());

            var parameters = GetParameters(feature, featData, model);

            var addHeight = parameters.AddHeight;

            var newAddHeight = app.SendMsgToUser2("Add extra height (Yes) or remove extra height (No)?", (int)swMessageBoxIcon_e.swMbQuestion, (int)swMessageBoxBtn_e.swMbYesNo) == (int)swMessageBoxResult_e.swMbHitYes;

            if (newAddHeight != addHeight)
            {
                parameters.AddHeight = newAddHeight;

                SetParameters(model, feature, featData, parameters);

                feature.ModifyDefinition(featData, model, (feature as IEntity).GetComponent());
            }
            else
            {
                featData.ReleaseSelectionAccess();
            }
            
            return true;
        }

        protected override MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model,
            IFeature feature, BoundingCylinderMacroFeatureParams parameters)
        {
            var modeler = app.IGetModeler();

            Point center;
            Vector axis;
            double radius;
            double height;
            double extraHeight;

            GetCylinderParameters(parameters, out center, out axis, out radius, out height, out extraHeight);

            var cyl = modeler.CreateCylinder(center, axis, radius, height + extraHeight);

            return MacroFeatureRebuildResult.FromBody(cyl, feature.GetDefinition() as IMacroFeatureData);
        }

        protected override void OnSetDimensions(ISldWorks app, IModelDoc2 model, IFeature feature, DimensionDataCollection dims, BoundingCylinderMacroFeatureParams parameters)
        {
            Point center;
            Vector axis;
            double radius;
            double height;
            double extraHeight;

            GetCylinderParameters(parameters, out center, out axis, out radius, out height, out extraHeight);

            var dimOrig = center.Move(axis, height);

            var dimDir = new Vector(axis);

            dims[0].Dimension.SetDirection(dimOrig, dimDir, Math.Abs(extraHeight));
        }

        private static void GetCylinderParameters(BoundingCylinderMacroFeatureParams parameters, out Point center, out Vector axis,
            out double radius, out double height, out double extraHeight)
        {
            var box = parameters.InputBody.GetBodyBox() as double[];

            center = new Point((box[0] + box[3]) / 2, box[1], (box[2] + box[5]) / 2);
            axis = new Vector(0, 1, 0);
            radius = (box[3] - box[0]) / 2;
            height = box[4] - box[1];
            extraHeight = parameters.ExtraHeight;

            if (!parameters.AddHeight)
            {
                extraHeight *= -1;
            }
        }
    }
}
