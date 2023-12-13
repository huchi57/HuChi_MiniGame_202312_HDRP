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

        [SerializeField]
        private AnimationCurve m_sliderToActualGamaCurve = AnimationCurve.Linear(-1, -0.1f, 1, 0.1f);

        private LiftGammaGain m_runtimeGamma;

        private void Awake()
        {
            var runtimeProfile = m_gammaSettingsVolume.profile;
            runtimeProfile.TryGet(out m_runtimeGamma);
            m_slider.onValueChanged.AddListener(ChangeGammaFromSliderValue);
            ChangeGammaFromSliderValue(m_slider.value);
        }

        private void OnDestroy()
        {
            m_slider.onValueChanged.RemoveListener(ChangeGammaFromSliderValue);
        }

        private void ChangeGammaFromSliderValue(float value)
        {
            m_runtimeGamma.gamma.value = m_sliderToActualGamaCurve.Evaluate(value) * Vector4.one;
        }
    }
}
