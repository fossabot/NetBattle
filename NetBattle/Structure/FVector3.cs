using System;

namespace NetBattle.Structure {
    public struct FVector3 {
        public static readonly FVector3 Zero = new FVector3(0.0f, 0.0f, 0.0f);
        public static readonly FVector3 I = new FVector3(1.0f, 0.0f, 0.0f);
        public static readonly FVector3 J = new FVector3(0.0f, 1.0f, 0.0f);
        public static readonly FVector3 K = new FVector3(0.0f, 0.0f, 1.0f);
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public FVector3(float x) {
            X = x;
            Y = 0.0f;
            Z = 0.0f;
        }

        public FVector3(float x, float y) {
            X = x;
            Y = y;
            Z = 0.0f;
        }

        public FVector3(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
        }

        public void Deconstruct(out float x, out float y, out float z) {
            x = X;
            y = Y;
            z = Z;
        }

        public static FVector3 Midpoint(FVector3 v, FVector3 w) => (v + w) / 2.0f;

        public static FVector3 operator +(FVector3 v) => v;
        public static FVector3 operator -(FVector3 v) => new FVector3(-v.X, -v.Y, -v.Z);
        public static FVector3 operator +(FVector3 v, FVector3 w) => new FVector3(v.X + w.X, v.Y + w.X, v.Z + w.X);
        public static FVector3 operator -(FVector3 v, FVector3 w) => new FVector3(v.X - w.X, v.Y - w.Y, v.Z - w.Z);
        public static FVector3 operator *(FVector3 v, float f) => new FVector3(v.X * f, v.Y * f, v.Z * f);
        public static FVector3 operator *(float f, FVector3 v) => new FVector3(v.X * f, v.Y * f, v.Z * f);

        public static FVector3 operator /(FVector3 v, float f) {
            if (Math.Abs(f) < float.Epsilon) {
                throw new DivideByZeroException();
            }

            return new FVector3(v.X / f, v.Y / f, v.Z / f);
        }
    }
}