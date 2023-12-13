using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    public class UIToggleConnector : MonoBehaviour
    {
        [SerializeField, Required] private Toggle m_toggle;

        private void Update()
        {
            if (InputManager.Submit.WasPressedThisFrame && EventSystem.current && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                m_toggle.isOn = !m_toggle.isOn;
            }
        }
    }
}
