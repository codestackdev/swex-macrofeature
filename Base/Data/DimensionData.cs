//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

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
        /// <param name="orientation">Orientation of the dimension. For the linear dimension this is a direction along the measured line. For radial dimension this is a axis of the cylinder</param>
        /// <remarks>Call this method within the <see cref="MacroFeatureEx{TParams}.OnSetDimensions(ISldWorks, IModelDoc2, IFeature, Base.MacroFeatureRebuildResult, DimensionDataCollection, TParams)"/></remarks>
        public static void SetOrientation(this DimensionData dimData, Point originPt, Vector orientation)
        {
            var length = (dimData.Dimension.GetSystemValue3((int)swInConfigurationOpts_e.swThisConfiguration, null) as double[])[0];

            Vector dir = null;
            Vector extDir = null;

            if (dimData.DisplayDimension.Type2 == (int)swDimensionType_e.swRadialDimension)
            {
                var yVec = new Vector(0, 1, 0);

                if (orientation.IsSame(yVec))
                {
                    dir = new Vector(1, 0, 0);
                }
                else
                {
                    dir = orientation.Cross(yVec);
                }

                extDir = orientation.Cross(dir);
            }
            else
            {
                dir = orientation;
            }

            dimData.Dimension.SetDirection(originPt, dir, length, extDir);
        }
    }
}
