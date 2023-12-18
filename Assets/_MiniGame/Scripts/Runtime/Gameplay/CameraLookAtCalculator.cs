using System;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class CameraLookAtCalculator : MonoBehaviour
    {
        [Serializable]
        public struct LookAtData
        {
            [Required]
            public Transform WeightPoint;

            [Range(0, 1)]
            public float Weight;
        }

        [SerializeField, Range(0, 1)]
        private float m_weightOfPlayer;

        [SerializeField]
        private LookAtData[] m_lookAtWeights;

        [SerializeField, NonEditable]
        private Transform m_weightedLookAt;

        private Vector3 m_positionSum;
        private float m_weightSum;

        private void OnValidate()
        {
            Awake();
        }

        private void Awake()
        {
            if (m_weightedLookAt == null)
            {
                m_weightedLookAt = new GameObject("CustomFollowTarget").transform;
                m_weightedLookAt.SetParent(transform);
            }
        }

        private void Update()
        {
            if (m_lookAtWeights.IsNullOrEmpty() || !GameManager.Player)
            {
                m_weightedLookAt = GameManager.Player;
                return;
            }

            m_weightSum = m_weightOfPlayer;
            foreach (var data in m_lookAtWeights)
            {
                m_weightSum += data.Weight;
            }

            m_positionSum = (m_weightOfPlayer / m_weightSum) * GameManager.Player.position;
            foreach (var data in m_lookAtWeights)
            {
                m_positionSum += (data.Weight / m_weightSum) * data.WeightPoint.position;
            }

            m_weightedLookAt.position = m_positionSum;
        }
    }
}
