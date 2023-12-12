using UnityEngine;

namespace UrbanFox
{
    public static class MathExtensions
    {
        private const float m_neglectableValue = 0.00001f;

        public static bool IsInRange(this int value, int rangeStart, int rangeEnd, bool minInclusive = true, bool maxInclusive = true)
        {
            int min = Mathf.Min(rangeStart, rangeEnd);
            int max = Mathf.Max(rangeStart, rangeEnd);

            if (minInclusive && maxInclusive)
            {
                return min <= value && value <= max;
            }

            if (minInclusive && !maxInclusive)
            {
                return min <= value && value < max;
            }

            if (!minInclusive && maxInclusive)
            {
                return min < value && value <= max;
            }

            return min < value && value < max;
        }

        public static bool IsInRange(this float value, float rangeStart, float rangeEnd, bool minInclusive = true, bool maxInclusive = true)
        {
            float min = Mathf.Min(rangeStart, rangeEnd);
            float max = Mathf.Max(rangeStart, rangeEnd);

            if (minInclusive && maxInclusive)
            {
                return min <= value && value <= max;
            }

            if (minInclusive && !maxInclusive)
            {
                return min <= value && value < max;
            }

            if (!minInclusive && maxInclusive)
            {
                return min < value && value <= max;
            }

            return min < value && value < max;
        }

        /// <summary>
        /// Clamps an angle in range [0, 360].
        /// </summary>
        public static float Angle360(this float angle)
        {
            if (angle > 360)
            {
                return Angle360(angle - 360);
            }

            if (angle < 0)
            {
                return Angle360(angle + 360);
            }

            return angle;
        }

        /// <summary>
        /// Clamps an angle in range [-180, 180].
        /// </summary>
        public static float AnglePositiveOrNegative180(this float angle)
        {
            if (angle > 180)
            {
                return AnglePositiveOrNegative180(angle - 360);
            }

            if (angle < -180)
            {
                return AnglePositiveOrNegative180(angle + 360);
            }

            return angle;
        }

        public static Vector3 ChangeLength(this Vector3 vector, float newLength)
        {
            return newLength * vector.normalized;
        }

        public static Vector3 RotateVectorAlongAxis(this Vector3 vector, Vector3 axis, float angle)
        {
            return Quaternion.AngleAxis(angle, axis) * vector;
        }

        public static bool IsApproximately(this float value1, float value2)
        {
            return Mathf.Abs(value2 - value1) < Mathf.Max(m_neglectableValue * Mathf.Max(Mathf.Abs(value1), Mathf.Abs(value2)), 8 * float.Epsilon);
        }

        public static bool IsApproximatelyZero(this float value)
        {
            return value.IsApproximately(0);
        }

        public static bool IsApproximately(this Vector2 value1, Vector2 value2)
        {
            return value1.x.IsApproximately(value2.x) && value1.y.IsApproximately(value2.y);
        }

        public static bool IsApproximately(this Vector3 value1, Vector3 value2)
        {
            return value1.x.IsApproximately(value2.x) && value1.y.IsApproximately(value2.y) && value1.z.IsApproximately(value2.z);
        }

        public static bool IsApproximately(this Vector4 value1, Vector4 value2)
        {
            return value1.x.IsApproximately(value2.x) && value1.y.IsApproximately(value2.y) && value1.z.IsApproximately(value2.z) && value1.w.IsApproximately(value2.w);
        }

        public static bool IsApproximately(this Color value1, Color value2)
        {
            return value1.r.IsApproximately(value2.r) && value1.g.IsApproximately(value2.g) && value1.b.IsApproximately(value2.b) && value1.a.IsApproximately(value2.a);
        }

        /// <summary>
        /// Get a perpendicular vector from a given normal.
        /// </summary>
        public static Vector3 GetPerpendicularVector(this Vector3 normal)
        {
            var normalMagnitude = normal.magnitude;
            if (normalMagnitude.IsApproximatelyZero())
            {
                return Vector3.zero;
            }

            var projectVector = Vector3.ProjectOnPlane(Vector3.forward, normal);

            // If a project vector is 0, then the surface is a horizontal plane.
            return projectVector.magnitude.IsApproximatelyZero() ? normalMagnitude * Vector3.up : projectVector.ChangeLength(normalMagnitude);
        }

        /// <summary>
        /// Get a random perpendicular vector from a given normal.
        /// </summary>
        public static Vector3 GetRandomPerpendicularVector(this Vector3 normal)
        {
            if (normal.magnitude.IsApproximatelyZero())
            {
                return Vector3.zero;
            }

            var randomUnitVector = Random.onUnitSphere;
            var projectVector = Vector3.ProjectOnPlane(randomUnitVector, normal);

            // (Brute-force, unlikely) Get another random perpendicular vector again if the cross product is parallel to the normal.
            return projectVector.magnitude.IsApproximatelyZero() ? normal.GetRandomPerpendicularVector() : projectVector.ChangeLength(normal.magnitude);
        }

        #region Vector value helpers
        public static Vector2 SetX(this Vector2 vector, float x)
        {
            return new Vector2(x, vector.y);
        }

        public static Vector2 SetY(this Vector2 vector, float y)
        {
            return new Vector2(vector.x, y);
        }

        public static Vector2Int SetX(this Vector2Int vector, int x)
        {
            return new Vector2Int(x, vector.y);
        }

        public static Vector2Int SetY(this Vector2Int vector, int y)
        {
            return new Vector2Int(vector.x, y);
        }

        public static Vector3 SetX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        public static Vector3 SetY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        public static Vector3 SetZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static Vector3Int SetX(this Vector3Int vector, int x)
        {
            return new Vector3Int(x, vector.y, vector.z);
        }

        public static Vector3Int SetY(this Vector3Int vector, int y)
        {
            return new Vector3Int(vector.x, y, vector.z);
        }

        public static Vector3Int SetZ(this Vector3Int vector, int z)
        {
            return new Vector3Int(vector.x, vector.y, z);
        }

        public static Vector4 SetX(this Vector4 vector, float x)
        {
            return new Vector4(x, vector.y, vector.z, vector.w);
        }

        public static Vector4 SetY(this Vector4 vector, float y)
        {
            return new Vector4(vector.x, y, vector.z, vector.w);
        }

        public static Vector4 SetZ(this Vector4 vector, float z)
        {
            return new Vector4(vector.x, vector.y, z, vector.w);
        }

        public static Vector4 SetW(this Vector4 vector, float w)
        {
            return new Vector4(vector.x, vector.y, vector.z, w);
        }
        #endregion
    }
}
