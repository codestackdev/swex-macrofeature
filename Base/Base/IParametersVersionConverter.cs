//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Base
{
    /// <summary>
    /// Mechanism allowing to support backward compatibility of the macro feature parameters across the versions.
    /// This interface is coupled with <see cref="Attributes.ParametersVersionAttribute"/>
    /// </summary>
    /// <remarks>This class is a dictionary of version and the appropriate converter from the previous version to 
    /// the specified one. Use <see cref="ParametersVersionConverter"/> for the specific implementation</remarks>
    public interface IParametersVersionConverter : IDictionary<Version, IParameterConverter>
    {
    }

    /// <summary>
    /// Specific implementation of <see cref="IParametersVersionConverter"/>
    /// </summary>
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

    /// <summary>
    /// Represents the conversion routines between this version of parameters and previous version of the parameters
    /// </summary>
    /// <remarks>Use specific implementation <see cref="ParameterConverter"/></remarks>
    public interface IParameterConverter
    {
        /// <summary>
        /// Converts edit bodies from previous version
        /// </summary>
        /// <param name="model">Pointer to current model</param>
        /// <param name="feat">Pointer to current feature</param>
        /// <param name="editBodies">Array of edit bodies form the previous version of parameters</param>
        /// <returns>Array of new bodies in the new version of macro feature</returns>
        IBody2[] ConvertEditBodies(IModelDoc2 model, IFeature feat, IBody2[] editBodies);

        /// <summary>
        /// Converts display dimensions from previous version
        /// </summary>
        /// <param name="model">Pointer to current model</param>
        /// <param name="feat">Pointer to current feature</param>
        /// <param name="dispDims">Array of display dimensions from the previous version</param>
        /// <returns>Array of display dimensions in the new version of macro feature</returns>
        /// <remarks>If number of dimensions have changed in the new version - automatic upgrade is not possible
        /// as currently SOLIDWORKS API doesn't allow to change the existing display dimensions of macro feature.
        /// If this is the case return the <see cref="Placeholders.DisplayDimensionPlaceholder"/> as a placeholder of the updated dimension.
        /// In this case dimension will not be create but feature will be operational and <see cref="MacroFeatureOutdateState_e.Dimensions"/> will be returned as the state of the feature parameters</remarks>
        IDisplayDimension[] ConvertDisplayDimensions(IModelDoc2 model, IFeature feat, IDisplayDimension[] dispDims);

        /// <summary>
        /// Converts parameters from previous version
        /// </summary>
        /// <param name="model">Pointer to current model</param>
        /// <param name="feat">Pointer to current feature</param>
        /// <param name="parameters">Dictionary of parameter names and values</param>
        /// <returns>Parameters for the new version</returns>
        /// <remarks>Parameters list also contains the indices for the objects in macro feature (edit bodies, selection, dimensions)</remarks>
        Dictionary<string, string> ConvertParameters(IModelDoc2 model, IFeature feat, Dictionary<string, string> parameters);

        /// <summary>
        /// Converts selections from previous version
        /// </summary>
        /// <param name="model">Pointer to current model</param>
        /// <param name="feat">Pointer to current feature</param>
        /// <param name="selection">Array of selections from previous version</param>
        /// <returns>Selections correspond to new version of macro feature</returns>
        object[] ConvertSelections(IModelDoc2 model, IFeature feat, object[] selection);
    }

    /// <inheritdoc/>
    /// <summary>
    /// Specific implementation of <see cref="IParameterConverter"/>
    /// </summary>
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
