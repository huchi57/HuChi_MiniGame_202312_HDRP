using UnityEngine;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Slider))]
    public class SettingsVolumeSlider : MonoBehaviour
    {
        [SerializeField, NonEditable]
        private Slider m_slider;

        private void OnValidate()
        {
            m_slider = GetComponent<Slider>();
        }

        private void Awake()
        {
            m_slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void OnDestroy()
        {
            m_slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        private void OnEnable()
        {
            m_slider.value = SettingsManager.Instance.Volume * 10;
        }

        private void OnSliderValueChanged(float value)
        {
            SettingsManager.Instance.Volume = value / 10;
            AudioManager.Instance.SetMasterVolume(value);
        }
    }
}
