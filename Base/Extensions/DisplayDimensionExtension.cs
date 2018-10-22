using CodeStack.SwEx.MacroFeature.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolidWorks.Interop.sldworks
{
    public static class DisplayDimensionExtension
    {
        //TODO: remove math utils from the parameters

        public static void SetDimensionPosition(this IDisplayDimension dispDim,
            Point originPt, Vector dir, double length, IMathUtility mathUtil)
        {
            var dimDirVec = mathUtil.CreateVector(dir.ToArray()) as MathVector;
            var startPt = mathUtil.CreatePoint(originPt.ToArray()) as IMathPoint;
            var endPt = MovePoint(startPt, dimDirVec, length);

            var refPts = new IMathPoint[] 
            {
                startPt,
                endPt,
                mathUtil.CreatePoint(new double[3]) as IMathPoint
            };

            MathVector extDirVec = null;

            var yVec = new Vector(0, 1, 0);
            if (dir.IsSame(yVec))
            {
                var xVec = new double[] { 1, 0, 0 };
                extDirVec = mathUtil.CreateVector(xVec) as MathVector;
            }
            else
            {
                extDirVec = (mathUtil.CreateVector(yVec.ToArray()) as MathVector).Cross(dimDirVec) as MathVector;
            }

            var dim = dispDim.GetDimension2(0);

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
