using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UrbanFox.MiniGame
{
    public class GameManager : RuntimeManager<GameManager>
    {
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
        private readonly List<string> m_dirtyScenes = new List<string>();

        [SerializeField]
        private float m_fullscreenBlackFadeTime;

        [SerializeField]
        private float m_fullscreenBlackIdleTime;

        [Header("Scene Options")]
        [SerializeField]
        private float m_defaultWaitTimeBeforeFadeOut;

        [SerializeField]
        private float m_defaultFadeTime;

        [SerializeField]
        private float m_deltaTimeBetweenSceneOperations;

        [Header("Scene Initialization Options")]
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

        public void AddDirtyScenes(params string[] scenes)
        {
            if (!scenes.IsNullOrEmpty())
            {
                foreach (var scene in scenes)
                {
                    if (!m_dirtyScenes.Contains(scene))
                    {
                        m_dirtyScenes.Add(scene);
                    }
                }
            }
        }

        public void RemoveDirtyScenes(params string[] scenes)
        {
            if (!scenes.IsNullOrEmpty())
            {
                foreach (var scene in scenes)
                {
                    if (m_dirtyScenes.Contains(scene))
                    {
                        m_dirtyScenes.Remove(scene);
                    }
                }
            }
        }

        public void RestartCheckpoint_Fade_Default(SceneLoadOperationCallbacks? callbacks = null)
        {
            RestartCheckpoint_Fade(m_defaultWaitTimeBeforeFadeOut, m_defaultFadeTime, m_defaultFadeTime, callbacks);
        }


        public void RestartCheckpoint_Fade(float waitTimeBeforeFadeOutBegins, SceneLoadOperationCallbacks? callbacks = null)
        {
            RestartCheckpoint_Fade(waitTimeBeforeFadeOutBegins, m_defaultFadeTime, m_defaultFadeTime, callbacks);
        }

        public void RestartCheckpoint_Fade(float waitTimeBeforeFadeOutBegins, float fadeOutDuration, float fadeInDuration, SceneLoadOperationCallbacks? callbacks = null)
        {
            if (m_currentGameState == GameState.GameplayPausable || m_currentGameState == GameState.GameCompletedWaitForInput)
            {
                UnloadAndLoadScenesFullScreen(m_dirtyScenes, m_dirtyScenes, true, waitTimeBeforeFadeOutBegins, fadeOutDuration, fadeInDuration, callbacks);
            }
        }

        public void RestartCheckpoint_Instant_Default(SceneLoadOperationCallbacks? callbacks = null)
        {
            RestartCheckpoint_Instant(m_defaultFadeTime, callbacks);
        }

        public void RestartCheckpoint_Instant(float fadeInDuration, SceneLoadOperationCallbacks? callbacks = null)
        {
            if (m_currentGameState == GameState.GameplayPausable)
            {
                UnloadAndLoadScenesFullScreen(m_dirtyScenes, m_dirtyScenes, true, 0, 0.1f, fadeInDuration, callbacks);
            }
        }

        public void UnloadAndLoadScenesFullScreen(IList<string> scenesToUnload, IList<string> scenesToLoad, bool isFirstSceneActive = true, SceneLoadOperationCallbacks? callbacks = null)
        {
            UnloadAndLoadScenesFullScreen(scenesToUnload, scenesToLoad, isFirstSceneActive, m_defaultWaitTimeBeforeFadeOut, m_defaultFadeTime, m_defaultFadeTime, callbacks);
        }

        public void UnloadAndLoadScenesFullScreen(IList<string> scenesToUnload, IList<string> scenesToLoad, bool isFirstSceneActive, float waitTimeBeforeFadeOutBegins, float fadeOutDuration, float fadeInDuration, SceneLoadOperationCallbacks? callbacks = null)
        {
            StartCoroutine(DoUnloadAndLoadScenesFullScreen());
            IEnumerator DoUnloadAndLoadScenesFullScreen()
            {
                SwitchGameState(GameState.Loading);

                // Wait time before fade out
                OnEachGameOverSignaled?.Invoke();
                yield return new WaitForSeconds(waitTimeBeforeFadeOutBegins);

                // Fade out begins
                callbacks?.OnFadeOutStarts?.Invoke();
                OnEachFadeOutStarts?.Invoke();
                UIManager.Instance.FadeOutToBlack(fadeOutDuration);
                yield return new WaitForSeconds(fadeOutDuration);

                // Fade out ends
                callbacks?.OnFadeOutCompletedAndIdleStarts?.Invoke();
                OnEachFadeOutCompletedAndIdleStarts?.Invoke();
                yield return new WaitForSeconds(m_deltaTimeBetweenSceneOperations);

                // Reload begins
                callbacks?.OnLoadingOperationStarts?.Invoke();
                OnEachLoadingOperationStarts?.Invoke();

                // Unload scenes
                var operations = new List<AsyncOperation>();
                if (!scenesToUnload.IsNullOrEmpty())
                {
                    foreach (var scene in scenesToUnload)
                    {
                        var targetScene = SceneManager.GetSceneByName(scene);
                        if (targetScene != null && targetScene.isLoaded)
                        {
                            operations.Add(SceneManager.UnloadSceneAsync(scene));
                        }
                    }
                    while (!AreOperationsCompleted(operations))
                    {
                        yield return null;
                    }
                }

                // Load scenes
                operations.Clear();
                if (!scenesToLoad.IsNullOrEmpty())
                {
                    foreach (var scene in scenesToLoad)
                    {
                        var targetScene = SceneManager.GetSceneByName(scene);
                        if (targetScene == null || !targetScene.isLoaded)
                        {
                            operations.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
                        }
                    }
                    while (!AreOperationsCompleted(operations))
                    {
                        yield return null;
                    }

                    // Optional: Set first scene as active
                    if (isFirstSceneActive && !scenesToLoad[0].IsNullOrEmpty())
                    {
                        yield return null;
                        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scenesToLoad[0]));
                    }
                }

                // Reload ends
                callbacks?.OnLoadingOperationCompletedAndIdleStarts?.Invoke();
                OnEachLoadingOperationCompletedAndIdleStarts?.Invoke();
                yield return new WaitForSeconds(m_deltaTimeBetweenSceneOperations);

                // Fade in begins
                callbacks?.OnFadeInStarts?.Invoke();
                OnEachFadeInStarts?.Invoke();
                UIManager.Instance.FadeInFromBlack(fadeInDuration);
                yield return new WaitForSeconds(fadeInDuration);

                // Fade in ends
                callbacks?.OnFadeInCompleted?.Invoke();
                OnEachFadeInCompleted?.Invoke();
            }
        }

        public void LoadScenesInBackground(string[] scenes, bool shouldTheFirstSceneInTheArrayBeActive)
        {
            StartCoroutine(DoLoadScenesInBackground());
            IEnumerator DoLoadScenesInBackground()
            {
                if (!scenes.IsNullOrEmpty())
                {
                    UnloadScenes(scenes);
                    var operations = new List<AsyncOperation>();
                    foreach (var scene in scenes)
                    {
                        var targetScene = SceneManager.GetSceneByName(scene);
                        if (targetScene == null || !targetScene.isLoaded)
                        {
                            operations.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
                        }
                    }
                    while (!AreOperationsCompleted(operations))
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
        }

        public void UnloadScenes(params string[] scenes)
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

        public void UnloadAllButPersistentScene()
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
            UnloadAllButPersistentScene();
            UnloadAndLoadScenesFullScreen(null, m_scenesToLoadOnStart, true, 0, 0, m_defaultFadeTime, new SceneLoadOperationCallbacks()
            {
                OnFadeInCompleted = () =>
                {
                    SwitchGameState(GameState.WaitForInputToStartGame);
                }
            });
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
