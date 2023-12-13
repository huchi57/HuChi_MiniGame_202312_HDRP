using UnityEngine;
using UnityEngine.EventSystems;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(EventSystem))]
    public class EventSystemManager : RuntimeManager<EventSystemManager>
    {
        [SerializeField, NonEditable] private EventSystem m_eventSystem;

        private GameObject m_cachedGameObject;

        public void SelectGameObject(GameObject newGameObject)
        {
            if (m_eventSystem.alreadySelecting)
            {
                return;
            }
            m_cachedGameObject = newGameObject;
            m_eventSystem.SetSelectedGameObject(newGameObject);
        }

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
