using UnityEngine;

namespace Barbaragno.RuntimePackages.Operations {
    /// <summary>
    /// Criteria for rounding a value.
    /// </summary>
    public enum RoundType {
        Round,
        Ceil,
        Floor
    }

    /// <summary>
    /// Utility Class for enhanced readability of <see cref="Mathf"/> functions through extension.
    /// <para>Also provides additional functions for advanced operations.</para>
    /// </summary>
    public static class MathOps {
        #region Float Functions
        /// <summary>
        /// <see cref="Mathf.Abs(float)"/>
        /// </summary>
        public static float Abs(this float value) => Mathf.Abs(value);

        /// <summary>
        /// Returns <paramref name="value"/> clamped up to <paramref name="maxValue"/>.
        /// </summary>
        public static float Max(this float value, float maxValue) => Mathf.Min(value, maxValue);

        /// <summary>
        /// Returns <paramref name="value"/> clamped down to <paramref name="minValue"/>.
        /// </summary>
        public static float Min(this float value, float minValue) => Mathf.Max(value, minValue);

        /// <summary>
        /// <see cref="Mathf.Sign(float)"/>
        /// </summary>
        public static float Sign(this float value) => Mathf.Sign(value);

        /// <summary>
        /// <see cref="Mathf.Round(float)"/> OR <see cref="Mathf.Ceil(float)"/> OR <see cref="Mathf.Floor(float)"/>
        /// </summary>
        public static float Round(this float value, RoundType roundType = RoundType.Round) {
            switch (roundType) {
                case RoundType.Round:
                    return Mathf.Round(value);
                case RoundType.Ceil:
                    return Mathf.Ceil(value);
                case RoundType.Floor:
                    return Mathf.Floor(value);
                default:
                    return Mathf.Round(value);
            }
        }

        /// <summary>
        /// <see cref="Mathf.Round(float)"/> OR <see cref="Mathf.Ceil(float)"/> OR <see cref="Mathf.Floor(float)"/> with limited amount of decimals.
        /// </summary>
        public static float Round(this float value, int decimals, RoundType roundType = RoundType.Round) {
            uint decs = 1;
            for (int i = 0; i < decimals; i++) {
                decs *= 10;
            }

            switch (roundType) {
                case RoundType.Round:
                    return Mathf.Round(value * decs) / decs;
                case RoundType.Ceil:
                    return Mathf.Ceil(value * decs) / decs;
                case RoundType.Floor:
                    return Mathf.Floor(value * decs) / decs;
                default:
                    return Mathf.Round(value * decs) / decs;
            }
        }

        /// <summary>
        /// <see cref="Mathf.Repeat(float, float)"/> with min value.
        /// </summary>
        public static float Loop(this float value, float min, float max) {
            if (value >= max) {
                value = min + (value - max);
                return value;
            }
            else if (value <= min) {
                value = max - (min - value);
                return value;
            }
            else {
                return value;
            }
        }
        #endregion

        #region Int Functions
        /// <summary>
        /// <see cref="Mathf.Abs(int)"/>
        /// </summary>
        public static int Abs(this int value) => Mathf.Abs(value);

        /// <summary>
        /// <see cref="Mathf.Sign(float)"/> using int values.
        /// </summary>
        public static int Sign(this int value) => value >= 0 ? 1 : -1;

        /// <summary>
        /// <see cref="Mathf.Sign(float)"/> using <see cref="float"/> but returning <see cref="int"/>.
        /// </summary>
        public static int SignToInt(this float value) => value >= 0 ? 1 : -1;

        /// <summary>
        /// <see cref="Mathf.Sign(float)"/> using <see cref="float"/> but returning <see cref="int"/>.
        /// </summary>
        public static int SignToInt(this float value, float threshold, int returnValue) => value >= threshold ? 1 : value <= -threshold ? -1 : returnValue;

        public static int SignToIntOrZero(this float value) {
            if (value > Mathf.Epsilon)
                return 1;

            if (value < -Mathf.Epsilon)
                return -1;

            return 0;
        }

        /// <summary>
        /// <see cref="Mathf.RoundToInt(float)"/> OR <see cref="Mathf.CeilToInt(float)"/> OR <see cref="Mathf.FloorToInt(float)"/> using <see cref="int"/> arguments.
        /// </summary>
        public static int Round(this int value, RoundType roundType = RoundType.Round) {
            switch (roundType) {
                case RoundType.Round:
                    return Mathf.RoundToInt(value);
                case RoundType.Ceil:
                    return Mathf.CeilToInt(value);
                case RoundType.Floor:
                    return Mathf.FloorToInt(value);
                default:
                    return Mathf.RoundToInt(value);
            }
        }

        /// <summary>
        /// <see cref="Mathf.RoundToInt(float)"/> OR <see cref="Mathf.CeilToInt(float)"/> OR <see cref="Mathf.FloorToInt(float)"/>
        /// </summary>
        public static int RoundToInt(this float value, RoundType roundType = RoundType.Round) {
            switch (roundType) {
                case RoundType.Round:
                    return Mathf.RoundToInt(value);
                case RoundType.Ceil:
                    return Mathf.CeilToInt(value);
                case RoundType.Floor:
                    return Mathf.FloorToInt(value);
                default:
                    return Mathf.RoundToInt(value);
            }
        }

        /// <summary>
        /// <see cref="Mathf.Repeat(float, float)"/> returning an <see cref="int"/>.
        /// </summary>
        public static int Repeat(this int value, int range) => Mathf.Repeat(value, range).RoundToInt();

        /// <summary>
        /// <see cref="Mathf.Repeat(float, float)"/> with min value and returning <see cref="int"/>.
        /// </summary>
        public static int Loop(this int value, int min, int max) {
            if (value >= max) {
                value = min + (value - max);
                return value;
            }
            else if (value < min) {
                value = max - (min - value);
                return value;
            }
            else {
                return value;
            }
        }

        public static int Cycle(this int value, int min, int max) {
            if (value >= max) {
                return min + (value - max);
            }
            else if (value < min) {
                return max - (min - value);
            }
            else {
                return value;
            }
        }
        #endregion

        #region Advanced Functions
        /// <summary>
        /// Calculates the Factorial of a given value.
        /// </summary>
        /// <param name="val">The start value of the Factorial operation.</param>
        /// <returns>The Factorial value of <paramref name="val"/>.</returns>
        public static float Factorial(float val) {
            if (val <= 1)
                return 1;
            else
                return val * Factorial(val - 1);
        }

        /// <summary>
        /// Calculates the Combination of two given values.
        /// </summary>
        /// <param name="n">The first value of the Combination.</param>
        /// <param name="k">The second value of the Combination.</param>
        /// <returns>The Combination between <paramref name="n"/> and <paramref name="k"/>.</returns>
        public static float Combination(float n, float k) {
            if (n <= 1)
                return 1;
            else
                return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }

        /// <summary>
        /// Calculates the Binomial Probability using the given values.
        /// </summary>
        /// <param name="n">Number of attempts.</param>
        /// <param name="s">Number of successes.</param>
        /// <param name="p">Probability of success.</param>
        /// <returns>The Binomial value resulted with the given values.</returns>
        public static float BinomialProbability(int n, int s, float p) {
            float pF = 1 - p;

            float c = Combination(n, s);
            float px = Mathf.Pow(p, s);
            float qnx = Mathf.Pow(pF, n - s);

            return c * px * qnx;
        }
        #endregion
    }
}