//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.MacroFeature.Data;
using CodeStack.SwEx.MacroFeature.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolidWorks.Interop.sldworks
{
    /// <summary>
    /// Extension methods of <see href="http://help.solidworks.com/2016/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.imodeler_members.html">IModeler</see> interface
    /// </summary>
    /// <remarks>Use these extensions in the context of <see cref="CodeStack.SwEx.MacroFeature.MacroFeatureEx.OnRebuild(ISldWorks, IModelDoc2, IFeature)"/>
    /// to create a body for macro feature
    /// </remarks>
    public static class ModelerEx
    {
        private static readonly IMathUtility m_MathUtils;

        static ModelerEx()
        {
            m_MathUtils = Context.CurrentApp.IGetMathUtility();
        }

        /// <inheritdoc cref="CreateBox(IModeler, Point, Vector, double, double, double)"/>
        /// <param name="refDir">Input or output direction of ref axis which corresponds to X. Specify null to auto calculate</param>
        public static IBody2 CreateBox(this IModeler modeler, Point center, Vector dir, ref Vector refDir,
            double width, double length, double height)
        {
            IMathVector refVec;
            var surf = CreatePlanarSurface(modeler, center, dir, ref refDir, out refVec);

            var xVec = new Vector(refVec.ArrayData as double[]);
            var yVec = xVec.Cross(dir);
            
            var getPointFunc = new Func<double, double, Point>(
                (x, y) =>
                {
                    var pt = center.Move(xVec, x);
                    pt = pt.Move(yVec, y);
                    return pt;
                });

            var corners = new Point[]
            {
                getPointFunc.Invoke(-width / 2, -length / 2),
                getPointFunc.Invoke(-width / 2, length / 2),
                getPointFunc.Invoke(width / 2, length / 2),
                getPointFunc.Invoke(width / 2, -length / 2)
            };

            var createCurveFunc = new Func<Point, Point, ICurve>((Point p1, Point p2) =>
            {
                var curve = modeler.CreateLine(p1.ToArray(), (p1 - p2).ToArray()) as ICurve;
                curve = curve.CreateTrimmedCurve2(p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z);
                return curve;
            });

            var curves = new ICurve[]
            {
                createCurveFunc.Invoke(corners[0], corners[1]),
                createCurveFunc.Invoke(corners[1], corners[2]),
                createCurveFunc.Invoke(corners[2], corners[3]),
                createCurveFunc.Invoke(corners[3], corners[0])
            };

            var zVec = m_MathUtils.CreateVector(dir.ToArray()) as MathVector;

            return Extrude(modeler, surf, curves, zVec, height);
        }

        /// <summary>
        /// Creates the box solid geometry
        /// </summary>
        /// <param name="modeler">Pointer to modeler</param>
        /// <param name="center">Center coordinate of the box in meters</param>
        /// <param name="dir">Direction of the box</param>
        /// <param name="width">Width of the box in meters</param>
        /// <param name="length">Length of the box in meters</param>
        /// <param name="height">Height of the box in meters. This is a dimension parallel to <paramref name="dir"/></param>
        /// <returns>Pointer to a temp body</returns>
        /// <remarks>Use this method instead of built-in <see href="http://help.solidworks.com/2016/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IModeler~CreateBodyFromBox3.html">IModeler::CreateBodyFromBox</see>
        /// If you need to preserve entity ids as the body generated using the built-in method won't allow to set user id,
        /// which means any reference geometry generated in relation to box entities will become dangling upon rebuild</remarks>
        public static IBody2 CreateBox(this IModeler modeler, Point center, Vector dir,
            double width, double length, double height)
        {
            Vector refDir = null;

            return CreateBox(modeler, center, dir, ref refDir, width, length, height);
        }

        /// <summary>
        /// Creates the cylindrical body
        /// </summary>
        /// <param name="modeler">Pointer to modeler</param>
        /// <param name="center">Center coordinate of cylinder in meters</param>
        /// <param name="axis">Cylinder axis</param>
        /// <param name="radius">Cylinder radius in meters</param>
        /// <param name="height">Cylinder height</param>
        /// <returns>Cylindrical temp body</returns>
        /// <remarks>Use this method instead of built-in <see href="http://help.solidworks.com/2016/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IModeler~CreateBodyFromCyl.html">IModeler::CreateBodyFromCyl</see>
        /// If you need to preserve entity ids as the body generated using the built-in method won't allow to set user id,
        /// which means any reference geometry generated in relation to cylinder entities will become dangling upon rebuild</remarks>
        public static IBody2 CreateCylinder(this IModeler modeler, Point center, Vector axis, double radius, double height)
        {
            IMathVector refVec;
            Vector refDir = null;

            var surf = CreatePlanarSurface(modeler, center, axis, ref refDir, out refVec);

            var radDir = new Vector(refVec.ArrayData as double[]);
            var refPt = center.Move(radDir, radius);
            
            var arc = modeler.CreateArc(center.ToArray(), axis.ToArray(), radius, refPt.ToArray(), refPt.ToArray()) as ICurve;
            arc = arc.CreateTrimmedCurve2(refPt.X, refPt.Y, refPt.Z, refPt.X, refPt.Y, refPt.Z);

            var dir = m_MathUtils.CreateVector(axis.ToArray()) as MathVector;

            return Extrude(modeler, surf, new ICurve[] { arc }, dir, height);
        }

        private static IBody2 Extrude(IModeler modeler, ISurface surf, ICurve[] boundary, MathVector dir, double height)
        {
            var sheetBody = surf.CreateTrimmedSheet4(boundary, true) as Body2;

            return modeler.CreateExtrudedBody(sheetBody, dir, height) as IBody2;
        }

        private static ISurface CreatePlanarSurface(IModeler modeler, Point center, Vector dir, 
            ref Vector refDir, out IMathVector refVec)
        {
            if (refDir == null)
            {
                var transform = GetTransformBetweenVectors(new Vector(0, 0, 1), dir, center);

                refVec = (m_MathUtils.CreateVector(
                    new double[] { 1, 0, 0 }) as IMathVector)
                    .MultiplyTransform(transform) as IMathVector;

                refDir = new Vector(refVec.ArrayData as double[]);
            }
            else
            {
                refVec = m_MathUtils.CreateVector(refDir.ToArray()) as IMathVector;
            }
            
            return modeler.CreatePlanarSurface2(center.ToArray(), dir.ToArray(), refVec.ArrayData) as ISurface;
        }

        private static IMathTransform GetTransformBetweenVectors(Vector firstVector, Vector secondVector, Point point)
        {
            var mathVec1 = (m_MathUtils.CreateVector(firstVector.ToArray()) as IMathVector).Normalise();
            var mathVec2 = (m_MathUtils.CreateVector(secondVector.ToArray()) as IMathVector).Normalise();
            var crossVec = (mathVec1.Cross(mathVec2) as IMathVector).Normalise();

            var dot = mathVec1.Dot(mathVec2);
            var vec1Len = mathVec1.GetLength();
            var vec2Len = mathVec2.GetLength();

            var angle = Math.Acos(dot / vec1Len * vec2Len);

            var mathPt = m_MathUtils.CreatePoint(point.ToArray()) as IMathPoint;

            var mathTransform = m_MathUtils.CreateTransformRotateAxis(mathPt, crossVec, angle) as IMathTransform;

            return mathTransform;
        }
    }
}
