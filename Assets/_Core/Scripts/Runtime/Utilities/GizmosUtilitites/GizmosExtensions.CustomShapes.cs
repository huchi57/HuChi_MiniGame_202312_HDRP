using UnityEngine;

namespace UrbanFox
{
    public static partial class GizmosExtensions
    {
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
