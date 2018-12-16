using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.SwEx.MacroFeature.Base;
using CodeStack.SwEx.MacroFeature.Example.Properties;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;

namespace CodeStack.SwEx.MacroFeature.Example
{
    public class ParamsMacroFeatureParamsVersionConverter : ParametersVersionConverter
    {
        private class VersConv_0_0To1_0 : ParameterConverter
        {
            public override Dictionary<string, string> ConvertParameters(IModelDoc2 model, IFeature feat, Dictionary<string, string> parameters)
            {
                var paramVal = parameters["Param1"];
                parameters.Remove("Param1");
                parameters.Add("Param2", paramVal);

                return parameters;
            }
        }

        public ParamsMacroFeatureParamsVersionConverter()
        {
            Add(new Version("1.0"), new VersConv_0_0To1_0());
        }
    }

    //public class ParamsMacroFeatureParams
    //{
    //    public string Param1 { get; set; }
    //    public int EditDefinitionsCount { get; set; }
    //}

    [ParametersVersion("1.0", typeof(ParamsMacroFeatureParamsVersionConverter))]
    public class ParamsMacroFeatureParams
    {
        public string Param2 { get; set; }
        public int EditDefinitionsCount { get; set; }
    }

    [ComVisible(true)]
    [Icon(typeof(Resources), nameof(Resources.codestack), "CodeStack\\MacroFeatureExample\\Icons")]
    [Options("SwEx.MacroFeature.Params", "CodeStack. Macro feature definition is not registered")]
    public class ParamsMacroFeature : MacroFeatureEx<ParamsMacroFeatureParams>
    {
        protected override bool OnEditDefinition(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            var featData = feature.GetDefinition() as IMacroFeatureData;

            featData.AccessSelections(model, null);

            var featParams = GetParameters(feature, featData, model);

            app.SendMsgToUser($"{nameof(featParams.Param2)} = {featParams.Param2}; {nameof(featParams.EditDefinitionsCount)} = {featParams.EditDefinitionsCount}");
            featParams.EditDefinitionsCount = featParams.EditDefinitionsCount + 1;

            SetParameters(model, feature, featData, featParams);

            feature.ModifyDefinition(featData, model, null);

            return true;
        }
    }
}
