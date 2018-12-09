//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Data
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point(double[] pt)
        {
            if (pt == null)
            {
                throw new ArgumentNullException(nameof(pt));
            }

            if (pt.Length != 3)
            {
                throw new ArgumentException("The size of input array must be equal to 3 (coordinate)");
            }

            X = pt[0];
            Y = pt[1];
            Z = pt[2];
        }

        public double[] ToArray()
        {
            return new double[] { X, Y, Z };
        }

        public bool IsSame(Point pt)
        {
            if (pt == null)
            {
                throw new ArgumentNullException(nameof(pt));
            }

            return IsSame(pt.X, pt.Y, pt.Z);
        }

        public bool IsSame(double x, double y, double z)
        {
            return X == x && Y == y && Z == z;
        }

        public static Vector operator -(Point pt1, Point pt2)
        {
            return new Vector(pt2.X - pt1.X, pt2.Y - pt1.Y, pt2.Z - pt1.Z);
        }

        public static Point operator +(Point pt, Vector vec)
        {
            return new Point(pt.X + vec.X, pt.Y + vec.Y, pt.Z + vec.Z);
        }

        public Point Move(Vector dir, double dist)
        {
            var moveVec = dir.Normalize();
            moveVec.Scale(dist);
            return this + moveVec;
        }

        public void Scale(double scalar)
        {
            X *= scalar;
            Y *= scalar;
            Z *= scalar;
        }

        public override string ToString()
        {
            return $"{X};{Y};{Z}";
        }
    }
}
