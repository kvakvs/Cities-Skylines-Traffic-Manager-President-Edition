namespace TrafficManager.Util {
    using System;
    using UnityEngine;

    /// <summary>
    /// Provides static functions for handling floating point values.
    /// </summary>
    public static class FloatUtil {
        /// <summary>A very small value for float comparisons to zero.</summary>
        public const float VERY_SMALL_FLOAT = 1e-12f;

        /// <summary>Checks whether two floats are very close to each other.</summary>
        /// <param name="a">One float.</param>
        /// <param name="b">Another float.</param>
        /// <returns>Are the two floats really close, like by one trillionth close.</returns>
        public static bool NearlyEqual(float a, float b) {
            return Mathf.Abs(a - b) < VERY_SMALL_FLOAT;
        }

        /// <summary>Whether a float is really close to zero, like by one trillionth close.</summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Is close to zero.</returns>
        public static bool IsZero(float value) {
            return Math.Abs(value) < VERY_SMALL_FLOAT;
        }
    }
}