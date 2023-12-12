using UnityEngine;
using UnityEngine.EventSystems;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(EventSystem))]
    public class EventSystemAutoSelectCacheWhenLoseFocus : MonoBehaviour
    {
        [SerializeField, NonEditable] private EventSystem m_eventSystem;

        private GameObject m_cachedGameObject;

        private void OnValidate()
        {
            m_eventSystem = GetComponent<EventSystem>();
        }

        private void Update()
        {
            if (m_eventSystem.currentSelectedGameObject != m_cachedGameObject && m_eventSystem.currentSelectedGameObject)
            {
                m_cachedGameObject = m_eventSystem.currentSelectedGameObject;
            }
            if (!m_eventSystem.currentSelectedGameObject && m_cachedGameObject)
            {
                m_eventSystem.SetSelectedGameObject(m_cachedGameObject);
            }
        }
    }
}
