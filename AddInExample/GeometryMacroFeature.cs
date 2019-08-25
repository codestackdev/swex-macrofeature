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
    [Common.Attributes.Icon(typeof(Resources), nameof(Resources.codestack))]
    public class GeometryMacroFeature : MacroFeatureEx
    {
        protected override bool OnEditDefinition(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            return true;
        }

        protected override MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model,
            IFeature feature)
        {
            var body = app.IGetModeler().CreateBox(new Point(0.1, 0.2, 0.3), new Vector(0, 0, 1), 0.1, 0.2, 0.3);

            return MacroFeatureRebuildResult.FromBody(body, feature.GetDefinition() as IMacroFeatureData);
        }
    }
}
