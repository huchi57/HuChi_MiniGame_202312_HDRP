using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using DG.Tweening;

namespace UrbanFox.MiniGame
{
    public class AudioManager : RuntimeManager<AudioManager>
    {
        private const string k_masterBusName = "bus:/Master";
        private const string k_gameBusName = "bus:/Master/Game";

        [SerializeField, Required]
        private UIController m_UIController;

        [SerializeField]
        private Ease m_easeIn;

        [SerializeField]
        private float m_defaultVolumeFadeInSeconds;

        [SerializeField]
        private Ease m_easeOut;

        [SerializeField]
        private float m_defaultVolumeFadeOutSeconds;

        private Bus m_masterBus;
        private Bus m_gameBus;

        private float m_internalGameVolume = 1;

        public void FadeInMasterBus(float fadeTime)
        {
            DOTween.To(() => m_internalGameVolume, x => m_internalGameVolume = x, 1, fadeTime).SetEase(m_easeIn);
        }

        public void FadeOutMasterBus(float fadeTime)
        {
            DOTween.To(() => m_internalGameVolume, x => m_internalGameVolume = x, 0, fadeTime).SetEase(m_easeOut);
        }

        private void Start()
        {
            m_masterBus = RuntimeManager.GetBus(k_masterBusName);
            m_gameBus = RuntimeManager.GetBus(k_gameBusName);
            m_UIController.OnPauseMenuOpening += OnPauseMenuOpening;
            m_UIController.OnPauseMenuClosing += OnPauseMenuClosing;
            GameManager.OnEachFadeOutCompletedAndIdleStarts += FadeOutGameAudio;
            GameManager.OnEachLoadingOperationCompletedAndIdleStarts += FadeInGameAudio;
        }

        private void OnDestroy()
        {
            m_UIController.OnPauseMenuOpening -= OnPauseMenuOpening;
            m_UIController.OnPauseMenuClosing -= OnPauseMenuClosing;
            GameManager.OnEachFadeOutCompletedAndIdleStarts -= FadeOutGameAudio;
            GameManager.OnEachLoadingOperationCompletedAndIdleStarts -= FadeInGameAudio;
            FMODUtilities.PrintMissingEventGUIDsOrPaths();
        }

        private void LateUpdate()
        {
            m_masterBus.setVolume(Mathf.Clamp01(SettingsManager.Instance.Volume));
            m_gameBus.setVolume(Mathf.Clamp01(m_internalGameVolume));
        }

        private void OnPauseMenuOpening()
        {
            m_gameBus.setPaused(true);
        }

        private void OnPauseMenuClosing()
        {
            m_gameBus.setPaused(false);
        }

        private void FadeOutGameAudio()
        {
            FadeOutMasterBus(m_defaultVolumeFadeOutSeconds);
        }

        private void FadeInGameAudio()
        {
            FadeInMasterBus(m_defaultVolumeFadeInSeconds);
        }
    }
}
