using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CodeStack.SwEx.MacroFeature;

namespace CodeStack.SwEx.MacroFeature.Data
{
    /// <summary>
    /// Structure representing macro feature dimension
    /// </summary>
    /// <remarks>This is passed to <see cref="MacroFeatureEx{TParams}.OnSetDimensions(ISldWorks, IModelDoc2, IFeature, DimensionDataCollection, TParams)"/>
    /// in the rebuild operation</remarks>
    public class DimensionData : IDisposable
    {
        /// <summary>
        /// Pointer to display dimension
        /// </summary>
        public IDisplayDimension DisplayDimension { get; private set; }

        /// <summary>
        /// Pointer to dimension
        /// </summary>
        public IDimension Dimension { get; private set; }

        internal DimensionData(IDisplayDimension dispDim)
        {
            DisplayDimension = dispDim;
            Dimension = dispDim.GetDimension2(0);
        }

        /// <summary>
        /// Disposing object
        /// </summary>
        public void Dispose()
        {
            if (Marshal.IsComObject(Dimension))
            {
                Marshal.ReleaseComObject(Dimension);
            }

            if (Marshal.IsComObject(DisplayDimension))
            {
                Marshal.ReleaseComObject(DisplayDimension);
            }

            Dimension = null;
            DisplayDimension = null;
        }
    }
}
