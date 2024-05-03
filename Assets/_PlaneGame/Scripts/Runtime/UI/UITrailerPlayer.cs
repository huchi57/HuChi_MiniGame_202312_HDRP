using System;
using UnityEngine;
using UnityEngine.Video;

namespace UrbanFox.MiniGame
{
    public class UITrailerPlayer : MonoBehaviour
    {
        [SerializeField, Required]
        private VideoPlayer m_trailer;

        [SerializeField, Required]
        private GameObject m_trailerContainer;

        private bool m_allowSkip;
        private Action m_onCompletePlaying;

        public void PlayTrailer_AllowSkip(Action onCompletePlaying = null)
        {
            m_allowSkip = true;
            PlayTrailer_Internal(onCompletePlaying);
        }

        public void PlayTrailer_DisallowSkip(Action onCompletePlaying = null)
        {
            m_allowSkip = false;
            PlayTrailer_Internal(onCompletePlaying);
        }

        private void PlayTrailer_Internal(Action onCompletePlaying = null)
        {
            if (m_allowSkip)
            {
                InputManager.OnAnyKeyPressed += OnAnyKeyPressed;
            }
            EventSystemManager.Instance.DisableEventSystem();
            m_trailer.loopPointReached += OnVideoEnded;
            m_onCompletePlaying = onCompletePlaying;
            m_trailerContainer.SetActive(true);
            m_trailer.Play();
        }

        private void OnVideoEnded(VideoPlayer source)
        {
            m_trailer.time = 0;
            EventSystemManager.Instance.EnableEventSystem();
            m_trailerContainer.SetActive(false);
            m_onCompletePlaying?.Invoke();
            m_trailer.loopPointReached -= OnVideoEnded;
            if (m_allowSkip)
            {
                InputManager.OnAnyKeyPressed -= OnAnyKeyPressed;
            }
        }

        private void OnAnyKeyPressed(UnityEngine.InputSystem.InputControl obj)
        {
            m_trailer.Stop();
            OnVideoEnded(m_trailer);
        }
    }
}
