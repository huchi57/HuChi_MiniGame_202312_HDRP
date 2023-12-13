using UnityEngine;

namespace UrbanFox.MiniGame
{
    public abstract class CameraContributorBase : MonoBehaviour
    {
        [SerializeField, Range(0, 1)]
        private float m_weight = 1;

        [SerializeField]
        private float m_priority;

        public float Weight => m_weight;
        public float Priority => m_priority;
    }
}
