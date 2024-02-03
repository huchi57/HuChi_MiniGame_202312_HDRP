using UnityEngine;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Image))]
    public class UIControlSchemeBasedImage : MonoBehaviour
    {
        [SerializeField, NonEditable] private Image m_image;
        [SerializeField] private Sprite m_keyboardAndMouseSprite;
        [SerializeField] private Sprite m_switchSprite;
        [SerializeField] private Sprite m_xInputSprite;
        [SerializeField] private Sprite m_dualSenseSprite;
        [SerializeField] private Sprite m_dualShockSprite;

        private void OnValidate()
        {
            m_image = GetComponent<Image>();
        }

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
            switch (controlScheme)
            {
                case ControlScheme.KeyboardAndMouse:
                    m_image.sprite = m_keyboardAndMouseSprite;
                    break;
                case ControlScheme.Switch:
                    m_image.sprite = m_switchSprite;
                    break;
                case ControlScheme.XInput:
                    m_image.sprite = m_xInputSprite;
                    break;
                case ControlScheme.DualSense:
                    m_image.sprite = m_dualSenseSprite;
                    break;
                case ControlScheme.DualShock:
                    m_image.sprite = m_dualShockSprite;
                    break;
                default:
                    break;
            }
        }
    }
}
