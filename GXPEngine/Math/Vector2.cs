using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace GXPEngine {
    /// <summary>
    /// Representation of 2D vectors and points.
    /// </summary>
    public struct Vector2 : IEquatable<Vector2>, IFormattable {
        /// <summary>
        /// X component of the vector
        /// </summary>
        public float x;

        /// <summary>
        /// Y component of the vector
        /// </summary>
        public float y;

        public Vector2(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public void Set(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public void Scale(Vector2 scale) {
            x *= scale.x;
            y *= scale.y;
        }

        public void Normalize() {
            float mag = magnitude;
            if (mag > float.Epsilon) {
                this = this / mag;
            }
            else {
                this = zero;
            }
        }

        public Vector2 normalized {
            get {
                Vector2 v = new Vector2(x, y);
                v.Normalize();
                return v;
            }
        }

        public override bool Equals(object other) {
            if (!(other is Vector2)) return false;
            return Equals((Vector2) other);
        }

        public bool Equals(Vector2 other) {
            return x == other.x && y == other.y;
        }

        public override int GetHashCode() {
            return x.GetHashCode() ^ (y.GetHashCode() << 2);
        }

        /// <summary>
        /// Returns a nicely formatted string for the vector
        /// </summary>
        public override string ToString() {
            return ToString(null, CultureInfo.InvariantCulture.NumberFormat);
        }

        public string ToString(string format, IFormatProvider formatProvider) {
            if (string.IsNullOrEmpty(format))
                format = "F1";
            return String.Format("({0}, {1})", x.ToString(format, formatProvider), y.ToString(format, formatProvider));
        }

        /// <summary>
        /// Returns the length of this vector
        /// </summary>
        public float magnitude => Mathf.Sqrt(x * x + y * y);

        /// <summary>
        /// Returns the squared length of this vector
        /// </summary>
        public float sqrMagnitude => x * x + y * y;
        
        /// <summary>
        /// Converts a <see cref="GXPEngine.Vector3"/> to a Vector2.
        /// </summary>
        public static implicit operator Vector2(Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        /// <summary>
        /// Converts a Vector2 to a <see cref="GXPEngine.Vector3"/>.
        /// </summary>
        public static implicit operator Vector3(Vector2 v)
        {
            return new Vector3(v.x, v.y, 0);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b) {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b) {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator *(Vector2 a, Vector2 b) {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        public static Vector2 operator /(Vector2 a, Vector2 b) {
            return new Vector2(a.x / b.x, a.y / b.y);
        }

        public static Vector2 operator -(Vector2 a) {
            return new Vector2(-a.x, -a.y);
        }

        public static Vector2 operator *(Vector2 a, float d) {
            return new Vector2(a.x * d, a.y * d);
        }

        public static Vector2 operator *(float d, Vector2 a) {
            return new Vector2(a.x * d, a.y * d);
        }

        public static Vector2 operator /(Vector2 a, float d) {
            return new Vector2(a.x / d, a.y / d);
        }

        public static bool operator ==(Vector2 lhs, Vector2 rhs) {
            float diffX = lhs.x - rhs.x;
            float diffY = lhs.y - rhs.y;
            return (diffX * diffX + diffY * diffY) < Mathf.Epsilon;
        }

        public static bool operator !=(Vector2 lhs, Vector2 rhs) {
            return !(lhs == rhs);
        }

        static readonly Vector2 zeroVector = new Vector2(0f, 0f);
        static readonly Vector2 oneVector = new Vector2(1f, 1f);
        static readonly Vector2 upVector = new Vector2(0f, 1f);
        static readonly Vector2 downVector = new Vector2(0f, -1f);
        static readonly Vector2 leftVector = new Vector2(-1f, 0f);
        static readonly Vector2 rightVector = new Vector2(1f, 0f);
        static readonly Vector2 positiveInfinityVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        static readonly Vector2 negativeInfinityVector = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        public static Vector2 zero => zeroVector;
        public static Vector2 one => oneVector;
        public static Vector2 up => upVector;
        public static Vector2 down => downVector;
        public static Vector2 left => leftVector;
        public static Vector2 right => rightVector;
        public static Vector2 positiveInfinity => positiveInfinityVector;
        public static Vector2 negativeInfinity => negativeInfinityVector;
    }
}