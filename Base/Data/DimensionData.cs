using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CodeStack.SwEx.MacroFeature;
using SolidWorks.Interop.swconst;

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

    /// <summary>
    /// Extension methods on dimension data in the macro feature
    /// </summary>
    public static class DimensionDataExtension
    {
        /// <summary>
        /// Sets the direction of the macro feature dimension.
        /// </summary>
        /// <param name="dimData">Dimension data></param>
        /// <param name="originPt">Dimension starting attach point</param>
        /// <param name="dir">Direction of the dimension</param>
        /// <remarks>Call this method within the <see cref="MacroFeatureEx{TParams}.OnSetDimensions(ISldWorks, IModelDoc2, IFeature, DimensionDataCollection, TParams)"/></remarks>
        public static void SetDirection(this DimensionData dimData, Point originPt, Vector dir)
        {
            var length = (dimData.Dimension.GetSystemValue3((int)swInConfigurationOpts_e.swThisConfiguration, null) as double[])[0];
            Vector extDir = null;

            if (dimData.DisplayDimension.Type2 == (int)swDimensionType_e.swRadialDimension)
            {
                extDir = new Vector(dir);
            }

            dimData.Dimension.SetDirection(originPt, dir, length, extDir);
        }
    }
}
