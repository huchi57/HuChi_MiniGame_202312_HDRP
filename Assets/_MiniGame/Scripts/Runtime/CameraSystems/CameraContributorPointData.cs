using System;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    [Serializable]
    public struct CameraContributorPointData
    {
        public const float DefaultPositionLerpSpeed = 5;
        public const float DefaultRotationSlerpSpeed = 5;
        public const float DefaultFOV = 60;
        public const float DefaultFOVLerpSpeed = 5;

        public Vector3 ReferencePoint;

        // [Header("Position Effector")]
        public Vector3 DistanceFromTargetToCamera;
        public Vector3 PositionOffsetAfterLookAt;
        public float PositionLerpSpeed;

        // [Header("Rotation Effector")]
        public Vector3 LookAtOffsetDistanceFromTarget;
        public float RotationSlerpSpeed;

        public float FOV;
        public float FOVLerpSpeed;
    }
}
