using UnityEngine;
using UnityEditor;

namespace UrbanFox.Editor
{
    public static partial class HandlesExtensions
    {
        public static void DrawPolyLine(Color color, params Vector3[] points)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawPolyLine(points);
            }
        }

        public static void DrawLine(Vector3 p1, Vector3 p2, Color color)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawLine(p1, p2);
            }
        }

        public static void DrawLine(Vector3 p1, Vector3 p2, float thickness, Color color)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawLine(p1, p2, Mathf.Max(0, thickness));
            }
        }

        public static void DrawLines(Vector3[] lineSegments, Color color)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawLines(lineSegments);
            }
        }

        public static void DrawLines(Vector3[] points, int[] segmentIndices, Color color)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawLines(points, segmentIndices);
            }
        }

        public static void DrawDottedLine(Vector3 p1, Vector3 p2, float screenSpaceSize, Color color)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawDottedLine(p1, p2, screenSpaceSize);
            }
        }

        public static void DrawDottedLines(Vector3[] lineSegments, float screenSpaceSize, Color color)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawDottedLines(lineSegments, screenSpaceSize);
            }
        }

        public static void DrawDottedLines(Vector3[] points, int[] segmentIndices, float screenSpaceSize, Color color)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawDottedLines(points, segmentIndices, screenSpaceSize);
            }
        }

        public static void DrawWireCube(Vector3 center, Quaternion rotation, Vector3 size, Color color)
        {
            using (new HandlesScope(center, rotation, size, color))
            {
                Handles.DrawWireCube(Vector3.zero, Vector3.one);
            }
        }

        public static void DrawWireCube(Vector3 center, Quaternion rotation, Vector3 size)
        {
            DrawWireCube(center, rotation, size, Handles.color);
        }

        public static void DrawWireCube(Vector3 center, Vector3 size, Color color)
        {
            DrawWireCube(center, Quaternion.identity, size, color);
        }

        public static void DrawAAPolyLine(Color color, params Vector3[] points)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawAAPolyLine(points);
            }
        }

        public static void DrawAAPolyLine(float width, Color color, params Vector3[] points)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawAAPolyLine(width, points);
            }
        }

        public static void DrawAAPolyLine(float width, int actualNumberOfPoints, Color color, params Vector3[] points)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawAAPolyLine(width, actualNumberOfPoints, points);
            }
        }

        public static void DrawAAConvexPolygon(Color color, params Vector3[] points)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawAAConvexPolygon(points);
            }
        }

        public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius, Color color)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawWireDisc(center, normal, radius);
            }
        }

        public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius, float thickness, Color color)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawWireDisc(center, normal, radius, thickness);
            }
        }

        public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, Color color)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawWireArc(center, normal, from, angle, radius);
            }
        }

        public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, float thickness, Color color)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawWireArc(center, normal, from, angle, radius, thickness);
            }
        }

        public static void DrawSolidDisc(Vector3 center, Vector3 normal, float radius, Color color)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawSolidDisc(center, normal, radius);
            }
        }

        public static void DrawSolidArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, Color color)
        {
            using (new HandlesScope(color))
            {
                Handles.DrawSolidArc(center, normal, from, angle, radius);
            }
        }
    }
}
