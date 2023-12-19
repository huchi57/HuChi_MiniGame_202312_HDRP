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

        private Bus m_masterBus;
        private Bus m_gameBus;
        private Bus m_UIBus;

        private float m_masterVolume;

        public void SetMasterVolume(float value)
        {
            m_masterVolume = value;
        }

        private void Start()
        {
            m_masterBus = RuntimeManager.GetBus(k_masterBusName);
            m_gameBus = RuntimeManager.GetBus(k_gameBusName);
            m_UIBus = RuntimeManager.GetBus(k_UIBusName);
            m_masterVolume = SettingsManager.Instance.Volume;
            m_UIController.OnPauseMenuOpening += OnPauseMenuOpening;
            m_UIController.OnPauseMenuClosing += OnPauseMenuClosing;
        }

        private void OnDestroy()
        {
            m_UIController.OnPauseMenuOpening -= OnPauseMenuOpening;
            m_UIController.OnPauseMenuClosing -= OnPauseMenuClosing;
        }

        private void Update()
        {
            m_masterVolume = Mathf.Clamp(SettingsManager.Instance.Volume, 0, 1);
            m_masterBus.setVolume(m_masterVolume);
        }

        private void OnPauseMenuClosing()
        {
            m_gameBus.setPaused(false);
        }

        private void OnPauseMenuOpening()
        {
            m_gameBus.setPaused(true);
        }
    }
}
