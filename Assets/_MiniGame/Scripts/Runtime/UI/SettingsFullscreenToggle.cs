using UnityEngine;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Toggle))]
    public class SettingsFullscreenToggle : MonoBehaviour
    {
        [SerializeField, NonEditable] private Toggle m_toggle;

        private void OnValidate()
        {
            m_toggle = GetComponent<Toggle>();
        }

        private void Awake()
        {
            m_toggle.isOn = SettingsManager.Instance.IsFullscreen;
            m_toggle.onValueChanged.AddListener(EnableFullscreen);
            EnableFullscreen(m_toggle.isOn);
        }

        private void OnDestroy()
        {
            m_toggle.onValueChanged.RemoveListener(EnableFullscreen);
        }

        private void EnableFullscreen(bool value)
        {
            SettingsManager.Instance.IsFullscreen = value;
            Screen.fullScreen = value;
        }
    }
}
