using System;

namespace Game.Utils {
    public static class Math {
        
        /// <summary>
        /// Returns <paramref name="value"/> mapped from one range [<paramref name="minA"/>, <paramref name="maxA"/>] to another range [<paramref name="minB"/>, <paramref name="maxB"/>]
        /// </summary>
        public static float Map(float value, float minA, float maxA, float minB, float maxB) {
            return (value - minA) / (maxA - minA) * (maxB - minB) + minB;
        }
        /// <summary>
        /// Returns <paramref name="value"/> mapped from one range [<paramref name="minA"/>, <paramref name="maxA"/>] to another range [<paramref name="minB"/>, <paramref name="maxB"/>]
        /// </summary>
        /// <param name="yes">This exists only to allow two methods with the same name since this method is a float extension</param>
        public static float Map(this float value, float minA, float maxA, float minB, float maxB, bool yes = false) {
            return (value - minA) / (maxA - minA) * (maxB - minB) + minB;
        }

        public static bool Between<TA>(this TA comp, TA min, TA max) where TA : IComparable {
            return comp.CompareTo(min) >= 0 && comp.CompareTo(max) < 0;
        }
    }
}