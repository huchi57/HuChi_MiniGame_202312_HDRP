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

        public static event Action OnEachGameStarts;

        public static event Action OnEachGameOverSignaled;

        public static event Action OnEachFadeOutStarts;

        public static event Action OnEachFadeOutCompletedAndIdleStarts;

        public static event Action OnEachLoadingOperationStarts;

        public static event Action OnEachLoadingOperationCompletedAndIdleStarts;

        public static event Action OnEachFadeInStarts;

        public static event Action OnEachFadeInCompleted;

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

        [Header("Scene Options")]

        [SerializeField, Scene]
        private string[] m_scenesToLoadOnStart;

        [SerializeField, Scene]
        private string[] m_persistentScenes;

        [SerializeField, NonEditable]
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
                OnEachGameStarts?.Invoke();
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

        public void LoadScenesWithFullScreenCover(string[] scenesToLoad, bool shouldTheFirstSceneInTheArrayBeActive, bool displayLoadingIcon, SceneLoadOperationCallbacks? callbacks = null)
        {
            LoadScenesWithFullScreenCover(scenesToLoad, shouldTheFirstSceneInTheArrayBeActive, displayLoadingIcon, m_fullscreenBlackFadeTime, callbacks);
        }

        public void LoadScenesWithFullScreenCover(string[] scenesToLoad, bool shouldTheFirstSceneInTheArrayBeActive, bool displayLoadingIcon, float fadeTime, SceneLoadOperationCallbacks? callbacks = null)
        {
            StartCoroutine(DoLoadScenesWithFullScreenCover());
            IEnumerator DoLoadScenesWithFullScreenCover()
            {
                SwitchGameState(GameState.Loading);

                // Fade out starts
                OnEachFadeOutStarts?.Invoke();
                callbacks?.OnFadeOutStarts?.Invoke();
                m_fullscreenBlack.DOFade(1, fadeTime);
                yield return new WaitForSeconds(fadeTime);

                // Fade out completed - start idle time
                OnEachFadeOutCompletedAndIdleStarts?.Invoke();
                callbacks?.OnFadeOutCompletedAndIdleStarts?.Invoke();
                yield return new WaitForSeconds(m_fullscreenBlackIdleTime / 2);
                if (displayLoadingIcon)
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

                // Idle completed - start loading scenes
                OnEachLoadingOperationStarts?.Invoke();
                callbacks?.OnLoadingOperationStarts?.Invoke();
                UnloadAllButPersistentScene();
                UnloadScenes(scenesToLoad);
                yield return new WaitForSeconds(m_fullscreenBlackIdleTime / 2);
                var operations = new List<AsyncOperation>();
                foreach (var scene in scenesToLoad)
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
                    while (!IsSceneLoaded(scenesToLoad[0]))
                    {
                        yield return null;
                    }
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(scenesToLoad[0]));
                }

                // Loading completed - start idle time
                OnEachLoadingOperationCompletedAndIdleStarts?.Invoke();
                callbacks?.OnLoadingOperationCompletedAndIdleStarts?.Invoke();
                yield return new WaitForSeconds(m_fullscreenBlackIdleTime / 2);
                if (displayLoadingIcon)
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
                yield return new WaitForSeconds(m_fullscreenBlackIdleTime / 2);

                // Fade in begins
                OnEachFadeInStarts?.Invoke();
                callbacks?.OnFadeInStarts?.Invoke();
                m_fullscreenBlack.DOFade(0, m_fullscreenBlackFadeTime).OnComplete(() =>
                {
                    OnEachFadeInCompleted?.Invoke();
                    callbacks?.OnFadeInCompleted?.Invoke();
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

        public void GameOverAndRestartCheckpoint_FadeOut(float waitSecondsBeforeFadeOut = 2, SceneLoadOperationCallbacks? callbacks = null)
        {
            if (m_currentGameState == GameState.GameplayPausable || m_currentGameState == GameState.GameCompletedWaitForInput)
            {
                StartCoroutine(DoGameOverAndRestartCheckpoint_FadeOut(waitSecondsBeforeFadeOut));
            }
            IEnumerator DoGameOverAndRestartCheckpoint_FadeOut(float waitSecondsBeforeFadeOut)
            {
                SwitchGameState(GameState.GameOverWaitForReload);
                OnEachGameOverSignaled?.Invoke();
                yield return new WaitForSeconds(waitSecondsBeforeFadeOut);
                LoadScenesWithFullScreenCover(scenesToLoad: m_scenesToLoadOnStart,
                    shouldTheFirstSceneInTheArrayBeActive: true,
                    displayLoadingIcon: false,
                    callbacks: callbacks);
            }
        }

        public void GameOverAndRestartCheckpoint_Instant(SceneLoadOperationCallbacks? callbacks = null)
        {
            if (m_currentGameState == GameState.GameplayPausable)
            {
                SwitchGameState(GameState.GameOverWaitForReload);
                OnEachGameOverSignaled?.Invoke();
                LoadScenesWithFullScreenCover(scenesToLoad: m_scenesToLoadOnStart,
                    shouldTheFirstSceneInTheArrayBeActive: true,
                    displayLoadingIcon: false,
                    fadeTime: 0.1f,
                    callbacks);
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
                    if (!m_persistentScenes.Contains(scene.name))
                    {
                        SceneManager.UnloadSceneAsync(scene);
                    }
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
            LoadScenesWithFullScreenCover(scenesToLoad: m_scenesToLoadOnStart,
                shouldTheFirstSceneInTheArrayBeActive: true,
                displayLoadingIcon: true,
                new SceneLoadOperationCallbacks()
                {
                    OnFadeInCompleted = () =>
                    {
                        SwitchGameState(GameState.WaitForInputToStartGame);
                    }
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
