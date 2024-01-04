using UnityEngine;
using FMOD.Studio;
using FMODUnity;

namespace UrbanFox.MiniGame
{
    public class AudioManager : RuntimeManager<AudioManager>
    {
        private const string k_masterBusName = "bus:/Master";
        private const string k_gameBusName = "bus:/Master/Game";
        private const string k_UIBusName = "bus:/Master/UI";

        [SerializeField, Required]
        private UIController m_UIController;

        [SerializeField]
        private float m_defaultVolumeFadeInSeconds;

        [SerializeField]
        private float m_defaultVolumeFadeOutSeconds;

        private Bus m_masterBus;
        private Bus m_gameBus;
        private Bus m_UIBus;

        private float m_currentVolume;

        private float m_userSettingsMasterVolume;
        private float m_globalMasterVolumeMultiplier = 1;
        private float m_volumeSmoothVelocity;
        private float m_currentSmoothTime = 1;

        public void SetUserSettingsMasterVolume(float value)
        {
            m_userSettingsMasterVolume = value;
        }

        public void FadeInMasterBus(float fadeTime)
        {
            m_currentSmoothTime = fadeTime;
            m_globalMasterVolumeMultiplier = 1;
        }

        public void FadeOutMasterBus(float fadeTime)
        {
            m_currentSmoothTime = fadeTime;
            m_globalMasterVolumeMultiplier = 0;
        }

        private void Start()
        {
            m_masterBus = RuntimeManager.GetBus(k_masterBusName);
            m_gameBus = RuntimeManager.GetBus(k_gameBusName);
            m_UIBus = RuntimeManager.GetBus(k_UIBusName);
            m_userSettingsMasterVolume = SettingsManager.Instance.Volume;
            m_UIController.OnPauseMenuOpening += OnPauseMenuOpening;
            m_UIController.OnPauseMenuClosing += OnPauseMenuClosing;
            GameManager.OnGameFullyFadeOutAndReloadStarted += OnGameFullyFadeOutAndReloadStarted;
            GameManager.OnGameReloadCompleted += OnGameReloadCompleted;
        }

        private void OnDestroy()
        {
            m_UIController.OnPauseMenuOpening -= OnPauseMenuOpening;
            m_UIController.OnPauseMenuClosing -= OnPauseMenuClosing;
            GameManager.OnGameFullyFadeOutAndReloadStarted -= OnGameFullyFadeOutAndReloadStarted;
            GameManager.OnGameReloadCompleted -= OnGameReloadCompleted;
            FMODUtilities.PrintMissingEventPaths();
        }

        private void Update()
        {
            m_userSettingsMasterVolume = Mathf.Clamp(SettingsManager.Instance.Volume, 0, 1);
            m_currentVolume = Mathf.SmoothDamp(m_currentVolume, m_userSettingsMasterVolume * m_globalMasterVolumeMultiplier, ref m_volumeSmoothVelocity, m_currentSmoothTime, float.MaxValue, Time.unscaledDeltaTime);
            m_masterBus.setVolume(Mathf.Clamp(m_currentVolume, 0, 1));
        }

        private void OnPauseMenuOpening()
        {
            m_gameBus.setPaused(true);
        }

        private void OnPauseMenuClosing()
        {
            m_gameBus.setPaused(false);
        }

        private void OnGameFullyFadeOutAndReloadStarted()
        {
            FadeOutMasterBus(m_defaultVolumeFadeOutSeconds);
        }

        private void OnGameReloadCompleted()
        {
            FadeInMasterBus(m_defaultVolumeFadeInSeconds);
        }
    }
}
