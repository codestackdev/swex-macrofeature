[![Documentation](https://img.shields.io/badge/-Documentation-green.svg)](https://www.codestack.net/labs/solidworks/swex/macro-feature/)
[![NuGet](https://img.shields.io/nuget/v/CodeStack.SwEx.MacroFeature.svg)](https://www.nuget.org/packages/CodeStack.SwEx.MacroFeature/)
[![Issues](https://img.shields.io/github/issues/codestack-net-dev/swex-macrofeature.svg)](https://github.com/codestack-net-dev/swex-macrofeature/issues)

# SwEx.MacroFeature
![SwEx.MacroFeature](https://www.codestack.net/labs/solidworks/swex/macro-feature/logo.png)
SwEx.MacroFeature enables SOLIDWORKS add-in developers to develop applications utilizing the macro feature functionality.

## Getting started
Create a public COM visible class for macro feature. Inherit one of the [MacroFeatureEx](https://docs.codestack.net/swex/macro-feature/html/N_CodeStack_SwEx_MacroFeature.htm) class overloads.

It is recommended to explicitly assign guid and prog id for the macro feature.

Optionally assign additional [options](https://docs.codestack.net/swex/macro-feature/html/T_CodeStack_SwEx_MacroFeature_Attributes_OptionsAttribute.htm) and [icon](https://docs.codestack.net/swex/macro-feature/html/T_CodeStack_SwEx_MacroFeature_Attributes_IconAttribute.htm)

~~~ cs
    [ComVisible(true)]
    [Guid("47827004-8897-49F5-9C65-5B845DC7F5AC")]
    [ProgId(Id)]
    [Options("CodeStack.MyMacroFeature", swMacroFeatureOptions_e.swMacroFeatureAlwaysAtEnd)]
    [Icon(typeof(Resources), nameof(Resources.macro_feature_icon), "CodeStack\\MyMacroFeature\\Icons")]
    public class MyMacroFeature : MacroFeatureEx<RoundStockFeatureParameters>
    {

    }
~~~

[Read More...](https://www.codestack.net/labs/solidworks/swex/macro-feature/getting-started)

## Lifecycle
Macro feature resides in the model and saved together with the document. Macro feature can handle various events during its lifecycle

* Regeneration
* Editing
* Updating state

Macro feature is a singleton service. Do not create any class level variables in the macro feature class. If it is required to track the lifecycle of particular macro feature use [Feature Handler](#feature-handler).

[Read More...](https://www.codestack.net/labs/solidworks/swex/macro-feature/lifecycle)

### Regeneration
This handler called when feature is being rebuilt (either when regenerate is invoked or when the parent elements have been changed).

Use [MacroFeatureRebuildResult](https://docs.codestack.net/swex/macro-feature/html/T_CodeStack_SwEx_MacroFeature_Base_MacroFeatureRebuildResult.htm) class to generate the required output.

Feature can generate the following output

* None
* Rebuild Error
* Solid or surface body
* Array of solid or surface bodies

Use [IModeler](http://help.solidworks.com/2017/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.imodeler.html) interface if feature needs to create new bodies. Only temp bodies can be returned from the regeneration method.

~~~ cs
protected override MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model, IFeature feature, MyParameters parameters)
{
    var body = GenerateBodyFromParameters(parameters);
    return MacroFeatureRebuildResult.FromBody(body, feature.GetDefinition() as IMacroFeatureData);
}
~~~

[Read More...](https://www.codestack.net/labs/solidworks/swex/macro-feature/lifecycle/regeneration)

### Edit Definition

Edit definition allows to modify the parameters of an existing feature. Edit definition is called when *Edit Feature* command is clicked form the feature manager tree.

![Edit Feature Command](https://www.codestack.net/labs/solidworks/swex/macro-feature/lifecycle/edit-definition/menu-edit-feature.png)

~~~ cs
protected override bool OnEditDefinition(ISldWorks app, IModelDoc2 model, IFeature feature)
{
    var featData = feature.GetDefinition() as IMacroFeatureData;

    //rollback feature
    featData.AccessSelections(model, null);

    //read current parameters
    var parameters = GetParameters(feature, featData, model);

    //Show property page or any other user interface
    var res = ShowPage(parameters);

    if (res)
    {
        //set parameters and update feature data
        SetParameters(model, feature, featData, parameters);
        feature.ModifyDefinition(featData, part, null);
    }
    else
    {
        //cancel modifications
        featData.ReleaseSelectionAccess();
    }

    return true;
}
~~~

It is important to use the same pointer to [IMacroFeatureData](http://help.solidworks.com/2016/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.imacrofeaturedata.html) while calling the [IMacroFeatureData::AccessSelections](http://help.solidworks.com/2016/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IMacroFeatureData~AccessSelections.html), [GetParameters](https://docs.codestack.net/swex/macro-feature/html/M_CodeStack_SwEx_MacroFeature_MacroFeatureEx_1_GetParameters.htm), [SetParameters](https://docs.codestack.net/swex/macro-feature/html/M_CodeStack_SwEx_MacroFeature_MacroFeatureEx_1_SetParameters.htm) and [IFeature::ModifyDefinition](http://help.solidworks.com/2016/english/api/sldworksapi/SOLIDWORKS.Interop.sldworks~SOLIDWORKS.Interop.sldworks.IFeature~ModifyDefinition.html) methods.

[Read More...](https://www.codestack.net/labs/solidworks/swex/macro-feature/lifecycle/edit-definition)

### State
This handler is called everytime state of the feature is changed. It should be used to provide additional security for macro feature

~~~ cs
protected override swMacroFeatureSecurityOptions_e OnUpdateState(ISldWorks app, IModelDoc2 model, IFeature feature)
{
    //disallow editing or suppressing of the feature
    return swMacroFeatureSecurityOptions_e.swMacroFeatureSecurityCannotBeDeleted 
                | swMacroFeatureSecurityOptions_e.swMacroFeatureSecurityCannotBeSuppressed;
}
~~~

[Read More...](https://www.codestack.net/labs/solidworks/swex/macro-feature/lifecycle/state)

### Feature Handler

[MacroFeatureEx<TParams, THandler> Class]() overload of macro feature allows defining the handler class which will be created for each feature. This provides a simple way to track the macro feature lifecycle (i.e. creation time and deletion time).

~~~ cs
public class LifecycleMacroFeatureParams
{
}

public class LifecycleMacroFeatureHandler : IMacroFeatureHandler
{
    public void Init(ISldWorks app, IModelDoc2 model, IFeature feat)
    {
        //feature is created or loaded
    }

    public void Unload()
    {
        //feature is deleted or model is closed
    }
}

[ComVisible(true)]
public class LifecycleMacroFeature : MacroFeatureEx<LifecycleMacroFeatureParams, LifecycleMacroFeatureHandler>
{
    protected override MacroFeatureRebuildResult OnRebuild(LifecycleMacroFeatureHandler handler, LifecycleMacroFeatureParams parameters)
    {
        return MacroFeatureRebuildResult.FromStatus(true);
    }
}
~~~

[Read More...](https://www.codestack.net/labs/solidworks/swex/macro-feature/lifecycle/feature-handler)

## Data

Macro feature can store additonal metadata and entities. The data includes

* Parameters
* Selections
* Edit bodies
* Dimensions

Required data can be defined within the macro feature data model. Special parameters (such as selections, edit bodies or dimensions) should be decorated with appropriate [attributes](https://docs.codestack.net/swex/macro-feature/html/N_CodeStack_SwEx_MacroFeature_Attributes.htm), all other proeprties will be considered as parameters.

[Read More...](https://www.codestack.net/labs/solidworks/swex/macro-feature/data)

### Parameters

~~~ cs
public class MacroFeatureParams
{
    public string Parameter1 { get; set; }
    public int Parameter2 { get; set; }
}

//this macro feature has two parameters (Parameter1 and Parameter2)
[ComVisible(true)]
public class MyMacroFeature : MacroFeatureEx<MacroFeatureParams>
{
}
~~~

[Read More...](https://www.codestack.net/labs/solidworks/swex/macro-feature/data/parameters)

### Selections

~~~ cs
public class MacroFeatureParams
{
    //selection parameter of any entity (e.g. face, edge, feature etc.)
    [ParameterSelection]
    public object AnyEntity { get; set; }

    //selection parameter of body
    [ParameterSelection]
    public IBody2 Body { get; set; }

    //selection parameter of array of faces
    [ParameterSelection]
    public List<IFace2> Faces { get; set; }
~~~

Parameter proeprties can be specified either using the direct SOLIDWORKS type or as object if type is unknown. List of selections is also supported.

[OnRebuild](https://docs.codestack.net/swex/macro-feature/html/M_CodeStack_SwEx_MacroFeature_MacroFeatureEx_OnRebuild.htm) handler will be called if any of the selections have changed.

[Read More...](https://www.codestack.net/labs/solidworks/swex/macro-feature/data/selections)

### Edit Bodies

Edit bodies are input bodies which macro feature will acquire. For example when boss-extrude feature is created using the merge bodies option the solid body it is based on became a body of the new boss-extrude. This could be validated by selecting the feature in the tree which will select the body as well. In this case the original body was passed as an edit body to the boss-extrude feature.

~~~ cs
public class MacroFeatureParams
{
    [ParameterEditBody]
    public IBody2 InputBody { get; set; }
}
~~~

[Read More...](https://www.codestack.net/labs/solidworks/swex/macro-feature/data/edit-bodies)

### Dimensions
Dimensions is an additional source of input for macro feature. Dimensions can be defined in the follwing way:

~~~ cs
public class DimensionMacroFeatureParams
{
    [ParameterDimension(swDimensionType_e.swLinearDimension)]
    public double FirstDimension { get; set; } = 0.01;

    [ParameterDimension(swDimensionType_e.swLinearDimension)]
    public double SecondDimension { get; set; }
}
~~~

It is required to arrange the dimensions after rebuild by overriding the [OnSetDimensions](https://docs.codestack.net/swex/macro-feature/html/M_CodeStack_SwEx_MacroFeature_MacroFeatureEx_1_OnSetDimensions.htm) method. Use [SetDirection](https://docs.codestack.net/swex/macro-feature/html/M_SolidWorks_Interop_sldworks_DimensionEx_SetDirection.htm) helper method to align the dimension.

~~~ cs
protected override void OnSetDimensions(ISldWorks app, IModelDoc2 model, IFeature feature, DimensionDataCollection dims, DimensionMacroFeatureParams parameters)
{
    dims[0].Dimension.SetDirection(new Point(0, 0, 0), new Vector(0, 1, 0),
        parameters.FirstDimension);

    dims[1].Dimension.SetDirection(new Point(0, 0, 0), new Vector(0, 0, 1),
        parameters.SecondDimension);
}
~~~

[Read More...](https://www.codestack.net/labs/solidworks/swex/macro-feature/data/dimensions)

### Backward Compatibility

Macro feature parameters might need to change from version to version. And SwEx.MacroFeature framework provides a mechanism to handle the backward compatibility of existing features.

Mark current version of parameters with [ParametersVersionAttribute](https://docs.codestack.net/swex/macro-feature/html/T_CodeStack_SwEx_MacroFeature_Attributes_ParametersVersionAttribute.htm) and increase the version if any of the parameters changed.

Implement the [Paramater Version Converter](https://docs.codestack.net/swex/macro-feature/html/T_CodeStack_SwEx_MacroFeature_Base_IParametersVersionConverter.htm) to convert from the latest version of the parameters to the nevest one. Framework will take care of aligning versions in case parameters are older than one version.

Old version of parameters
~~~ cs
[ParametersVersion("1.0", typeof(MacroFeatureParamsVersionConverter))]
public class MacroFeatureParams
{
    public string Param1 { get; set; }
    public int Param2 { get; set; }
}
~~~

New version of parameters

~~~ cs
[ParametersVersion("2.0", typeof(MacroFeatureParamsVersionConverter))]
public class MacroFeatureParams
{
    public string Param1A { get; set; }//parameter renamed
    public int Param2 { get; set; }
    public string Param3 { get; set; }//new parameter added
}

public class MacroFeatureParamsVersionConverter : ParametersVersionConverter
{
    private class VersConv_1_0To2_0 : ParameterConverter
    {
        public override Dictionary<string, string> ConvertParameters(IModelDoc2 model, IFeature feat, Dictionary<string, string> parameters)
        {
            var paramVal = parameters["Param1"];
            parameters.Remove("Param1");
            parameters.Add("Param1A", paramVal);//renaming parameter
            parameters.Add("Param3", "Default");//adding new parameter with default value
            return parameters;
        }
    }

    public ParamsMacroFeatureParamsVersionConverter()
    {
        //conversion from version 1.0 to 2.0
        Add(new Version("2.0"), new VersConv_1_0To2_0());
        //TODO: add more version converters
    }
}
~~~


[Read More...](https://www.codestack.net/labs/solidworks/swex/macro-feature/data/backward-compatibility)