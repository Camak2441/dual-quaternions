using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;

namespace DualQuaternions {
    public readonly struct Quaternion {

        public static Quaternion One = new(1, 0, 0, 0);

        public static Quaternion operator +(Quaternion a, Quaternion b) => new(a.R + b.R, a.I + b.I, a.J + b.J, a.K + b.K);
        public static Quaternion operator -(Quaternion a, Quaternion b) => new(a.R - b.R, a.I - b.I, a.J - b.J, a.K - b.K);
        public static Quaternion operator -(Quaternion a) => new(-a.R, -a.I, -a.J, -a.K);
        public static Quaternion operator *(Quaternion a, Quaternion b) => new(
            a.R * b.R - a.I * b.I - a.J * b.J - a.K * b.K,
            a.R * b.I + a.I * b.R + a.J * b.K - a.K * b.J,
            a.R * b.J - a.I * b.K + a.J * b.R + a.K * b.I,
            a.R * b.K + a.I * b.J - a.J * b.I + a.K * b.R
            );
        public static Quaternion operator *(double a, Quaternion b) => new(a * b.R, a * b.I, a * b.J, a * b.K);
        public static Quaternion operator /(Quaternion a, Quaternion b) => new(
            ( a.R * b.R + a.I * b.I + a.J * b.J + a.K * b.K) / b.SqrMag,
            (-a.R * b.I + a.I * b.R - a.J * b.K + a.K * b.J) / b.SqrMag,
            (-a.R * b.J + a.I * b.K + a.J * b.R - a.K * b.I) / b.SqrMag,
            (-a.R * b.K - a.I * b.J + a.J * b.I + a.K * b.R) / b.SqrMag
            );
        public static Quaternion operator /(Quaternion a, double b) => new(a.R / b, a.I / b, a.J / b, a.K / b);
        public static bool operator ==(Quaternion a, Quaternion b) => a.R == b.R && a.I == b.I && a.J == b.J && a.K == b.K;
        public static bool operator !=(Quaternion a, Quaternion b) => !(a == b);
        public static implicit operator Quaternion((double r, double i, double j, double k) v) => new(v.r, v.i, v.j, v.k);

        public static Quaternion Conj(Quaternion a) => new(a.R, -a.I, -a.J, -a.K);
        public static Quaternion Inv(Quaternion a) => new(a.R / a.SqrMag, -a.I / a.SqrMag, -a.J / a.SqrMag, -a.K / a.SqrMag);
        public static Quaternion Norm(Quaternion a) {
            double mag = Math.Sqrt(a.SqrMag);
            return new(a.R / mag, a.I / mag, a.J / mag, a.K / mag);
        }

        public static Quaternion MakeFromAxisAngle(Vector3 axis, double angle) {
            axis.Normalize();
            double s = Math.Sin(angle / 2);
            return new Quaternion(Math.Cos(angle / 2), axis.X * s, axis.Y * s, axis.Z * s);
        }

        public static Quaternion MakeFromVector(Vector3 x) => new(0, x.X, x.Y, x.Z);

        public static Vector3 ApplyTransform(Vector3 p, Quaternion t) {
            Quaternion res = t * MakeFromVector(p) * Inv(t);
            return new Vector3((float)res.I, (float)res.J, (float)res.K);
        }

        public readonly double R;
        public readonly double I;
        public readonly double J;
        public readonly double K;
        public readonly double SqrMag;
        public readonly bool IsZero;

        public Quaternion(double r, double i, double j, double k) {
            this.R = r;
            this.I = i;
            this.J = j;
            this.K = k;
            this.SqrMag = r * r + i * i + j * j + k * k;
            IsZero = SqrMag == 0;
        }

        public Vector3 Apply(Vector3 p) => ApplyTransform(p, this);
        public Quaternion Norm() => Norm(this);

        public override string ToString() {
            return $"{R} + {I}i + {J}j + {K}k";
        }

        public override bool Equals(object obj) => obj is Quaternion q && this == q;

        public override int GetHashCode() => HashCode.Combine(R, I, J, K);
    }
}
