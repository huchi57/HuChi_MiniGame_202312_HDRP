using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace UrbanFox.MiniGame
{
    public class CameraBrain : MonoBehaviour
    {
        [Serializable]
        public enum UpdateType
        {
            Update,
            LateUpdate,
            FixedUpdate
        }

        public static CameraBrain Main { get; private set; }

        private readonly List<CameraContributorBase> m_cameraContributors = new List<CameraContributorBase>();

        [SerializeField, Required]
        private CinemachineVirtualCamera m_cinemachineVirtualCamera;

        [SerializeField]
        private UpdateType m_updateType;

        [SerializeField, Info("How fast do point data value changes.")]
        private float m_pointDataValueChangeRate = 5;

        [Header("Default Values")]

        [SerializeField]
        private float m_defaultPositionLerpSpeed = 5;

        [SerializeField]
        private float m_defaultRotationSlerpSpeed = 5;

        [SerializeField]
        private float m_defaultFOV = 60;

        [SerializeField]
        private float m_defaultFOVLerpSpeed = 5;

        [Header("Deceleration Settings")]

        [SerializeField]
        private AnimationCurve m_positionDeclerationCurve;

        [SerializeField]
        private AnimationCurve m_rotationDecelerationCurve;

        [SerializeField]
        private AnimationCurve m_FOVDecelerationCurve;

        private Vector3 m_lastFrameCameraPosition;
        private Vector3 m_cameraDeltaPosition;

        private Vector3 m_lastFrameCameraAngle;
        private Vector3 m_cameraDeltaAngle;

        private float m_lastFrameFOV;
        private float m_cameraDeltaFOV;

        public void AddContributor(CameraContributorBase contributor)
        {
            if (contributor && !m_cameraContributors.Contains(contributor))
            {
                m_cameraContributors.Add(contributor);
            }
        }

        public void RemoveContributor(CameraContributorBase contributor)
        {
            if (contributor && m_cameraContributors.Contains(contributor))
            {
                m_cameraContributors.Remove(contributor);
            }
        }

        public void ClearAllContributors()
        {
            m_cameraContributors.Clear();
        }

        private void Awake()
        {
            m_lastFrameCameraPosition = transform.position;
            m_lastFrameCameraAngle = transform.eulerAngles;
            m_lastFrameFOV = m_cinemachineVirtualCamera.m_Lens.FieldOfView;
            if (Main)
            {
                FoxyLogger.LogWarning($"A duplicated camera has been found. Only one should be present.");
            }
            Main = this;
        }

        private void Update()
        {
            if (m_updateType == UpdateType.Update)
            {
                UpdateCamera(Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (m_updateType == UpdateType.LateUpdate)
            {
                UpdateCamera(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (m_updateType == UpdateType.FixedUpdate)
            {
                UpdateCamera(Time.fixedDeltaTime);
            }
        }

        private void UpdateCamera(float deltaTime)
        {
            if (m_cameraContributors.IsNullOrEmpty())
            {
                transform.position += m_cameraDeltaPosition;
                m_cameraDeltaPosition = Vector3.Lerp(m_cameraDeltaPosition, Vector3.zero, m_positionDeclerationCurve.Evaluate(m_cameraDeltaPosition.magnitude / deltaTime) * deltaTime);

                transform.eulerAngles += m_cameraDeltaAngle;
                m_cameraDeltaAngle = Vector3.Slerp(m_cameraDeltaAngle, Vector3.zero, m_rotationDecelerationCurve.Evaluate(m_cameraDeltaAngle.magnitude / deltaTime) * deltaTime);

                m_cinemachineVirtualCamera.m_Lens.FieldOfView += m_cameraDeltaFOV;
                m_cameraDeltaFOV = Mathf.Lerp(m_cameraDeltaFOV, 0, m_FOVDecelerationCurve.Evaluate(m_cameraDeltaFOV / deltaTime) * deltaTime);

                return;
            }

            m_cameraDeltaPosition = transform.position - m_lastFrameCameraPosition;
            m_lastFrameCameraPosition = transform.position;

            m_cameraDeltaAngle = transform.eulerAngles - m_lastFrameCameraAngle;
            m_lastFrameCameraAngle = transform.eulerAngles;

            m_cameraDeltaFOV = m_cinemachineVirtualCamera.m_Lens.FieldOfView - m_lastFrameFOV;
            m_lastFrameFOV = m_cinemachineVirtualCamera.m_Lens.FieldOfView;

            var targetContributor = GetHighestPriorityContributorFromList(m_cameraContributors);
            if (!targetContributor)
            {
                return;
            }

            targetContributor.CalculatePointData(transform.position, transform.rotation, m_cinemachineVirtualCamera.m_Lens.FieldOfView, deltaTime, out var targetCameraPosition, out var targetCameraRotation, out var targetFOV);
            transform.SetPositionAndRotation(Vector3.Lerp(transform.position, targetCameraPosition, m_defaultPositionLerpSpeed * deltaTime),
                Quaternion.Slerp(transform.rotation, targetCameraRotation, m_defaultRotationSlerpSpeed * deltaTime));
            m_cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(m_cinemachineVirtualCamera.m_Lens.FieldOfView, targetFOV, m_defaultFOVLerpSpeed * deltaTime);
        }

        private CameraContributorBase GetHighestPriorityContributorFromList(List<CameraContributorBase> list)
        {
            if (list.IsNullOrEmpty())
            {
                return null;
            }
            if (list.Count == 1)
            {
                return list[0];
            }
            if (list[0] == null)
            {
                return null;
            }
            var result = list[0];
            for(int i = 1; i < list.Count; i++)
            {
                if (list[i] && list[i].Priority > result.Priority)
                {
                    result = list[i];
                }
            }
            return result;
        }
    }
}
