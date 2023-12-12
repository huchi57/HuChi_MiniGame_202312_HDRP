using UnityEngine;

namespace UrbanFox
{
    public static partial class GizmosExtensions
    {
        public static void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            using (new GizmosScope(color))
            {
                Gizmos.DrawLine(from, to);
            }
        }

        public static void DrawLines(Color color, params Vector3[] points)
        {
            if (points != null && points.Length > 0)
            {
                using (new GizmosScope(color))
                {
                    for (int i = 0; i < points.Length - 1; i++)
                    {
                        Gizmos.DrawLine(points[i], points[i + 1]);
                    }
                }
            }
        }

        public static void DrawWireSphere(Vector3 center, float radius, Color color)
        {
            using (new GizmosScope(color))
            {
                Gizmos.DrawWireSphere(center, radius);
            }
        }

        public static void DrawSphere(Vector3 center, float radius, Color color)
        {
            using (new GizmosScope(color))
            {
                Gizmos.DrawSphere(center, radius);
            }
        }

        public static void DrawWireCube(Vector3 center, Quaternion rotation, Vector3 size, Color color)
        {
            using (new GizmosScope(center, rotation, size, color))
            {
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }
        }

        public static void DrawWireCube(Vector3 center, Quaternion rotation, Vector3 size)
        {
            DrawWireCube(center, rotation, size, Gizmos.color);
        }

        public static void DrawWireCube(Vector3 center, Vector3 size, Color color)
        {
            DrawWireCube(center, Quaternion.identity, size, color);
        }

        public static void DrawCube(Vector3 center, Quaternion rotation, Vector3 size, Color color)
        {
            using (new GizmosScope(center, rotation, size, color))
            {
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

        public static void DrawCube(Vector3 center, Quaternion rotation, Vector3 size)
        {
            DrawCube(center, rotation, size, Gizmos.color);
        }

        public static void DrawCube(Vector3 center, Vector3 size, Color color)
        {
            DrawCube(center, Quaternion.identity, size, color);
        }

        public static void DrawFrustum(Vector3 center, float fov, float maxRange, float minRange, float aspect, Color color)
        {
            using (new GizmosScope(color))
            {
                Gizmos.DrawFrustum(center, fov, maxRange, minRange, aspect);
            }
        }

        public static void DrawRay(Ray r, Color color)
        {
            using (new GizmosScope(color))
            {
                Gizmos.DrawRay(r);
            }
        }

        public static void DrawRay(Vector3 from, Vector3 direction, Color color)
        {
            using (new GizmosScope(color))
            {
                Gizmos.DrawRay(from, direction);
            }
        }

        public static void DrawMesh(Mesh mesh, int submeshIndex, Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            using (new GizmosScope(color))
            {
                Gizmos.DrawMesh(mesh, submeshIndex, position, rotation, scale);
            }
        }

        public static void DrawMesh(Mesh mesh, int submeshIndex, Vector3 position, Quaternion rotation, Color color)
        {
            DrawMesh(mesh, submeshIndex, position, rotation, Vector3.one, color);
        }

        public static void DrawMesh(Mesh mesh, int submeshIndex, Vector3 position, Color color)
        {
            DrawMesh(mesh, submeshIndex, position, Quaternion.identity, Vector3.one, color);
        }

        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            DrawMesh(mesh, -1, position, rotation, scale, color);
        }

        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Color color)
        {
            DrawMesh(mesh, -1, position, rotation, Vector3.one, color);
        }

        public static void DrawMesh(Mesh mesh, Vector3 position, Color color)
        {
            DrawMesh(mesh, -1, position, Quaternion.identity, Vector3.one, color);
        }

        public static void DrawMesh(Mesh mesh, Color color)
        {
            DrawMesh(mesh, -1, Vector3.zero, Quaternion.identity, Vector3.one, color);
        }

        public static void DrawWireMesh(Mesh mesh, int submeshIndex, Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            using (new GizmosScope(color))
            {
                Gizmos.DrawWireMesh(mesh, submeshIndex, position, rotation, scale);
            }
        }

        public static void DrawWireMesh(Mesh mesh, int submeshIndex, Vector3 position, Quaternion rotation, Color color)
        {
            DrawMesh(mesh, submeshIndex, position, rotation, Vector3.one, color);
        }

        public static void DrawWireMesh(Mesh mesh, int submeshIndex, Vector3 position, Color color)
        {
            DrawMesh(mesh, submeshIndex, position, Quaternion.identity, Vector3.one, color);
        }

        public static void DrawWireMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            DrawMesh(mesh, -1, position, rotation, scale, color);
        }

        public static void DrawWireMesh(Mesh mesh, Vector3 position, Quaternion rotation, Color color)
        {
            DrawMesh(mesh, -1, position, rotation, Vector3.one, color);
        }

        public static void DrawWireMesh(Mesh mesh, Vector3 position, Color color)
        {
            DrawMesh(mesh, -1, position, Quaternion.identity, Vector3.one, color);
        }

        public static void DrawWireMesh(Mesh mesh, Color color)
        {
            DrawMesh(mesh, -1, Vector3.zero, Quaternion.identity, Vector3.one, color);
        }
    }
}
