using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    public class SettingsGammaAdjuster : MonoBehaviour
    {
        [SerializeField, Required]
        private Volume m_gammaSettingsVolume;

        [SerializeField, Required]
        private Slider m_slider;

        private LiftGammaGain m_runtimeGamma;

        private void Start()
        {
            var runtimeProfile = m_gammaSettingsVolume.profile;
            runtimeProfile.TryGet(out m_runtimeGamma);
            m_slider.value = SettingsManager.Instance.Brightness;
            m_slider.onValueChanged.AddListener(ChangeGammaFromSliderValue);
            ChangeGammaFromSliderValue(m_slider.value);
        }

        private void OnDestroy()
        {
            m_slider.onValueChanged.RemoveListener(ChangeGammaFromSliderValue);
        }

        public void ChangeGammaFromSliderValue(float value)
        {
            m_runtimeGamma.gamma.value = SettingsManager.Instance.SliderToActualGammaCurve.Evaluate(value) * Vector4.one;
            SettingsManager.Instance.Brightness = value;
        }
    }
}
