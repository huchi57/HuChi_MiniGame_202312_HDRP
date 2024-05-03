using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace UrbanFox.MiniGame
{
    public class EndingCutToTrailer : MonoBehaviour
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

        [SerializeField, Required]
        private CanvasGroup m_background;

        [SerializeField]
        private CreditSlide m_endTitle;

        [SerializeField, Required]
        private UITrailerPlayer m_trailer;

        [SerializeField]
        private CreditSlide[] m_creditsSlides;

        [SerializeField]
        private bool m_lastSlideWaitForInput;

        [SerializeField, ShowIf(nameof(m_lastSlideWaitForInput), true)]
        private CreditSlide m_lastSlide;

        private void OnEnable()
        {
            if (GameInstance.CurrentGameState != GameState.GameplayPausable)
            {
                return;
            }
            GameInstance.SwitchGameState(GameState.Loading);
            m_background.gameObject.SetActive(true);
            StartCoroutine(Sequence());
            IEnumerator Sequence()
            {
                m_endTitle.Slide.alpha = 0;
                m_endTitle.Slide.gameObject.SetActive(true);
                m_endTitle.Slide.DOFade(1, m_endTitle.FadeTime);
                yield return new WaitForSecondsRealtime(m_endTitle.FadeTime + m_endTitle.DisplayDuration);
                m_endTitle.Slide.DOFade(0, m_endTitle.FadeTime).OnComplete(() =>
                {
                    m_endTitle.Slide.gameObject.SetActive(false);
                });
                yield return new WaitForSecondsRealtime(m_endTitle.FadeTime + m_endTitle.IntervalUntillNextSlide);

                m_trailer.PlayTrailer_DisallowSkip(OnTrailerFinished);
            }
        }

        private void OnTrailerFinished()
        {
            StartCoroutine(Sequence());
            IEnumerator Sequence()
            {
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

                if (m_lastSlideWaitForInput)
                {
                    InputManager.OnAnyKeyPressed += OnAnyKeyPressed;
                    m_lastSlide.Slide.gameObject.SetActive(true);
                    m_lastSlide.Slide.alpha = 0;
                    m_lastSlide.Slide.DOFade(1, m_lastSlide.FadeTime);
                }
                else
                {
                    m_background.gameObject.SetActive(false);
                    GameManager.Instance.RestartGame();
                }
            }
        }

        private void OnAnyKeyPressed(UnityEngine.InputSystem.InputControl obj)
        {
            InputManager.OnAnyKeyPressed -= OnAnyKeyPressed;
            m_lastSlide.Slide.DOFade(0, m_lastSlide.FadeTime).OnComplete(() =>
            {
                m_lastSlide.Slide.gameObject.SetActive(false);
                m_background.gameObject.SetActive(false);
                GameManager.Instance.RestartGame();
            });
        }
    }
}
