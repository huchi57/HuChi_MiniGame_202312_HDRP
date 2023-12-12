using UnityEngine;
using UnityEditor;

namespace UrbanFox.Editor
{
    public static partial class HandlesExtensions
    {
        public static void DrawWireCylinder(Vector3 surfaceACenter, Vector3 surfaceBCenter, float radius, Color color)
        {
            using (new HandlesScope(color))
            {
                var direction = (surfaceBCenter - surfaceACenter).normalized;
                Handles.DrawWireDisc(surfaceACenter, direction, radius);
                Handles.DrawWireDisc(surfaceBCenter, direction, radius);

                var perpendicularVector = direction.GetPerpendicularVector();
                Handles.DrawLine(surfaceACenter + radius * perpendicularVector, surfaceBCenter + radius * perpendicularVector);
                Handles.DrawLine(surfaceACenter - radius * perpendicularVector, surfaceBCenter - radius * perpendicularVector);

                perpendicularVector = Quaternion.AngleAxis(90, direction) * perpendicularVector;
                Handles.DrawLine(surfaceACenter + radius * perpendicularVector, surfaceBCenter + radius * perpendicularVector);
                Handles.DrawLine(surfaceACenter - radius * perpendicularVector, surfaceBCenter - radius * perpendicularVector);
            }
        }

        public static void DrawWireCylinder(Vector3 surfaceACenter, Vector3 surfaceBCenter, float radius)
        {
            DrawWireCylinder(surfaceACenter, surfaceBCenter, radius, Handles.color);
        }

        public static void DrawWireCone(Vector3 baseCenter, float baseRadius, Vector3 coneDirection, float coneHeight, Color color)
        {
            using (new HandlesScope(color))
            {
                var apex = baseCenter + coneHeight * coneDirection;
                Handles.DrawWireDisc(baseCenter, coneDirection, baseRadius);
                Handles.DrawLine(baseCenter, apex);

                var perpendicularVector = coneDirection.GetPerpendicularVector();
                Handles.DrawLine(apex, baseCenter + baseRadius * perpendicularVector);
                Handles.DrawLine(apex, baseCenter - baseRadius * perpendicularVector);

                perpendicularVector = Quaternion.AngleAxis(90, coneDirection) * perpendicularVector;
                Handles.DrawLine(apex, baseCenter + baseRadius * perpendicularVector);
                Handles.DrawLine(apex, baseCenter - baseRadius * perpendicularVector);
            }
        }

        public static void DrawWireCone(Vector3 baseCenter, float baseRadius, Vector3 coneDirection, float coneHeight)
        {
            DrawWireCone(baseCenter, baseRadius, coneDirection, coneHeight, Handles.color);
        }
    }
}
