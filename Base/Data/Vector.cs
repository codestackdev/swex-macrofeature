using System;

namespace CodeStack.SwEx.MacroFeature.Data
{
    public class Vector : Point
    {
        public Vector(double x, double y, double z) : base(x, y, z)
        {
        }

        public Vector(double[] dir) : base(dir)
        {
        }

        public Vector(Vector vec) : base(vec.X, vec.Y, vec.Z)
        {
        }

        public bool IsSame(Vector vec, bool normilize = true)
        {
            if (vec == null)
            {
                throw new ArgumentNullException(nameof(vec));
            }

            if (normilize)
            {
                var thisNorm = this.Normalize();
                var otherNorm = vec.Normalize();

                return thisNorm.IsSame(otherNorm.X, otherNorm.Y, otherNorm.Z)
                    || thisNorm.IsSame(-otherNorm.X, -otherNorm.Y, -otherNorm.Z);
            }
            else
            {
                return IsSame(vec.X, vec.Y, vec.Z);
            }
        }

        public Vector Normalize()
        {
            var thisLen = GetLength();
            var thisNorm = new Vector(X / thisLen, Y / thisLen, Z / thisLen);
            return thisNorm;
        }

        public Vector Cross(Vector vector)
        {
            var x = Y * vector.Z - vector.Y * Z; ;
            var y = (X * vector.Z - vector.X * Z) * -1;
            var z = X * vector.Y - vector.X * Y;

            return new Vector(x, y, z);
        }

        public double GetLength()
        {
            return Math.Sqrt(Math.Pow(X, 2)
                + Math.Pow(Y, 2)
                + Math.Pow(Z, 2));
        }
    }
}
