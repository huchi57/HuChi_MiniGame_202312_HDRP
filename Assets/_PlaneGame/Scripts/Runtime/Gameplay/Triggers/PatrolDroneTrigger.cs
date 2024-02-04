using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class PatrolDroneTrigger : MonoBehaviour
    {
        [SerializeField] private PatrolDrone m_playerChaser;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GameInstance.PlayerTag) && m_playerChaser)
            {
                m_playerChaser.StartChasingTarget(other.transform);
            }
        }
    }
}
