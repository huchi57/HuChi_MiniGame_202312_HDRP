using System;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public abstract class CameraContributorBase : MonoBehaviour
    {
        [Serializable]
        public enum TargetType
        {
            Player,
            CustomTarget
        }

        [SerializeField]
        protected float m_priority;

        [Space]

        [SerializeField]
        protected TargetType m_targetType;

        [SerializeField, ShowIf(nameof(m_targetType), TargetType.CustomTarget)]
        protected Transform m_customTarget;

        public float Priority => m_priority;

        protected Transform Target
        {
            get
            {
                return m_targetType switch
                {
                    TargetType.CustomTarget => m_customTarget,
                    _ => GameManager.Player,
                };
            }
        }

        public abstract void CalculatePointData(Vector3 currentCameraPosition, Quaternion currentCameraRotation, float currentFOV, float deltaTime, out Vector3 targetCameraPosition, out Quaternion targetCameraRotation, out float targetFOV);
    }
}
