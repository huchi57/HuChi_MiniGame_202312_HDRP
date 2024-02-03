using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class CameraContributorInterpolated : CameraContributorBase
    {
        [SerializeField]
        private Space m_referenceSpace = Space.Self;

        [SerializeField]
        private bool m_projectReferencePointOnPolygonIfItIsOutside = true;

        [SerializeField, HideInInspector]
        private CameraContributorPointData[] m_referencePoints;

        private CameraContributorPointData m_weightedPointData;
        private Vector3[] m_referencePointsInWorldPosition;
        private float m_commonDenominator;
        private float[] m_distanceBetweenCameraAndReferencePoint;

        public override void CalculatePointData(Vector3 currentBaseCameraPosition, Vector3 currentPostLookAtOffsetCameraPosition, Quaternion currentCameraRotation, float currentFOV, float deltaTime,
            out Vector3 targetBaseCameraPosition, out Vector3 targetPostLookAtOffsetCameraPosition, out Quaternion targetCameraRotation, out float targetFOV, out float positionLerpSpeed, out float rotationSlerpSpeed, out float FOVLerpSpeed)
        {
            if (m_referencePoints.IsNullOrEmpty())
            {
                targetBaseCameraPosition = currentBaseCameraPosition;
                targetPostLookAtOffsetCameraPosition = currentPostLookAtOffsetCameraPosition;
                targetCameraRotation = currentCameraRotation;
                targetFOV = currentFOV;
                positionLerpSpeed = CameraContributorPointData.DefaultPositionLerpSpeed;
                rotationSlerpSpeed = CameraContributorPointData.DefaultRotationSlerpSpeed;
                FOVLerpSpeed = CameraContributorPointData.DefaultFOVLerpSpeed;
                return;
            }

            var referencePoint = m_projectReferencePointOnPolygonIfItIsOutside ? ProjectPointOnPolygonIfItIsOutside(Target.position, m_referencePointsInWorldPosition) : Target.position;

            m_commonDenominator = 0;
            for (int i = 0; i < m_distanceBetweenCameraAndReferencePoint.Length; i++)
            {
                var distance = Vector3.Distance(currentBaseCameraPosition, ConvertPointToWorldSpace(m_referencePoints[i].ReferencePoint));
                if (distance.IsApproximatelyZero())
                {
                    // Directly use the closest point's data if distance = 0
                    targetBaseCameraPosition = referencePoint + m_referencePoints[i].DistanceFromTargetToCamera;
                    targetCameraRotation = Quaternion.LookRotation(referencePoint + m_referencePoints[i].LookAtOffsetDistanceFromTarget - currentBaseCameraPosition);
                    targetPostLookAtOffsetCameraPosition = targetBaseCameraPosition + m_referencePoints[i].PositionOffsetAfterLookAt;
                    targetFOV = m_referencePoints[i].FOV;
                    positionLerpSpeed = m_referencePoints[i].PositionLerpSpeed;
                    rotationSlerpSpeed = m_referencePoints[i].RotationSlerpSpeed;
                    FOVLerpSpeed = m_referencePoints[i].FOVLerpSpeed;
                    return;
                }
                m_distanceBetweenCameraAndReferencePoint[i] = distance;
                m_commonDenominator += 1 / m_distanceBetweenCameraAndReferencePoint[i];
            }

            m_weightedPointData.DistanceFromTargetToCamera = Vector3.zero;
            m_weightedPointData.PositionOffsetAfterLookAt = Vector3.zero;
            m_weightedPointData.PositionLerpSpeed = 0;
            m_weightedPointData.LookAtOffsetDistanceFromTarget = Vector3.zero;
            m_weightedPointData.RotationSlerpSpeed = 0;
            m_weightedPointData.FOV = 0;
            m_weightedPointData.FOVLerpSpeed = 0;

            for (int i = 0; i < m_referencePoints.Length; i++)
            {
                m_weightedPointData.DistanceFromTargetToCamera += m_referencePoints[i].DistanceFromTargetToCamera / m_distanceBetweenCameraAndReferencePoint[i];
                m_weightedPointData.PositionOffsetAfterLookAt += m_referencePoints[i].PositionOffsetAfterLookAt / m_distanceBetweenCameraAndReferencePoint[i];
                m_weightedPointData.PositionLerpSpeed += m_referencePoints[i].PositionLerpSpeed / m_distanceBetweenCameraAndReferencePoint[i];
                m_weightedPointData.LookAtOffsetDistanceFromTarget += m_referencePoints[i].LookAtOffsetDistanceFromTarget / m_distanceBetweenCameraAndReferencePoint[i];
                m_weightedPointData.RotationSlerpSpeed += m_referencePoints[i].RotationSlerpSpeed / m_distanceBetweenCameraAndReferencePoint[i];
                m_weightedPointData.FOV += m_referencePoints[i].FOV / m_distanceBetweenCameraAndReferencePoint[i];
                m_weightedPointData.FOVLerpSpeed += m_referencePoints[i].FOVLerpSpeed / m_distanceBetweenCameraAndReferencePoint[i];
            }

            m_weightedPointData.DistanceFromTargetToCamera /= m_commonDenominator;
            m_weightedPointData.PositionOffsetAfterLookAt /= m_commonDenominator;
            m_weightedPointData.PositionLerpSpeed /= m_commonDenominator;
            m_weightedPointData.LookAtOffsetDistanceFromTarget /= m_commonDenominator;
            m_weightedPointData.RotationSlerpSpeed /= m_commonDenominator;
            m_weightedPointData.FOV /= m_commonDenominator;
            m_weightedPointData.FOVLerpSpeed /= m_commonDenominator;

            targetBaseCameraPosition = referencePoint + m_weightedPointData.DistanceFromTargetToCamera;
            targetCameraRotation = Quaternion.LookRotation(referencePoint + m_weightedPointData.LookAtOffsetDistanceFromTarget - currentBaseCameraPosition);
            targetPostLookAtOffsetCameraPosition = targetBaseCameraPosition + m_weightedPointData.PositionOffsetAfterLookAt;
            targetFOV = m_weightedPointData.FOV;
            positionLerpSpeed = m_weightedPointData.PositionLerpSpeed;
            rotationSlerpSpeed = m_weightedPointData.RotationSlerpSpeed;
            FOVLerpSpeed = m_weightedPointData.FOVLerpSpeed;
        }

        private void Awake()
        {
            m_distanceBetweenCameraAndReferencePoint = new float[m_referencePoints.IsNullOrEmpty() ? 0 : m_referencePoints.Length];
            m_referencePointsInWorldPosition = new Vector3[m_distanceBetweenCameraAndReferencePoint.Length];
            if (!m_referencePoints.IsNullOrEmpty())
            {
                for (int i = 0; i < m_referencePoints.Length; i++)
                {
                    m_referencePointsInWorldPosition[i] = ConvertPointToWorldSpace(m_referencePoints[i].ReferencePoint);
                }
            }
        }

        private Vector3 ConvertPointToWorldSpace(Vector3 point)
        {
            return m_referenceSpace == Space.World ? point : point + transform.position;
        }

        private Vector3 ProjectPointOnPolygonIfItIsOutside(Vector3 point, Vector3[] polygonVertices)
        {
            if (point.IsPointInConvexPolygon(polygonVertices))
            {
                return point;
            }
            return point.ProjectPointOnConvexPolygon(polygonVertices);
        }

        private void OnDrawGizmosSelected()
        {
            if (m_referencePoints.IsNullOrEmpty())
            {
                return;
            }
            foreach (var point in m_referencePoints)
            {
                GizmosExtensions.DrawWireSphere(point.ReferencePoint, 0.15f, Color.white);
                GizmosExtensions.DrawWireSphere(point.ReferencePoint + point.LookAtOffsetDistanceFromTarget, 0.15f, Color.yellow);
                GizmosExtensions.DrawLine(point.ReferencePoint, point.ReferencePoint + point.LookAtOffsetDistanceFromTarget, Color.white);
            }
        }
    }
}
