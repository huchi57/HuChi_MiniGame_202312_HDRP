using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UrbanFox.MiniGame
{
    public class UIManager : RuntimeManager<UIManager>
    {
        [Serializable]
        public enum LoadingIconType
        {
            Bar,
            SpinningWheel
        }

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

        [Header("Loading Icons")]
        [SerializeField]
        private float m_loadingIconFadeTime;

        [SerializeField]
        private LoadingIconType m_loadingIconType;

        [SerializeField, Required, ShowIf(nameof(m_loadingIconType), LoadingIconType.Bar)]
        private Slider m_loadingBar;

        [SerializeField, Required, ShowIf(nameof(m_loadingIconType), LoadingIconType.Bar)]
        private CanvasGroup m_loadingBarCanvasGroup;

        [SerializeField, Required, ShowIf(nameof(m_loadingIconType), LoadingIconType.SpinningWheel)]
        private CanvasGroup m_loadingWheelCanvasGroup;

        [SerializeField, ShowIf(nameof(m_loadingIconType), LoadingIconType.SpinningWheel)]
        private float m_loadingWheelSpinningSpeed;

        private bool m_enableSplashScreen;

        public void EnableSplashScreen(bool value)
        {
            m_enableSplashScreen = value;
        }

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

        public void FadeInLoadingIcon()
        {
            StartCoroutine(DoFadeInLoadingIcon());
            IEnumerator DoFadeInLoadingIcon()
            {
                switch (m_loadingIconType)
                {
                    case LoadingIconType.Bar:
                        m_loadingBar.value = 0;
                        m_loadingBarCanvasGroup.DOFade(1, m_loadingIconFadeTime);
                        yield return new WaitForSeconds(m_loadingIconFadeTime);
                        break;
                    case LoadingIconType.SpinningWheel:
                        m_loadingWheelCanvasGroup.DOFade(1, m_loadingIconFadeTime);
                        yield return new WaitForSeconds(m_loadingIconFadeTime);
                        break;
                    default:
                        break;
                }
            }
        }

        public void FadeOutLoadingIcon()
        {
            StartCoroutine(DoFadeOutLoadingIcon());
            IEnumerator DoFadeOutLoadingIcon()
            {
                switch (m_loadingIconType)
                {
                    case LoadingIconType.Bar:
                        m_loadingBarCanvasGroup.DOFade(0, m_loadingIconFadeTime);
                        yield return new WaitForSeconds(m_loadingIconFadeTime);
                        break;
                    case LoadingIconType.SpinningWheel:
                        m_loadingWheelCanvasGroup.DOFade(0, m_loadingIconFadeTime);
                        yield return new WaitForSeconds(m_loadingIconFadeTime);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetLoadingBarSliderValue(float value)
        {
            m_loadingBar.value = value;
        }

        public void QuitGame()
        {
            if (GameManager.IsInstanceExist)
            {
                GameManager.Instance.QuitGame();
            }
        }

        public void RestartGame()
        {
            if (GameManager.IsInstanceExist)
            {
                GameManager.Instance.RestartGame();
            }
            m_pauseMenuPageGroup.ClosePageGroup();
        }

        private void Start()
        {
            InputManager.OnAnyKeyButReservedKeysPressed += OnAnyKeyPressed;
            GameManager.OnFadeOutCompleted += ResetSplashScreenState;
            InputManager.Escape.OnKeyDown += OnEscapePressed;
            m_pauseMenuPageGroup.OnPageGroupStartsToOpen += OnPauseMenuStartsToOpen;
            m_pauseMenuPageGroup.OnPageGroupStartsToClose += OnPauseMenuStartsToClose;
            m_fullscreenBlack.alpha = 1;
            if (m_loadingBarCanvasGroup)
            {
                m_loadingBarCanvasGroup.alpha = 0;
            }
            if (m_loadingWheelCanvasGroup)
            {
                m_loadingWheelCanvasGroup.alpha = 0;
            }
        }

        private void OnDestroy()
        {
            InputManager.OnAnyKeyButReservedKeysPressed -= OnAnyKeyPressed;
            GameManager.OnFadeOutCompleted -= ResetSplashScreenState;
            InputManager.Escape.OnKeyDown -= OnEscapePressed;
            m_pauseMenuPageGroup.OnPageGroupStartsToOpen -= OnPauseMenuStartsToOpen;
            m_pauseMenuPageGroup.OnPageGroupStartsToClose -= OnPauseMenuStartsToClose;
        }

        private void LateUpdate()
        {
            if (m_loadingIconType == LoadingIconType.SpinningWheel && m_loadingWheelCanvasGroup)
            {
                m_loadingWheelCanvasGroup.transform.Rotate(m_loadingWheelSpinningSpeed * Time.deltaTime * Vector3.back);
            }
        }

        private void ResetSplashScreenState()
        {
            if (m_enableSplashScreen)
            {
                m_titleSplashScreen.gameObject.SetActive(true);
                m_titleSplashScreen.DOFade(1, 1).SetUpdate(true);
            }
        }

        private void OnAnyKeyPressed(UnityEngine.InputSystem.InputControl obj)
        {
            if (GameInstance.CurrentGameState == GameState.WaitForInputToStartGame || GameInstance.CurrentGameState == GameState.GameplayPausable)
            {
                m_titleSplashScreen.DOFade(0, m_titleSplashScreenFadeTime).OnComplete(() =>
                {
                    m_titleSplashScreen.gameObject.SetActive(false);
                    m_enableSplashScreen = false;
                });
            }
        }

        private void OnEscapePressed()
        {
            switch (GameInstance.CurrentGameState)
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
            GameInstance.SwitchGameState(GameState.Paused);
        }

        private void OnPauseMenuStartsToClose()
        {
            OnPauseMenuClosing?.Invoke();
            GameInstance.SwitchToPreviousGameState();
        }
    }
}
