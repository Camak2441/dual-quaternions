using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DualQuaternions {
    public readonly struct DualQuaternion {

        public static DualQuaternion operator +(DualQuaternion a, DualQuaternion b) => new(a.R + b.R, a.E + b.E);
        public static DualQuaternion operator -(DualQuaternion a, DualQuaternion b) => new(a.R - b.R, a.E - b.E);
        public static DualQuaternion operator -(DualQuaternion a) => new(-a.R, -a.E);
        public static DualQuaternion operator *(DualQuaternion a, DualQuaternion b) => new(a.R * b.R, a.R * b.E + a.E * b.R);
        public static DualQuaternion operator *(double a, DualQuaternion b) => new(a * b.R, a * b.E);
        public static DualQuaternion operator /(DualQuaternion a, DualQuaternion b) => new(a.R / b.R, (a.E * b.R - a.R * b.E) / (b.R * b.R));
        public static DualQuaternion operator /(DualQuaternion a, double b) => new(a.R / b, a.E / b);
        public static bool operator ==(DualQuaternion a, DualQuaternion b) => a.R == b.R && a.E == b.E;
        public static bool operator !=(DualQuaternion a, DualQuaternion b) => !(a == b);
        public static implicit operator DualQuaternion((Quaternion r, Quaternion e) v) => new(v.r, v.e);

        public static DualQuaternion DConj(DualQuaternion a) => new(a.R, -a.E);
        public static DualQuaternion QConj(DualQuaternion a) => new(Quaternion.Conj(a.R), Quaternion.Conj(a.E));
        public static DualQuaternion FConj(DualQuaternion a) => new(Quaternion.Conj(a.R), -Quaternion.Conj(a.E));
        public static DualQuaternion Norm(DualQuaternion a) => a / Math.Sqrt(a.R.SqrMag);

        public static DualQuaternion Identity = MakeFromAxisAngleTrans(Vector3.UnitX, 0, Vector3.Zero);

        public static DualQuaternion MakeFromAxisAngleTrans(Vector3 axis, double angle, Vector3 t) {
            Quaternion q = Quaternion.MakeFromAxisAngle(axis, angle);
            return new(q, 0.5 * Quaternion.MakeFromVector(t) * q);
        }
        public static DualQuaternion MakeFromVector(Vector3 x) => new(Quaternion.One, Quaternion.MakeFromVector(x));

        public static Vector3 ApplyTransform(Vector3 p, DualQuaternion t) {
            DualQuaternion res = t * MakeFromVector(p) * FConj(t);
            return new Vector3((float)res.E.I, (float)res.E.J, (float)res.E.K);
        }

        public static Matrix ConvertToMatrix(DualQuaternion t) {
            Vector3 x, y, z, o;
            o = t.Apply(Vector3.Zero);
            x = t.Apply(Vector3.UnitX) - o;
            y = t.Apply(Vector3.UnitY) - o;
            z = t.Apply(Vector3.UnitZ) - o;
            return new(x.X, y.X, z.X, 0, x.Y, y.Y, z.Y, 0, x.Z, y.Z, z.Z, 0, o.X, o.Y, o.Z, 1);
        }

        public readonly Quaternion R;
        public readonly Quaternion E;

        public DualQuaternion(Quaternion r, Quaternion e) {
            this.R = r;
            this.E = e;
        }

        public Vector3 Apply(Vector3 p) => ApplyTransform(p, this);

        public Matrix ConvertToMatrix() => ConvertToMatrix(this);

        public DualQuaternion Norm() => Norm(this);

        public override string ToString() {
            return $"{R} + ({E})ε";
        }

        public override bool Equals(object obj) => obj is DualQuaternion dq && this == dq;

        public override int GetHashCode() => HashCode.Combine(R, E);
    }
}
