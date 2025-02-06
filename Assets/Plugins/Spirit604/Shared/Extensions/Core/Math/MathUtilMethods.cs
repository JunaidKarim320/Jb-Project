using UnityEngine;

namespace Spirit604.Extensions
{
    public static class MathUtilMethods
    {
        public static Quaternion LookRotationSafe(Vector3 dir)
        {
            return LookRotationSafe(dir, Vector3.up);
        }

        public static Quaternion LookRotationSafe(Vector3 dir, Vector3 upwards)
        {
            if (dir != Vector3.zero)
            {
                return Quaternion.LookRotation(dir, upwards);
            }
            else
            {
                return Quaternion.identity;
            }
        }

        public static uint GetRandomSeed()
        {
            var seed = (uint)UnityEngine.Random.Range(uint.MinValue, uint.MaxValue);

            if (seed == 0)
            {
                return GetRandomSeed();
            }

            return seed;
        }

        public static uint ModifySeed(uint sourceSeed, int index)
        {
            unchecked
            {
                return sourceSeed + (uint)(1337 * index);
            }
        }

        public static void CustomRoundValue(ref float sourceValue, float roundValue)
        {
            if (roundValue != 0)
            {
                sourceValue = Mathf.Round(sourceValue / roundValue) * roundValue;
            }
        }

        public static void CustomRoundVectorValue(ref Vector3 sourceVector, float roundValue)
        {
            if (roundValue > 0)
            {
                CustomRoundValue(ref sourceVector.x, roundValue);
                CustomRoundValue(ref sourceVector.y, roundValue);
                CustomRoundValue(ref sourceVector.z, roundValue);
            }
        }

        public static void CustomRoundOffsetVectorValue(ref Vector3 sourceVector, Vector3 offset, float roundValue, bool includeYAxis = true)
        {
            if (roundValue > 0)
            {
                CustomRoundValue(ref offset.x, roundValue);

                if (includeYAxis)
                {
                    CustomRoundValue(ref offset.y, roundValue);
                }

                CustomRoundValue(ref offset.z, roundValue);
            }

            sourceVector += offset;
        }

        public static void CustomFloorValue(ref float sourceValue, float roundValue)
        {
            if (roundValue != 0)
            {
                sourceValue = Mathf.Floor(sourceValue / roundValue) * roundValue;
            }
        }

        public static void CustomFloorVectorValue(ref Vector3 sourceVector, float roundValue)
        {
            if (roundValue > 0)
            {
                CustomFloorValue(ref sourceVector.x, roundValue);
                CustomFloorValue(ref sourceVector.y, roundValue);
                CustomFloorValue(ref sourceVector.z, roundValue);
            }
        }

        public static float Round(float value, int decimals)
        {
            return (float)System.Math.Round(value, decimals);
        }

        public static bool IsEqual(this float lhs, float rhs)
        {
            return IsEqual(lhs, rhs, Mathf.Epsilon);
        }

        public static bool IsEqual(this float lhs, float rhs, float precision)
        {
            return Mathf.Abs(lhs - rhs) < precision;
        }
    }
}
