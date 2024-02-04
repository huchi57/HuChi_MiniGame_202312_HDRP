using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class GameOverTrigger : MonoBehaviour
    {
        [SerializeField] private float m_waitTimeBeforeRestartingWhenGameOverTriggered = 2.5f;
        [SerializeField] private float m_fadeOutTime = 2;
        [SerializeField] private float m_fadeInTime = 2;

        public float WaitTimeBeforeRestartingWhenGameOverTriggered => m_waitTimeBeforeRestartingWhenGameOverTriggered;
        public float FadeOutTime => m_fadeOutTime;
        public float FadeInTime => m_fadeInTime;
    }
}
