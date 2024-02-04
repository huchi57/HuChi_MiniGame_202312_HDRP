using UnityEngine;
using UnityEngine.Events;

namespace UrbanFox.MiniGame
{
    public class CustomTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent m_onTriggerEnter;
        [SerializeField] private UnityEvent m_onTriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GameInstance.PlayerTag))
            {
                m_onTriggerEnter?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(GameInstance.PlayerTag))
            {
                m_onTriggerExit?.Invoke();
            }
        }
    }
}
