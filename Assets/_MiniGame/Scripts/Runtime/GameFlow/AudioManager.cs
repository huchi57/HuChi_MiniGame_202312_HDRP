using UnityEngine;
using FMODUnity;

namespace UrbanFox.MiniGame
{
    public class AudioManager : RuntimeManager<AudioManager>
    {
        private const string k_reverbBusName = "bus:/Reverb";

        private FMOD.Studio.Bus m_reverbBus;
        private float m_volume;

        public void SetReverbVolume(float value)
        {
            m_volume = value;
        }

        private void Start()
        {
            m_reverbBus = RuntimeManager.GetBus(k_reverbBusName);
            m_volume = SettingsManager.Instance.Volume;
        }

        private void Update()
        {
            m_volume = Mathf.Clamp(SettingsManager.Instance.Volume, 0, 1);
            m_reverbBus.setVolume(m_volume);
        }
    }
}
