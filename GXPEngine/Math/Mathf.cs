using System;
using System.Runtime.InteropServices;
namespace GXPEngine
{
	/// <summary>
	/// Contains several functions for doing floating point Math
	/// </summary>
	public static class Mathf
	{
		public static volatile float FloatMinNormal = 1.17549435E-38f;
		public static volatile float FloatMinDenormal = float.Epsilon;
		public static bool IsFlushToZeroEnabled = (FloatMinDenormal == 0);
		
		/// <summary>
		/// Constant PI
		/// </summary>
		public const float PI = (float)System.Math.PI;
		
		/// <summary>
		/// A representation of positive infinity
		/// </summary>
		public const float Infinity = float.PositiveInfinity;
		
		/// <summary>
		/// A representation of negative infinity
		/// </summary>
		public const float NegativeInfinity = float.PositiveInfinity;
		
		/// <summary>
		/// Degrees-to-radians conversion constant
		/// </summary>
		public const float Deg2Rad = PI * 2F / 360F;
		
		/// <summary>
		/// Radians-to-degrees conversion constant
		/// </summary>
		public const float Rad2Deg = 1F / Deg2Rad;
		
		/// <summary>
		/// Tiny floating point value
		/// </summary>
		public static readonly float Epsilon = IsFlushToZeroEnabled ? FloatMinNormal : FloatMinDenormal;

		/// <summary>
		/// Returns the absolute value of specified number
		/// </summary>
		public static int Abs (int value) {
			return (value<0)?-value:value;
		}

		/// <summary>
		/// Returns the absolute value of specified number
		/// </summary>
		public static float Abs(float value) {
			return (value<0)?-value:value;
		}

		/// <summary>
		/// Returns the acosine of the specified number
		/// </summary>
		public static float Acos(float f) {
			return (float)System.Math.Acos(f);
		}

		/// <summary>
		/// Returns the arctangent of the specified number
		/// </summary>
		public static float Atan(float f) {
			return (float)System.Math.Atan (f);
		}

		/// <summary>
		/// Returns the angle whose tangent is the quotent of the specified values
		/// </summary>
		public static float Atan2(float y, float x) {
			return (float)System.Math.Atan2 (y, x);
		}

		/// <summary>
		/// Returns the smallest integer bigger greater than or equal to the specified number
		/// </summary>
		public static int Ceiling(float a) {
			return (int)System.Math.Ceiling (a);
		}

		/// <summary>
		/// Returns the cosine of the specified number in radians
		/// </summary>
		public static float Cos(float f) {
			return (float)System.Math.Cos (f);
		}

		/// <summary>
		/// Returns the hyperbolic cosine of the specified number
		/// </summary>
		public static float Cosh(float value) {
			return (float)System.Math.Cosh (value);
		}

		/// <summary>
		/// Returns e raised to the given number
		/// </summary>
		public static float Exp(float f) {
			return (float)System.Math.Exp (f);
		}

		/// <summary>
		/// Returns the largest integer less than or equal to the specified value
		/// </summary>
		public static int Floor(float f) {
			return (int)System.Math.Floor (f);
		}

		/// <summary>
		/// Returns the natural logarithm of the specified number
		/// </summary>
		public static float Log(float f) {
			return (float)System.Math.Log (f);
		}

		/// <summary>
		/// Returns the log10 of the specified number
		/// </summary>
		public static float Log10(float f) {
			return (float)System.Math.Log10(f);
		}
		
		/// <summary>
		/// Returns x raised to the power of y
		/// </summary>
		public static float Pow(float x, float y) {
			return (float)System.Math.Pow (x, y);
		}

		/// <summary>
		/// Returns the nearest integer to the specified value
		/// </summary>
		public static int Round(float f) {
			return (int)System.Math.Round (f);
		}

		/// <summary>
		/// Returns a value indicating the sign of the specified number (-1=negative, 0=zero, 1=positive)
		/// </summary>
		public static int Sign(float f) {
			if (f < 0) return -1;
			if (f > 0) return 1;
			return 0;
		}

		/// <summary>
		/// Returns a value indicating the sign of the specified number (-1=negative, 0=zero, 1=positive)
		/// </summary>
		public static int Sign(int f) {
			if (f < 0) return -1;
			if (f > 0) return 1;
			return 0;
		}

		/// <summary>
		/// Returns the sine of the specified number in radians
		/// </summary>
		public static float Sin(float f) {
			return (float)System.Math.Sin (f);
		}
		
		/// <summary>
		/// Returns the hyperbolic sine of the specified number
		/// </summary>
		public static float Sinh(float value) {
			return (float)System.Math.Sinh (value);
		}

		/// <summary>
		/// Returns the square root of the specified number
		/// </summary>
		public static float Sqrt(float f) {
			return (float)System.Math.Sqrt (f);
		}

		/// <summary>
		/// Returns the tangent of the specified number in radians
		/// </summary>
		public static float Tan(float f) {
			return (float)System.Math.Tan (f);
		}
		
		/// <summary>
		/// Returns the hyperbolic tangent of the specified number
		/// </summary>
		public static float Tanh(float value) {
			return (float)System.Math.Tanh (value);
		}

		/// <summary>
		/// Calculates the integral part of the specified number
		/// </summary>
		public static float Truncate(float f) {
			return (float)System.Math.Truncate (f);
		}

		/// <summary>
		/// Clamps f in the range [min,max]:
		/// Returns min if f<min, max if f>max, and f otherwise.
		/// </summary>
		public static float Clamp(float f, float min, float max) {
			return f < min ? min : (f > max ? max : f);
		}

		/// <summary>
		/// Clamps <paramref name="v"/> in the range [<paramref name="min"/>, <paramref name="max"/>]
		/// </summary>
		/// <param name="v">The value to be clamped</param>
		/// <param name="min">The minimum value</param>
		/// <param name="max">The maximum value</param>
		/// <returns><paramref name="min"/> if <paramref name="v"/> is less than <paramref name="min"/>,
		/// <paramref name="max"/> if <paramref name="v"/> is greater than <paramref name="max"/>,
		/// and <paramref name="v"/> otherwise.</returns>
		public static int Clamp(int v, int min, int max) {
			if (v < min) return min;
			if (v > max) return max;
			return v;
		}

		/// <summary>
		/// Clamps <paramref name="f"/> in the range [0, 1]
		/// </summary>
		/// <param name="f">The value to clamp</param>
		/// <returns>0 if <paramref name="f"/> is less than 0,
		/// 1 if <paramref name="f"/> is greater than 1,
		/// and <paramref name="f"/> otherwise.</returns>
		public static float Clamp01(float f) {
			if (f < 0f) return 0f;
			if (f > 1f) return 1f;
			return f;
		}

		/// <summary>
		/// Returns the smallest of the two specified values
		/// </summary>
        public static float Min(float a, float b) { return a < b ? a : b; }
		
		/// <summary>
		/// Returns the smallest of two or more values
		/// </summary>
        public static float Min(params float[] values)
        {
            int len = values.Length;
            if (len == 0)
                return 0;
            float m = values[0];
            for (int i = 1; i < len; i++)
            {
                if (values[i] < m)
                    m = values[i];
            }
            return m;
        }

		/// <summary>
		/// Returns the smallest of the two specified values
		/// </summary>
        public static int Min(int a, int b) { return a < b ? a : b; }
		
		/// <summary>
		/// Returns the smallest of two or more values
		/// </summary>
        public static int Min(params int[] values)
        {
            int len = values.Length;
            if (len == 0)
                return 0;
            int m = values[0];
            for (int i = 1; i < len; i++)
            {
                if (values[i] < m)
                    m = values[i];
            }
            return m;
        }

        /// <summary>
        /// Returns the biggest of the two specified values
        /// </summary>
        public static float Max(float a, float b) { return a > b ? a : b; }
        
        /// <summary>
        /// Returns the biggest of two or more values
        /// </summary>
        public static float Max(params float[] values)
        {
            int len = values.Length;
            if (len == 0)
                return 0;
            float m = values[0];
            for (int i = 1; i < len; i++)
            {
                if (values[i] > m)
                    m = values[i];
            }
            return m;
        }

        /// <summary>
        /// Returns the biggest of the two specified values
        /// </summary>
        public static int Max(int a, int b) { return a > b ? a : b; }
        
        /// <summary>
        /// Returns the biggest of two or more values
        /// </summary>
        public static int Max(params int[] values)
        {
            int len = values.Length;
            if (len == 0)
                return 0;
            int m = values[0];
            for (int i = 1; i < len; i++)
            {
                if (values[i] > m)
                    m = values[i];
            }
            return m;
        }

        /// <summary>
        /// Interpolates between <paramref name="a"/> and <paramref name="b"/>
        /// by <paramref name="t"/>. <paramref name="t"/> is clamped between 0 and 1
        /// </summary>
        public static float Lerp(float a, float b, float t) {
	        return a + (b - a) * Clamp01(t);
        }
		
        /// <summary>
        /// Interpolates between <paramref name="a"/> and <paramref name="b"/>
        /// by <paramref name="t"/> without clamping the interpolant
        /// </summary>
        public static float LerpUnclamped(float a, float b, float t) {
	        return a + (b - a) * t;
        }
        
	}
}

