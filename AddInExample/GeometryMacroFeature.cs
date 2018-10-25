using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.SwEx.MacroFeature.Example.Properties;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CodeStack.SwEx.MacroFeature.Base;

namespace CodeStack.SwEx.MacroFeature.Example
{
    [ComVisible(true)]
    [Icon(typeof(Resources), nameof(Resources.codestack), "CodeStack\\MacroFeatureExample\\Icons")]
    public class GeometryMacroFeature : MacroFeatureEx
    {
        protected override MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model,
            IFeature feature)
        {
            var body = app.IGetModeler().CreateBodyFromBox3(
                new double[]
                {
                    0, 0, 0,
                    0, 1, 0,
                    0.1, 0.1, 0.1
                });

            return MacroFeatureRebuildResult.FromBody(body);
        }
    }
}
