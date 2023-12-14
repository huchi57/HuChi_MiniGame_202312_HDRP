using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class CameraContributorInterpolated : CameraContributorBase
    {
        [SerializeField]
        protected Space m_referenceSpace = Space.Self;

        [SerializeField, HideInInspector]
        private CameraContributorPointData[] m_referencePoints;

        private CameraContributorPointData m_weightedPointData;
        private float m_commonDenominator;
        private float[] m_distanceBetweenCameraAndReferencePoint;

        public override void CalculatePointData(Vector3 currentCameraPosition, Quaternion currentCameraRotation, float currentFOV, float deltaTime, out Vector3 targetCameraPosition, out Quaternion targetCameraRotation, out float targetFOV)
        {
            if (m_referencePoints.IsNullOrEmpty())
            {
                targetCameraPosition = currentCameraPosition;
                targetCameraRotation = currentCameraRotation;
                targetFOV = currentFOV;
                return;
            }

            m_commonDenominator = 0;
            for (int i = 0; i < m_distanceBetweenCameraAndReferencePoint.Length; i++)
            {
                var distance = Vector3.Distance(currentCameraPosition, ConvertPointToWorldSpace(m_referencePoints[i].ReferencePoint));
                if (distance.IsApproximatelyZero())
                {
                    // Directly use the closest point's data if distance = 0
                    targetCameraPosition = Vector3.Lerp(currentCameraPosition, Target.position + m_referencePoints[i].DistanceFromTargetToCamera, m_referencePoints[i].PositionLerpSpeed * deltaTime);
                    targetCameraRotation = Quaternion.Slerp(currentCameraRotation, Quaternion.LookRotation(Target.position + m_referencePoints[i].LookAtOffsetDistanceFromTarget - currentCameraPosition), m_referencePoints[i].RotationSlerpSpeed * deltaTime);
                    targetFOV = Mathf.Lerp(currentFOV, m_referencePoints[i].FOV, m_referencePoints[i].FOVLerpSpeed * deltaTime);
                    return;
                }
                m_distanceBetweenCameraAndReferencePoint[i] = distance;
                m_commonDenominator += 1 / m_distanceBetweenCameraAndReferencePoint[i];
            }

            m_weightedPointData.DistanceFromTargetToCamera = Vector3.zero;
            m_weightedPointData.PositionLerpSpeed = 0;
            m_weightedPointData.LookAtOffsetDistanceFromTarget = Vector3.zero;
            m_weightedPointData.RotationSlerpSpeed = 0;
            m_weightedPointData.FOV = 0;
            m_weightedPointData.FOVLerpSpeed = 0;

            for (int i = 0; i < m_referencePoints.Length; i++)
            {
                m_weightedPointData.DistanceFromTargetToCamera += m_referencePoints[i].DistanceFromTargetToCamera / m_distanceBetweenCameraAndReferencePoint[i];
                m_weightedPointData.PositionLerpSpeed += m_referencePoints[i].PositionLerpSpeed / m_distanceBetweenCameraAndReferencePoint[i];
                m_weightedPointData.LookAtOffsetDistanceFromTarget += m_referencePoints[i].LookAtOffsetDistanceFromTarget / m_distanceBetweenCameraAndReferencePoint[i];
                m_weightedPointData.RotationSlerpSpeed += m_referencePoints[i].RotationSlerpSpeed / m_distanceBetweenCameraAndReferencePoint[i];
                m_weightedPointData.FOV += m_referencePoints[i].FOV / m_distanceBetweenCameraAndReferencePoint[i];
                m_weightedPointData.FOVLerpSpeed += m_referencePoints[i].FOVLerpSpeed / m_distanceBetweenCameraAndReferencePoint[i];
            }

            m_weightedPointData.DistanceFromTargetToCamera /= m_commonDenominator;
            m_weightedPointData.PositionLerpSpeed /= m_commonDenominator;
            m_weightedPointData.LookAtOffsetDistanceFromTarget /= m_commonDenominator;
            m_weightedPointData.RotationSlerpSpeed /= m_commonDenominator;
            m_weightedPointData.FOV /= m_commonDenominator;
            m_weightedPointData.FOVLerpSpeed /= m_commonDenominator;

            targetCameraPosition = Vector3.Lerp(currentCameraPosition, Target.position + m_weightedPointData.DistanceFromTargetToCamera, m_weightedPointData.PositionLerpSpeed * deltaTime);
            targetCameraRotation = Quaternion.Slerp(currentCameraRotation, Quaternion.LookRotation(Target.position + m_weightedPointData.LookAtOffsetDistanceFromTarget - currentCameraPosition), m_weightedPointData.RotationSlerpSpeed * deltaTime);
            targetFOV = Mathf.Lerp(currentFOV, m_weightedPointData.FOV, m_weightedPointData.FOVLerpSpeed * deltaTime);
        }

        private void Awake()
        {
            m_distanceBetweenCameraAndReferencePoint = new float[m_referencePoints.IsNullOrEmpty() ? 0 : m_referencePoints.Length];
        }

        private Vector3 ConvertPointToWorldSpace(Vector3 point)
        {
            return m_referenceSpace == Space.World ? point : point + transform.position;
        }
    }
}
