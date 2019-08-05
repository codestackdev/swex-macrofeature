//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.MacroFeature.Helpers;
using CodeStack.SwEx.MacroFeature.Data;

namespace SolidWorks.Interop.sldworks
{
    /// <summary>
    /// Extension methods of <see href="http://help.solidworks.com/2016/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.idimension.html">IDimension</see> interface
    /// </summary>
    public static class DimensionEx
    {
        private static readonly IMathUtility m_MathUtils;

        static DimensionEx()
        {
            m_MathUtils = Context.CurrentApp.IGetMathUtility();
        }

        /// <summary>
        /// Sets the direction of the macro feature dimension.
        /// </summary>
        /// <param name="dim">Pointer to dimension. Usually retrieved from <see cref="DimensionData.Dimension"/></param>
        /// <param name="originPt">Dimension starting attach point</param>
        /// <param name="dir">Direction of the dimension</param>
        /// <param name="length">Length of the dimension (usually equal to its value)</param>
        /// <param name="extDir">Optional direction of extension line</param>
        /// <remarks>Call this method within the <see cref="CodeStack.SwEx.MacroFeature.MacroFeatureEx{TParams}.OnSetDimensions(ISldWorks, IModelDoc2, IFeature, DimensionDataCollection, TParams)"/></remarks>
        public static void SetDirection(this IDimension dim,
            Point originPt, Vector dir, double length, Vector extDir = null)
        {
            var dimDirVec = m_MathUtils.CreateVector(dir.ToArray()) as MathVector;
            var startPt = m_MathUtils.CreatePoint(originPt.ToArray()) as IMathPoint;
            var endPt = m_MathUtils.CreatePoint(originPt.Move(dir, length).ToArray()) as IMathPoint;

            var refPts = new IMathPoint[] 
            {
                startPt,
                endPt,
                m_MathUtils.CreatePoint(new double[3]) as IMathPoint
            };

            if (extDir == null)
            {
                var yVec = new Vector(0, 1, 0);
                if (dir.IsSame(yVec))
                {
                    extDir = new Vector(1, 0, 0);
                }
                else
                {
                    extDir = yVec.Cross(dir);
                }
            }

            var extDirVec = m_MathUtils.CreateVector(extDir.ToArray()) as MathVector;
            
            dim.DimensionLineDirection = dimDirVec;
            dim.ExtensionLineDirection = extDirVec;
            dim.ReferencePoints = refPts;
        }
    }
}
