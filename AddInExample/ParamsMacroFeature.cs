using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.SwEx.MacroFeature.Example.Properties;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Example
{
    public class ParamsMacroFeatureParams
    {
        public string Param1 { get; set; }
        public int EditDefinitionsCount { get; set; }
    }

    [ComVisible(true)]
    [Icon(typeof(Resources), nameof(Resources.codestack), "CodeStack\\MacroFeatureExample\\Icons")]
    public class ParamsMacroFeature : MacroFeatureEx<ParamsMacroFeatureParams>
    {
        protected override bool OnEditDefinition(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            var featData = feature.GetDefinition() as IMacroFeatureData;

            featData.AccessSelections(model, null);

            var featParams = GetParameters(featData);

            app.SendMsgToUser($"{nameof(featParams.Param1)} = {featParams.Param1}; {nameof(featParams.EditDefinitionsCount)} = {featParams.EditDefinitionsCount}");
            featParams.EditDefinitionsCount = featParams.EditDefinitionsCount + 1;

            SetParameters(featData, featParams);

            feature.ModifyDefinition(featData, model, null);

            return true;
        }
    }
}
