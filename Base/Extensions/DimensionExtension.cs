using CodeStack.SwEx.MacroFeature.Helpers;
using CodeStack.SwEx.MacroFeature.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolidWorks.Interop.sldworks
{
    public static class DimensionExtension
    {
        private static readonly ISldWorks m_App;

        static DimensionExtension()
        {
            m_App = Context.CurrentApp;
        }

        public static void SetDimensionPosition(this IDimension dim,
            Point originPt, Vector dir, double length)
        {
            var mathUtils = m_App.IGetMathUtility();

            var dimDirVec = mathUtils.CreateVector(dir.ToArray()) as MathVector;
            var startPt = mathUtils.CreatePoint(originPt.ToArray()) as IMathPoint;
            var endPt = MovePoint(startPt, dimDirVec, length);

            var refPts = new IMathPoint[] 
            {
                startPt,
                endPt,
                mathUtils.CreatePoint(new double[3]) as IMathPoint
            };

            MathVector extDirVec = null;

            var yVec = new Vector(0, 1, 0);
            if (dir.IsSame(yVec))
            {
                var xVec = new double[] { 1, 0, 0 };
                extDirVec = mathUtils.CreateVector(xVec) as MathVector;
            }
            else
            {
                extDirVec = (mathUtils.CreateVector(yVec.ToArray()) as MathVector).Cross(dimDirVec) as MathVector;
            }
            
            dim.DimensionLineDirection = dimDirVec;
            dim.ExtensionLineDirection = extDirVec;
            dim.ReferencePoints = refPts;
        }

        private static IMathPoint MovePoint(IMathPoint pt, MathVector dir, double dist)
        {
            var moveVec = dir.Normalise().Scale(dist);

            return pt.AddVector(moveVec) as IMathPoint;
        }
    }
}
