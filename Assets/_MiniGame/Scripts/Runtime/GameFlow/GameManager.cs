using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

namespace UrbanFox.MiniGame
{
    public class GameManager : RuntimeManager<GameManager>
    {
        [Serializable]
        public enum LoadingIconType
        {
            Bar,
            SpinningWheel
        }

        public const string PlayerTag = "Player";

        public static event Action<GameState> OnGameStateChanged;

        /// <summary>
        /// The instant the gameplay begins.
        /// </summary>
        public static event Action OnGameStartSignaled;

        /// <summary>
        /// The instant that a game over procedure starts, such as stopping user input.
        /// </summary>
        public static event Action OnGameOverSignaled;

        /// <summary>
        /// The instant that the scenes is reloading when the screen gets black.
        /// </summary>
        public static event Action OnGameFullyFadeOutAndReloadStarted;

        /// <summary>
        /// The instant that the scenes are reloaded while the screen is still black, ready to fade in to game again.
        /// </summary>
        public static event Action OnGameReloadCompleted;

        public static PlayerController PlayerController
        {
            get;
            private set;
        }

        public static Transform Player
        {
            get
            {
                if (m_player)
                {
                    return m_player;
                }
                var playerObject = GameObject.FindWithTag(PlayerTag);
                if (playerObject)
                {
                    m_player = playerObject.transform;
                }
                return m_player;
            }
        }

        private static Transform m_player;

        [SerializeField]
        private float m_fullscreenBlackFadeTime;

        [SerializeField]
        private float m_fullscreenBlackIdleTime;

        [SerializeField]
        private float m_loadingIconFadeTime;

        [SerializeField, Required]
        private CanvasGroup m_fullscreenBlack;

        [SerializeField]
        private LoadingIconType m_loadingIconType;

        [SerializeField, ShowIf(nameof(m_loadingIconType), LoadingIconType.Bar)]
        private Slider m_loadingBar;

        [SerializeField, ShowIf(nameof(m_loadingIconType), LoadingIconType.Bar)]
        private CanvasGroup m_loadingBarCanvasGroup;

        [SerializeField, ShowIf(nameof(m_loadingIconType), LoadingIconType.SpinningWheel)]
        private CanvasGroup m_loadingWheelCanvasGroup;

        [SerializeField, ShowIf(nameof(m_loadingIconType), LoadingIconType.SpinningWheel)]
        private float m_loadingWheelSpinningSpeed;

        [Header("Game Start Options")]

        [SerializeField, Scene]
        private string[] m_scenesToLoadOnStart;

        [SerializeField]
        private GameState m_currentGameState;

        private GameState m_previousGameState;

        public GameState CurrentGameState => m_currentGameState;

        public static void RegisterPlayer(PlayerController player)
        {
            PlayerController = player;
            m_player = player.transform;
        }

        public static void RegisterPlayer(Transform player)
        {
            m_player = player;
        }

        public void SwitchGameState(GameState gameState)
        {
            m_previousGameState = m_currentGameState;
            m_currentGameState = gameState;
            OnGameStateChanged?.Invoke(m_currentGameState);
            if (m_currentGameState == GameState.GameplayPausable)
            {
                OnGameStartSignaled?.Invoke();
            }
        }

        public void SwitchToPreviousGameState()
        {
            SwitchGameState(m_previousGameState);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        public void LoadScenesWithFullscreenCover(string[] scenes, bool shouldTheFirstSceneInTheArrayBeActive, bool displayLoadingIcon, Action onFadeOutBegins = null, Action onFadeOutEnds = null, Action onLoadCompleted = null, Action onFadeInBegins = null, Action onFadeInEnds = null)
        {
            StartCoroutine(DoLoadScenesWithFullscreenCover());
            IEnumerator DoLoadScenesWithFullscreenCover()
            {
                SwitchGameState(GameState.Loading);
                onFadeOutBegins?.Invoke();
                m_fullscreenBlack.DOFade(1, m_fullscreenBlackFadeTime);
                yield return new WaitForSeconds(m_fullscreenBlackFadeTime);
                onFadeOutEnds?.Invoke();
                yield return new WaitForSeconds(m_fullscreenBlackIdleTime / 2);
                if (displayLoadingIcon)
                {
                    switch (m_loadingIconType)
                    {
                        case LoadingIconType.Bar:
                            m_loadingBar.value = 0;
                            m_loadingBarCanvasGroup.DOFade(1, m_loadingIconFadeTime);
                            break;
                        case LoadingIconType.SpinningWheel:
                            m_loadingWheelCanvasGroup.DOFade(1, m_loadingIconFadeTime);
                            break;
                        default:
                            break;
                    }
                }
                UnloadScenes(scenes);
                yield return new WaitForSeconds(m_fullscreenBlackIdleTime / 2);
                var operations = new List<AsyncOperation>();
                foreach (var scene in scenes)
                {
                    operations.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
                }
                while (AreOperationsCompleted(operations))
                {
                    m_loadingBar.value = GetOperationsProgress(operations);
                    yield return null;
                }
                m_loadingBar.value = 1;
                if (shouldTheFirstSceneInTheArrayBeActive)
                {
                    while (!IsSceneLoaded(scenes[0]))
                    {
                        yield return null;
                    }
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(scenes[0]));
                }
                onLoadCompleted?.Invoke();
                yield return new WaitForSeconds(m_fullscreenBlackIdleTime / 2);
                switch (m_loadingIconType)
                {
                    case LoadingIconType.Bar:
                        m_loadingBarCanvasGroup.DOFade(0, m_loadingIconFadeTime);
                        break;
                    case LoadingIconType.SpinningWheel:
                        m_loadingWheelCanvasGroup.DOFade(0, m_loadingIconFadeTime);
                        break;
                    default:
                        break;
                }
                yield return new WaitForSeconds(m_loadingIconFadeTime + m_fullscreenBlackIdleTime / 2);
                onFadeInBegins?.Invoke();
                m_fullscreenBlack.DOFade(0, m_fullscreenBlackFadeTime).OnComplete(() =>
                {
                    onFadeInEnds?.Invoke();
                });
            }
        }

        public void LoadScenesInBackground(string[] scenes, bool shouldTheFirstSceneInTheArrayBeActive)
        {
            StartCoroutine(DoLoadScenesInBackground());
            IEnumerator DoLoadScenesInBackground()
            {
                UnloadScenes(scenes);
                var operations = new List<AsyncOperation>();
                foreach (var scene in scenes)
                {
                    operations.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
                }
                while (AreOperationsCompleted(operations))
                {
                    yield return null;
                }
                yield return null;
                if (shouldTheFirstSceneInTheArrayBeActive)
                {
                    while (!IsSceneLoaded(scenes[0]))
                    {
                        yield return null;
                    }
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(scenes[0]));
                }
            }
        }

        public void GameOverAndRestartCheckpoint(float waitSecondsBeforeFadeOut = 2)
        {
            if (m_currentGameState == GameState.GameplayPausable)
            {
                StartCoroutine(DoGameOverAndRestartCheckpoint(waitSecondsBeforeFadeOut));
            }
            IEnumerator DoGameOverAndRestartCheckpoint(float waitSecondsBeforeFadeOut)
            {
                SwitchGameState(GameState.GameOverWaitForReload);
                OnGameOverSignaled?.Invoke();
                yield return new WaitForSeconds(waitSecondsBeforeFadeOut);
                LoadScenesWithFullscreenCover(scenes: m_scenesToLoadOnStart,
                    shouldTheFirstSceneInTheArrayBeActive: true,
                    displayLoadingIcon: false,
                    onFadeOutEnds: () =>
                    {
                        UnloadAllButPersistentScene();
                        OnGameFullyFadeOutAndReloadStarted?.Invoke();
                    },
                    onLoadCompleted: () =>
                    {
                        OnGameReloadCompleted?.Invoke();
                    },
                    onFadeInEnds: () =>
                    {
                        SwitchGameState(GameState.WaitForInputToStartGame);
                    }
                    );
            }
        }

        public void UnloadScenes(string[] scenes)
        {
            if (scenes.IsNullOrEmpty())
            {
                return;
            }
            foreach (var scene in scenes)
            {
                if (IsSceneLoaded(scene))
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
        }

        private void UnloadAllButPersistentScene()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && scene.buildIndex != 0)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
        }

        private void Start()
        {
            m_previousGameState = m_currentGameState;
            m_fullscreenBlack.alpha = 1;
            m_loadingBarCanvasGroup.alpha = 0;
            m_loadingWheelCanvasGroup.alpha = 0;
            UnloadAllButPersistentScene();
            LoadScenesWithFullscreenCover(scenes: m_scenesToLoadOnStart,
                shouldTheFirstSceneInTheArrayBeActive: true,
                displayLoadingIcon: true,
                onFadeInEnds: () =>
                {
                    SwitchGameState(GameState.WaitForInputToStartGame);
                });
        }

        private void LateUpdate()
        {
            if (m_loadingIconType == LoadingIconType.SpinningWheel && m_loadingWheelCanvasGroup)
            {
                m_loadingWheelCanvasGroup.transform.Rotate(m_loadingWheelSpinningSpeed * Time.deltaTime * Vector3.back);
            }
        }

        private bool IsSceneLoaded(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (scene == null || !scene.IsValid())
            {
                return false;
            }
            return scene.isLoaded;
        }

        private bool AreOperationsCompleted(List<AsyncOperation> operations)
        {
            if (operations.IsNullOrEmpty())
            {
                return true;
            }
            foreach (var operation in operations)
            {
                if (!operation.isDone)
                {
                    return false;
                }
            }
            return true;
        }

        private float GetOperationsProgress(List<AsyncOperation> operations)
        {
            if (operations.IsNullOrEmpty())
            {
                return 0;
            }
            float value = 0;
            foreach (var operation in operations)
            {
                value += operation.progress / 0.9f;
            }
            return value / operations.Count;
        }
    }
}
