using UnityEngine;

namespace UrbanFox
{
    public static partial class GizmosExtensions
    {
        public static void DrawBounds(Bounds bounds, Color color)
        {
            // Bottom
            var p1 = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
            var p2 = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
            var p3 = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
            var p4 = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);

            // Top
            var p5 = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
            var p6 = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
            var p7 = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);
            var p8 = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);

            using (new GizmosScope(color))
            {
                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p4);
                Gizmos.DrawLine(p4, p1);
                Gizmos.DrawLine(p5, p6);
                Gizmos.DrawLine(p6, p7);
                Gizmos.DrawLine(p7, p8);
                Gizmos.DrawLine(p8, p5);
                Gizmos.DrawLine(p1, p5);
                Gizmos.DrawLine(p2, p6);
                Gizmos.DrawLine(p3, p7);
                Gizmos.DrawLine(p4, p8);
            }
        }

        public static void DrawBounds(Bounds bounds)
        {
            DrawBounds(bounds, Gizmos.color);
        }

        public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius, Color color)
        {
            var forwardDirection = normal.GetPerpendicularVector();
            using (new GizmosScope(center, Quaternion.LookRotation(forwardDirection, normal), radius * Vector3.one, color))
            {
                var p0 = Vector3.forward;
                for (int angle = 0; angle < 360; angle++)
                {
                    var p1 = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
                    Gizmos.DrawLine(p0, p1);
                    p0 = p1;
                }
            }
        }

        public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius)
        {
            DrawWireDisc(center, normal, radius, Gizmos.color);
        }

        public static void DrawWireCylinder(Vector3 surfaceACenter, Vector3 surfaceBCenter, float radius, Color color)
        {
            using (new GizmosScope(color))
            {
                var direction = (surfaceBCenter - surfaceACenter).normalized;
                DrawWireDisc(surfaceACenter, direction, radius);
                DrawWireDisc(surfaceBCenter, direction, radius);

                var perpendicularVector = direction.GetPerpendicularVector();
                Gizmos.DrawLine(surfaceACenter + radius * perpendicularVector, surfaceBCenter + radius * perpendicularVector);
                Gizmos.DrawLine(surfaceACenter - radius * perpendicularVector, surfaceBCenter - radius * perpendicularVector);

                perpendicularVector = Quaternion.AngleAxis(90, direction) * perpendicularVector;
                Gizmos.DrawLine(surfaceACenter + radius * perpendicularVector, surfaceBCenter + radius * perpendicularVector);
                Gizmos.DrawLine(surfaceACenter - radius * perpendicularVector, surfaceBCenter - radius * perpendicularVector);
            }
        }

        public static void DrawWireCylinder(Vector3 surfaceACenter, Vector3 surfaceBCenter, float radius)
        {
            DrawWireCylinder(surfaceACenter, surfaceBCenter, radius, Gizmos.color);
        }

        public static void DrawWireCone(Vector3 baseCenter, float baseRadius, Vector3 coneDirection, float coneHeight, Color color)
        {
            using (new GizmosScope(color))
            {
                var apex = baseCenter + coneHeight * coneDirection;
                DrawWireDisc(baseCenter, coneDirection, baseRadius);
                Gizmos.DrawLine(baseCenter, apex);

                var perpendicularVector = coneDirection.GetPerpendicularVector();
                Gizmos.DrawLine(apex, baseCenter + baseRadius * perpendicularVector);
                Gizmos.DrawLine(apex, baseCenter - baseRadius * perpendicularVector);

                perpendicularVector = Quaternion.AngleAxis(90, coneDirection) * perpendicularVector;
                Gizmos.DrawLine(apex, baseCenter + baseRadius * perpendicularVector);
                Gizmos.DrawLine(apex, baseCenter - baseRadius * perpendicularVector);
            }
        }

        public static void DrawWireCone(Vector3 baseCenter, float baseRadius, Vector3 coneDirection, float coneHeight)
        {
            DrawWireCone(baseCenter, baseRadius, coneDirection, coneHeight, Gizmos.color);
        }
    }
}
