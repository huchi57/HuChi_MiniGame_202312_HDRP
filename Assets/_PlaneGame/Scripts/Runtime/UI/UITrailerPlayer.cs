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

        private Action m_onCompletePlaying;

        public void PlayTrailer(Action onCompletePlaying = null)
        {
            EventSystemManager.Instance.DisableEventSystem();
            InputManager.OnAnyKeyPressed += OnAnyKeyPressed;
            m_trailer.loopPointReached += OnVideoEnded;
            m_onCompletePlaying = onCompletePlaying;
            m_trailerContainer.SetActive(true);
            m_trailer.Play();
        }

        private void OnVideoEnded(VideoPlayer source)
        {
            EventSystemManager.Instance.EnableEventSystem();
            m_trailerContainer.SetActive(false);
            m_onCompletePlaying?.Invoke();
            InputManager.OnAnyKeyPressed -= OnAnyKeyPressed;
            m_trailer.loopPointReached -= OnVideoEnded;
        }

        private void OnAnyKeyPressed(UnityEngine.InputSystem.InputControl obj)
        {
            EventSystemManager.Instance.EnableEventSystem();
            m_trailerContainer.SetActive(false);
            m_onCompletePlaying?.Invoke();
            InputManager.OnAnyKeyPressed -= OnAnyKeyPressed;
        }
    }
}
