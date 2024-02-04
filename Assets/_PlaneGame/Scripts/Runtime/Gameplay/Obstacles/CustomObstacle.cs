using UnityEngine;
using UnityEngine.Events;

namespace UrbanFox.MiniGame
{
    public class CustomObstacle : MonoBehaviour
    {
        [SerializeField]
        private bool m_comparePlayerTag = true;

        [SerializeField, ShowIf(nameof(m_comparePlayerTag), false)]
        private string m_customTag = "Untagged";

        [SerializeField]
        private UnityEvent m_onCollisionEnter;

        [SerializeField]
        private bool m_invokeOnlyOnce = true;

        private bool m_invoked;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(m_comparePlayerTag ? GameInstance.PlayerTag : m_customTag))
            {
                if (!m_invokeOnlyOnce || !m_invoked)
                {
                    m_invoked = true;
                    m_onCollisionEnter?.Invoke();
                }
            }
        }
    }
}
