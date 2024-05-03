using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UrbanFox.MiniGame
{
    public class GameManager : RuntimeManager<GameManager>
    {
        public static event Action OnRestartTriggered;
        public static event Action OnFadeOutCompleted;
        public static event Action OnLoadedScenesReady;
        public static event Action OnFadeInCompleted;

        [Header("Transition Time Settings")]
        [SerializeField]
        private float m_defaultWaitTimeBeforeFadeOut;

        [SerializeField]
        private float m_waitTimeBetweenLoads;

        [SerializeField]
        private float m_defaultFadeOutTime;

        [SerializeField]
        private float m_defaultFadeInTime;

        [Header("Scene Options")]
        [SerializeField, Scene]
        private string[] m_persistentScenes;

        [SerializeField, Scene]
        private string m_splashScreenScene;

        [SerializeField, Scene]
        private string m_gameReferenceScene;

        [SerializeField]
        private bool m_skipSplashScreenSceneOnStart;

        private readonly List<string> m_dirtyScenes = new List<string>();

        public void AddDirtyScenes(IEnumerable<string> scenes)
        {
            if (scenes.IsNullOrEmpty())
            {
                return;
            }
            foreach (var scene in scenes)
            {
                if (!m_dirtyScenes.Contains(scene))
                {
                    m_dirtyScenes.Add(scene);
                }
            }
        }

        public void RemoveDirtyScenes(IEnumerable<string> scenes)
        {
            if (scenes.IsNullOrEmpty())
            {
                return;
            }
            foreach (var scene in scenes)
            {
                if (m_dirtyScenes.Contains(scene))
                {
                    m_dirtyScenes.Remove(scene);
                }
            }
        }

        public void RestartGame()
        {
            GameInstance.PlayerController.UpdateRespawnPoint(Vector3.zero);
            CameraBrain.Main.OverrideCheckpointPosition(Vector3.zero, Quaternion.identity);
            CoroutineHelper.DelayCallFunction(0.05f, () =>
            {
                // HACK: Force delaying 0.05 seconds and make game state to pausable to make restart checkpoint work.
                GameInstance.SwitchGameState(GameState.GameplayPausable);
                UIManager.Instance.EnableSplashScreen(true);
                RestartCheckpoint_Instant();
            });
        }

        public void RestartCheckpoint_Instant(Action onComplete = null)
        {
            RestartCheckpoint_Fade(0, 0.1f, m_defaultFadeInTime, onComplete);
        }

        public void RestartCheckpoint_Fade(Action onComplete = null)
        {
            RestartCheckpoint_Fade(m_defaultWaitTimeBeforeFadeOut, m_defaultFadeOutTime, m_defaultFadeInTime, onComplete);
        }

        public void RestartCheckpoint_Fade(float waitTime, float fadeOutTime, float fadeInTime, Action onComplete = null)
        {
            if (GameInstance.CurrentGameState == GameState.GameplayPausable || GameInstance.CurrentGameState == GameState.GameCompletedWaitForInput)
            {
                StartCoroutine(Coroutine());
            }
            IEnumerator Coroutine()
            {
                GameInstance.SwitchGameState(GameState.Loading);
                OnRestartTriggered?.Invoke();
                yield return new WaitForSeconds(waitTime);
                yield return UnloadAndReloadScenes_Coroutine(m_dirtyScenes, m_dirtyScenes, fadeOutTime, fadeInTime, enableDefaultCallbacks: true, () =>
                {
                    GameInstance.SwitchGameState(GameState.WaitForInputToStartGame);
                    onComplete?.Invoke();
                });
            }
        }

        public void UnloadAndReloadScenes(IEnumerable<string> scenesToUnload, IEnumerable<string> scenesToLoad, float fadeOutTime, float fadeInTime, bool enableDefaultCallbacks, Action onComplete = null)
        {
            StartCoroutine(UnloadAndReloadScenes_Coroutine(scenesToUnload, scenesToLoad, fadeOutTime, fadeInTime, enableDefaultCallbacks, onComplete));
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        private void Start()
        {
#if UNITY_EDITOR
            UnloadAllButPersistentScene();
#endif
#if !UNITY_EDITOR
            m_skipSplashScreenSceneOnStart = false;
#endif
            var sceneToLoadOnStart = m_skipSplashScreenSceneOnStart ? m_gameReferenceScene : m_splashScreenScene;
            UnloadAndReloadScenes(null, new string[] { sceneToLoadOnStart }, 0, m_defaultFadeInTime, enableDefaultCallbacks: false, () =>
            {
                GameInstance.SwitchGameState(m_skipSplashScreenSceneOnStart ? GameState.WaitForInputToStartGame : GameState.Loading);
            });
        }

        private IEnumerator UnloadAndReloadScenes_Coroutine(IEnumerable<string> scenesToUnload, IEnumerable<string> scenesToLoad, float fadeOutTime, float fadeInTime, bool enableDefaultCallbacks, Action onComplete = null)
        {
            AudioManager.Instance.FadeOutGameBus(fadeOutTime);
            UIManager.Instance.FadeOutToBlack(fadeOutTime);
            yield return new WaitForSeconds(fadeOutTime);
            if (enableDefaultCallbacks)
            {
                OnFadeOutCompleted?.Invoke();
            }

            yield return new WaitForSeconds(m_waitTimeBetweenLoads);
            yield return FoxySceneManager.UnloadScenes_Coroutine(scenesToUnload);
            yield return FoxySceneManager.LoadScenes_Coroutine(scenesToLoad);
            if (enableDefaultCallbacks)
            {
                OnLoadedScenesReady?.Invoke();
            }
            AudioManager.Instance.FadeInGameBus(fadeInTime);
            yield return new WaitForSeconds(m_waitTimeBetweenLoads);

            UIManager.Instance.FadeInFromBlack(fadeInTime);
            yield return new WaitForSeconds(fadeInTime);
            if (enableDefaultCallbacks)
            {
                OnFadeInCompleted?.Invoke();
            }
            onComplete?.Invoke();
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
    }
}
