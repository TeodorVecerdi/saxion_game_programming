using System;

namespace Game {
    /// <summary>
    /// Taken from source code of game Rimworld by Ludeon Studios
    /// </summary>
    public class RandomNumberGenerator {
        private const uint Prime1 = 2654435761;
        private const uint Prime2 = 2246822519;
        private const uint Prime3 = 3266489917;
        private const uint Prime4 = 668265263;
        private const uint Prime5 = 374761393;
        public uint seed = (uint) DateTime.Now.GetHashCode();

        public int GetInt(uint iterations) {
            return (int) GetHash((int) iterations);
        }

        public float GetFloat(uint iterations) {
            return (float) ((GetInt(iterations) - (double) int.MinValue) / uint.MaxValue);
        }

        private uint GetHash(int buffer) {
            var num1 = Rotate(seed + 374761393U + 4U + (uint) (buffer * -1028477379), 17) * 668265263U;
            var num2 = (num1 ^ (num1 >> 15)) * 2246822519U;
            var num3 = (num2 ^ (num2 >> 13)) * 3266489917U;
            return num3 ^ (num3 >> 16);
        }

        private static uint Rotate(uint value, int count) {
            return (value << count) | (value >> (32 - count));
        }
    }
}