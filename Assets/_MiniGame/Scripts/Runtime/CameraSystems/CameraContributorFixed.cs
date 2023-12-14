using UnityEngine;

namespace UrbanFox.MiniGame
{
    [ExecuteInEditMode]
    public class CameraContributorFixed : CameraContributorBase
    {
        [SerializeField, HideInInspector]
        private CameraContributorPointData m_pointData;

        public override void CalculatePointData(Vector3 currentCameraPosition, Quaternion currentCameraRotation, float currentFOV, float deltaTime, out Vector3 targetCameraPosition, out Quaternion targetCameraRotation, out float targetFOV)
        {
            targetCameraPosition = Vector3.Lerp(currentCameraPosition, m_pointData.ReferencePoint + m_pointData.DistanceFromTargetToCamera, m_pointData.PositionLerpSpeed * deltaTime);
            targetCameraRotation = Quaternion.Slerp(currentCameraRotation, Quaternion.LookRotation(Target.position + m_pointData.LookAtOffsetDistanceFromTarget - currentCameraPosition), m_pointData.RotationSlerpSpeed * deltaTime);
            targetFOV = Mathf.Lerp(currentFOV, m_pointData.FOV, deltaTime * m_pointData.FOVLerpSpeed);
        }

        private void Update()
        {
            m_pointData.ReferencePoint = transform.position;
        }

        private void Reset()
        {
            m_pointData.ReferencePoint = transform.position;
            m_pointData.DistanceFromTargetToCamera = new Vector3(0, 2, -5);
            m_pointData.PositionLerpSpeed = CameraContributorPointData.DefaultPositionLerpSpeed;
            m_pointData.LookAtOffsetDistanceFromTarget = new Vector3(0.25f, 0, 0);
            m_pointData.RotationSlerpSpeed = CameraContributorPointData.DefaultRotationSlerpSpeed;
            m_pointData.FOV = CameraContributorPointData.DefaultFOV;
            m_pointData.FOVLerpSpeed = CameraContributorPointData.DefaultFOVLerpSpeed;
        }
    }
}
