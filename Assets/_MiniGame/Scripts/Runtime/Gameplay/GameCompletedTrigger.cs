using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace UrbanFox.MiniGame
{
    public class GameCompletedTrigger : MonoBehaviour
    {
        [Serializable]
        public struct CreditSlide
        {
            [Required]
            public CanvasGroup Slide;

            [Min(0)]
            public float FadeTime;

            [Min(0)]
            public float DisplayDuration;

            [Min(0)]
            public float IntervalUntillNextSlide;
        }

        [SerializeField]
        private float m_waitForSeconds;

        [SerializeField]
        private CreditSlide[] m_creditsSlides;

        private void Awake()
        {
            if (!m_creditsSlides.IsNullOrEmpty())
            {
                foreach (var slide in m_creditsSlides)
                {
                    slide.Slide.gameObject.SetActive(false);
                    slide.Slide.alpha = 0;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                player.LockToPitchAngle(0);
                StartCoroutine(PlayCredits());
            }
        }

        private IEnumerator PlayCredits()
        {
            yield return new WaitForSeconds(m_waitForSeconds);
            GameManager.Instance.SwitchGameState(GameState.Loading);
            if (!m_creditsSlides.IsNullOrEmpty())
            {
                foreach (var slide in m_creditsSlides)
                {
                    slide.Slide.gameObject.SetActive(true);
                    slide.Slide.alpha = 0;
                    slide.Slide.DOFade(1, slide.FadeTime);
                    yield return new WaitForSeconds(slide.FadeTime + slide.DisplayDuration);
                    slide.Slide.DOFade(0, slide.FadeTime).OnComplete(() =>
                    {
                        slide.Slide.gameObject.SetActive(false);
                    });
                    yield return new WaitForSeconds(slide.FadeTime + slide.IntervalUntillNextSlide);
                }
            }
            GameManager.Instance.SwitchGameState(GameState.GameCompletedWaitForInput);
            InputManager.OnAnyKeyPressed += OnAnykeyPressed;
        }

        private void OnAnykeyPressed(UnityEngine.InputSystem.InputControl obj)
        {
            GameManager.Instance.GameOverAndRestartCheckpoint_FadeOut(1);
            InputManager.OnAnyKeyPressed -= OnAnykeyPressed;
        }
    }
}
