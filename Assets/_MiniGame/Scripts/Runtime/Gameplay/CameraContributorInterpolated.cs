using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class CameraContributorInterpolated : CameraContributorBase
    {
        [Serializable]
        public enum TargetType
        {
            Player,
            CustomTarget
        }

        [Serializable]
        public struct PointData
        {
            public Vector3 ReferencePoint;

            // [Header("Position Effector")]
            public Vector3 DistanceFromTargetToCamera;
            public float PositionLerpSpeed;

            // [Header("Rotation Effector")]
            public Vector3 LookAtOffsetDistanceFromTarget;
            public float RotationSlerpSpeed;

            public float FOV;
            public float FOVLerpSpeed;
        }

        [Space]

        [SerializeField]
        private Space m_referenceSpace = Space.Self;

        [SerializeField]
        private TargetType m_targetType;

        [SerializeField, ShowIf(nameof(m_targetType), TargetType.CustomTarget)]
        private Transform m_customTarget;

        [SerializeField, HideInInspector]
        private PointData[] m_referencePoints;

        private Transform Target
        {
            get
            {
                switch (m_targetType)
                {
                    case TargetType.CustomTarget:
                        return m_customTarget;
                    case TargetType.Player:
                    default:
                        return GameManager.Player;
                }
            }
        }
    }
}
