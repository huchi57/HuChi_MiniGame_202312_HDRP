using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class ControlSchemeBasedObjectSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject m_keyboardAndMouseObject;
        [SerializeField] private GameObject m_switchObject;
        [SerializeField] private GameObject m_xInputObject;
        [SerializeField] private GameObject m_dualSenseObject;
        [SerializeField] private GameObject m_dualShockObject;

        private void Start()
        {
            InputDeviceManager.OnControlSchemeChanged += OnControlSchemeChanged;
        }

        private void OnDestroy()
        {
            InputDeviceManager.OnControlSchemeChanged -= OnControlSchemeChanged;
        }

        private void OnControlSchemeChanged(ControlScheme controlScheme)
        {
            if (m_keyboardAndMouseObject)
            {
                m_keyboardAndMouseObject.gameObject.SetActive(controlScheme == ControlScheme.KeyboardAndMouse);
            }
            if (m_switchObject)
            {
                m_switchObject.gameObject.SetActive(controlScheme == ControlScheme.Switch);
            }
            if (m_xInputObject)
            {
                m_xInputObject.gameObject.SetActive(controlScheme == ControlScheme.XInput);
            }
            if (m_dualSenseObject)
            {
                m_dualSenseObject.gameObject.SetActive(controlScheme == ControlScheme.DualSense);
            }
            if (m_dualShockObject)
            {
                m_dualShockObject.gameObject.SetActive(controlScheme == ControlScheme.DualShock);
            }
        }
    }
}
