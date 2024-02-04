using System.Collections;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using DG.Tweening;
using System;

namespace UrbanFox.MiniGame
{
    public class AudioManager : RuntimeManager<AudioManager>
    {
        private const string k_masterBusName = "bus:/Master";
        private const string k_gameBusName = "bus:/Master/Game";
        private const string k_reverbBusName = "bus:/Reverb";

        [SerializeField]
        private float m_slightDelayBeforeFadeIn;

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
        private Bus m_reverbBus;

        private float m_diegeticSoundsVolume = 1;

        private float m_cachedMasterBusVolume;
        private float m_cachedDiegeticSoundsVolume;
        private bool m_hasVolumeChanged;

        public float MasterBusVolume
        {
            get
            {
                m_masterBus.getVolume(out var volume);
                return volume;
            }
        }

        public float GameBusVolume
        {
            get
            {
                m_gameBus.getVolume(out var volume);
                return volume;
            }
        }

        public void FadeInGameBus(float fadeTime, Action onCompleted = null)
        {
            StartCoroutine(DoFadeInGameBus());
            IEnumerator DoFadeInGameBus()
            {
                yield return new WaitForSeconds(m_slightDelayBeforeFadeIn);
                DOTween.To(() => m_diegeticSoundsVolume, x => m_diegeticSoundsVolume = x, 1, fadeTime).SetEase(m_easeIn).OnComplete(() =>
                {
                    onCompleted?.Invoke();
                });
            }
        }

        public void FadeOutGameBus(float fadeTime, Action onCompleted = null)
        {
            DOTween.To(() => m_diegeticSoundsVolume, x => m_diegeticSoundsVolume = x, 0, fadeTime).SetEase(m_easeOut).OnComplete(() =>
            {
                onCompleted?.Invoke();
            });
        }

        private void Start()
        {
            m_masterBus = RuntimeManager.GetBus(k_masterBusName);
            m_gameBus = RuntimeManager.GetBus(k_gameBusName);
            m_reverbBus = RuntimeManager.GetBus(k_reverbBusName);
            UIManager.OnPauseMenuOpening += OnPauseMenuOpening;
            UIManager.OnPauseMenuClosing += OnPauseMenuClosing;
        }

        private void OnDestroy()
        {
            UIManager.OnPauseMenuOpening -= OnPauseMenuOpening;
            UIManager.OnPauseMenuClosing -= OnPauseMenuClosing;
            FMODUtilities.PrintMissingEventGUIDsOrPaths();
        }

        private void LateUpdate()
        {
            var clampedSettingsVolume = Mathf.Clamp01(SettingsManager.Instance.Volume);
            var clampedDiegeticSoundsVolume = Mathf.Clamp01(m_diegeticSoundsVolume);
            if (m_cachedMasterBusVolume != clampedSettingsVolume)
            {
                m_cachedMasterBusVolume = clampedSettingsVolume;
                m_masterBus.setVolume(clampedSettingsVolume);
                m_hasVolumeChanged = true;
            }
            if (m_cachedDiegeticSoundsVolume != clampedDiegeticSoundsVolume)
            {
                m_cachedDiegeticSoundsVolume = clampedDiegeticSoundsVolume;
                m_gameBus.setVolume(clampedDiegeticSoundsVolume);
                m_hasVolumeChanged = true;
            }
            if (m_hasVolumeChanged)
            {
                m_reverbBus.setVolume(clampedSettingsVolume * clampedDiegeticSoundsVolume);
                m_hasVolumeChanged = false;
            }
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
            FadeOutGameBus(m_defaultVolumeFadeOutSeconds);
        }

        private void FadeInGameAudio()
        {
            FadeInGameBus(m_defaultVolumeFadeInSeconds);
        }
    }
}
