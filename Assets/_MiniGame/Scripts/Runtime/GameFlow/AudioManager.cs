using UnityEngine;
using FMODUnity;

namespace UrbanFox.MiniGame
{
    public class AudioManager : RuntimeManager<AudioManager>
    {
        private const string k_masterBusName = "bus:/";

        private FMOD.Studio.Bus m_masterBus;
        private float m_masterVolume;

        public void SetMasterVolume(float value)
        {
            m_masterVolume = value;
        }

        private void Start()
        {
            m_masterBus = RuntimeManager.GetBus(k_masterBusName);
            m_masterVolume = SettingsManager.Instance.Volume;
        }

        private void Update()
        {
            m_masterVolume = Mathf.Clamp(SettingsManager.Instance.Volume, 0, 1);
            m_masterBus.setVolume(m_masterVolume);
        }
    }
}
