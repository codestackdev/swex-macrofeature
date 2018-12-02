using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Base
{
    public interface IParametersVersionConverter : IDictionary<Version, IParameterConverter>
    {
    }

    public class ParametersVersionConverter : Dictionary<Version, IParameterConverter>, IParametersVersionConverter
    {
        public ParametersVersionConverter() : base()
        {
        }

        public ParametersVersionConverter(IDictionary<Version, IParameterConverter> dictionary) 
            : base(dictionary)
        {
        }
    }

    public interface IParameterConverter
    {
        IBody2[] ConvertEditBodies(IModelDoc2 model, IFeature feat, IBody2[] editBodies);
        IDisplayDimension[] ConvertDisplayDimensions(IModelDoc2 model, IFeature feat, IDisplayDimension[] dispDims);
        Dictionary<string, string> ConvertParameters(IModelDoc2 model, IFeature feat, Dictionary<string, string> parameters);
        object[] ConvertSelections(IModelDoc2 model, IFeature feat, object[] selection);
    }

    public class ParameterConverter : IParameterConverter
    {
        public virtual IDisplayDimension[] ConvertDisplayDimensions(IModelDoc2 model, IFeature feat, IDisplayDimension[] dispDims)
        {
            return dispDims;
        }

        public virtual IBody2[] ConvertEditBodies(IModelDoc2 model, IFeature feat, IBody2[] editBodies)
        {
            return editBodies;
        }

        public virtual Dictionary<string, string> ConvertParameters(IModelDoc2 model, IFeature feat, Dictionary<string, string> parameters)
        {
            return parameters;
        }

        public virtual object[] ConvertSelections(IModelDoc2 model, IFeature feat, object[] selection)
        {
            return selection;
        }
    }
}
