using System;
using UnityEngine;
using DG.Tweening;

namespace UrbanFox.MiniGame
{
    public class UIController : MonoBehaviour
    {
        public event Action OnPauseMenuOpening;
        public event Action OnPauseMenuClosing;

        [SerializeField, Required]
        private CanvasGroup m_titleSplashScreen;

        [SerializeField]
        private float m_titleSplashScreenFadeTime = 1;

        [SerializeField, Required]
        private UIPageGroup m_pauseMenuPageGroup;

        private void Awake()
        {
            GameManager.OnGameStartSignaled += OnGameStart;
            InputManager.Escape.OnKeyDown += OnEscapePressed;
            m_pauseMenuPageGroup.OnPageGroupStartsToOpen += OnPauseMenuStartsToOpen;
            m_pauseMenuPageGroup.OnPageGroupStartsToClose += OnPauseMenuStartsToClose;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStartSignaled -= OnGameStart;
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
