using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class TimeManager : MonoBehaviour
    {
        private static float m_timeScale = 1;

        [SerializeField] private float m_globalTimeScaleMultiplier = 1;

        public static float TimeScale
        {
            get => m_timeScale;
            set => m_timeScale = value;
        }

        private void LateUpdate()
        {
            Time.timeScale = m_timeScale * m_globalTimeScaleMultiplier;
        }
    }
}
