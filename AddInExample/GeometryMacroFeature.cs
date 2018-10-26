using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.SwEx.MacroFeature.Example.Properties;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CodeStack.SwEx.MacroFeature.Base;
using CodeStack.SwEx.MacroFeature.Data;
using System.Runtime.Remoting.Contexts;

namespace CodeStack.SwEx.MacroFeature.Example
{
    [ComVisible(true)]
    [Icon(typeof(Resources), nameof(Resources.codestack), "CodeStack\\MacroFeatureExample\\Icons")]
    public class GeometryMacroFeature : MacroFeatureEx
    {
        protected override bool OnEditDefinition(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            model.Insert3DSketch2(true);
            model.SketchManager.AddToDB = true;
            model.SketchManager.CreatePoint(0.1, 0.2, 0.3);
            model.SketchManager.CreatePoint(-0.0605662432702594, 0.0894337567297407, -0.0288675134594813);
            model.SketchManager.CreatePoint(-0.0183012701892219, -0.0683012701892219, 0.0866025403784439);
            model.SketchManager.CreatePoint(0.0605662432702594, -0.0894337567297407, 0.0288675134594813);
            model.SketchManager.CreatePoint(0.0183012701892219, 0.0683012701892219, -0.0866025403784439);
            model.SketchManager.AddToDB = false;
            model.Insert3DSketch2(true);

            return true;
        }

        protected override MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model,
            IFeature feature)
        {
            //var body = app.IGetModeler().CreateBodyFromBox3(
            //    new double[]
            //    {
            //        0, 0, 0,
            //        0, 1, 0,
            //        0.1, 0.1, 0.1
            //    });

            var body = app.IGetModeler().CreateBox(new Point(0.1, 0.2, 0.3), new Vector(1, 1, 1), 0.1, 0.2, 0.3);

            return MacroFeatureRebuildResult.FromBody(body, feature.GetDefinition() as IMacroFeatureData);
        }
    }
}
