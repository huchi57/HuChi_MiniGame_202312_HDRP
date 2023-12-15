using UnityEngine;

namespace UrbanFox.MiniGame
{
    [ExecuteInEditMode]
    public class CameraContributorFixed : CameraContributorBase
    {
        [SerializeField, HideInInspector]
        private CameraContributorPointData m_pointData;

        public override void CalculatePointData(Vector3 currentBaseCameraPosition, Vector3 currentPostLookAtOffsetCameraPosition, Quaternion currentCameraRotation, float currentFOV, float deltaTime,
            out Vector3 targetBaseCameraPosition, out Vector3 targetPostLookAtOffsetCameraPosition, out Quaternion targetCameraRotation, out float targetFOV, out float positionLerpSpeed, out float rotationSlerpSpeed, out float FOVLerpSpeed)
        {
            targetBaseCameraPosition = m_pointData.ReferencePoint + m_pointData.ReferencePoint + m_pointData.DistanceFromTargetToCamera;
            targetCameraRotation = Quaternion.LookRotation(Target.position + m_pointData.LookAtOffsetDistanceFromTarget - currentBaseCameraPosition);
            targetPostLookAtOffsetCameraPosition = targetBaseCameraPosition + m_pointData.PositionOffsetAfterLookAt;
            targetFOV = m_pointData.FOV;
            positionLerpSpeed = m_pointData.PositionLerpSpeed;
            rotationSlerpSpeed = m_pointData.RotationSlerpSpeed;
            FOVLerpSpeed = m_pointData.FOVLerpSpeed;
        }

        private void Update()
        {
            m_pointData.ReferencePoint = transform.position;
        }

        private void Reset()
        {
            m_pointData.ReferencePoint = transform.position;
            m_pointData.DistanceFromTargetToCamera = new Vector3(0, 2, -5);
            m_pointData.PositionOffsetAfterLookAt = Vector3.zero;
            m_pointData.PositionLerpSpeed = CameraContributorPointData.DefaultPositionLerpSpeed;
            m_pointData.LookAtOffsetDistanceFromTarget = new Vector3(0.25f, 0, 0);
            m_pointData.RotationSlerpSpeed = CameraContributorPointData.DefaultRotationSlerpSpeed;
            m_pointData.FOV = CameraContributorPointData.DefaultFOV;
            m_pointData.FOVLerpSpeed = CameraContributorPointData.DefaultFOVLerpSpeed;
        }

        private void OnDrawGizmosSelected()
        {
            GizmosExtensions.DrawWireSphere(m_pointData.ReferencePoint, 0.15f, Color.white);
            GizmosExtensions.DrawWireSphere(m_pointData.ReferencePoint + m_pointData.LookAtOffsetDistanceFromTarget, 0.15f, Color.yellow);
            GizmosExtensions.DrawLine(m_pointData.ReferencePoint, m_pointData.ReferencePoint + m_pointData.LookAtOffsetDistanceFromTarget, Color.white);
        }
    }
}
