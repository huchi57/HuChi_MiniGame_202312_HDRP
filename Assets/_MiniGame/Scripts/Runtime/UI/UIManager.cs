using System;
using UnityEngine;
using DG.Tweening;

namespace UrbanFox.MiniGame
{
    public class UIManager : RuntimeManager<UIManager>
    {
        public static event Action OnPauseMenuOpening;
        public static event Action OnPauseMenuClosing;

        [SerializeField, Required]
        private CanvasGroup m_titleSplashScreen;

        [SerializeField, Required]
        private CanvasGroup m_fullscreenBlack;

        [SerializeField]
        private float m_titleSplashScreenFadeTime = 1;

        [SerializeField, Required]
        private UIPageGroup m_pauseMenuPageGroup;

        public void FadeOutToBlack(float fadeDuration, Action onCompleted = null)
        {
            m_fullscreenBlack.gameObject.SetActive(true);
            if (fadeDuration < 0)
            {
                m_fullscreenBlack.alpha = 1;
                onCompleted?.Invoke();
            }
            else
            {
                m_fullscreenBlack.DOFade(1, fadeDuration).OnComplete(() =>
                {
                    onCompleted?.Invoke();
                });
            }
        }

        public void FadeInFromBlack(float fadeDuration, Action onCompleted = null)
        {
            if (fadeDuration < 0)
            {
                m_fullscreenBlack.alpha = 0;
                m_fullscreenBlack.gameObject.SetActive(false);
                onCompleted?.Invoke();
            }
            else
            {
                m_fullscreenBlack.DOFade(0, fadeDuration).OnComplete(() =>
                {
                    m_fullscreenBlack.gameObject.SetActive(false);
                    onCompleted?.Invoke();
                });
            }
        }

        private void Start()
        {
            GameManager.OnEachGameStarts += OnGameStart;
            InputManager.Escape.OnKeyDown += OnEscapePressed;
            m_pauseMenuPageGroup.OnPageGroupStartsToOpen += OnPauseMenuStartsToOpen;
            m_pauseMenuPageGroup.OnPageGroupStartsToClose += OnPauseMenuStartsToClose;
            m_fullscreenBlack.alpha = 1;
        }

        private void OnDestroy()
        {
            GameManager.OnEachGameStarts -= OnGameStart;
            InputManager.Escape.OnKeyDown -= OnEscapePressed;
            m_pauseMenuPageGroup.OnPageGroupStartsToOpen -= OnPauseMenuStartsToOpen;
            m_pauseMenuPageGroup.OnPageGroupStartsToClose -= OnPauseMenuStartsToClose;
        }

        private void OnGameStart()
        {
            m_titleSplashScreen.DOFade(0, m_titleSplashScreenFadeTime).OnComplete(() =>
            {
                m_titleSplashScreen.gameObject.SetActive(false);
            });
        }

        private void OnEscapePressed()
        {
            switch (GameManager.Instance.CurrentGameState)
            {
                case GameState.Loading:
                    break;
                case GameState.WaitForInputToStartGame:
                case GameState.GameplayPausable:
                    if (!m_pauseMenuPageGroup.gameObject.activeSelf)
                    {
                        m_pauseMenuPageGroup.OpenPageGroup();
                    }
                    break;
                case GameState.GameplayNonPausable:
                    break;
                case GameState.GameOverWaitForReload:
                    break;
                case GameState.Paused:
                    break;
                default:
                    break;
            }
        }

        private void OnPauseMenuStartsToOpen()
        {
            OnPauseMenuOpening?.Invoke();
            GameManager.Instance.SwitchGameState(GameState.Paused);
        }

        private void OnPauseMenuStartsToClose()
        {
            OnPauseMenuClosing?.Invoke();
            GameManager.Instance.SwitchToPreviousGameState();
        }
    }
}
