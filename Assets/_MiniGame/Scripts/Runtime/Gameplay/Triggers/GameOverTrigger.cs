using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class GameOverTrigger : MonoBehaviour
    {
        [SerializeField] private float m_waitTimeBeforeRestartingWhenGameOverTriggered = 2.5f;

        public float WaitTimeBeforeRestartingWhenGameOverTriggered => m_waitTimeBeforeRestartingWhenGameOverTriggered;
    }
}
