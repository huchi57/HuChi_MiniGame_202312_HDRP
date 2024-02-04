using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UrbanFox.MiniGame
{
    public class NewGameManager : RuntimeManager<NewGameManager>
    {
        public const string PlayerTag = "Player";

        public static event Action OnRestartCheckpointTriggered;
        public static event Action OnFadeOutStarted;
        public static event Action OnFadeOutCompleted;

        public static event Action OnReloadedScenesReady;
        public static event Action OnFadeInStarted;
        public static event Action OnFadeInCompleted;

        private readonly List<string> m_dirtyScenes = new List<string>();

        [SerializeField, Scene]
        private string[] m_initialScenes;

        [SerializeField, Scene]
        private string[] m_persistentScenes;

        [Header("Default Checkpoint Reset Wait Time")]
        [SerializeField, Min(0)]
        private float m_defaultWaitTimeBeforeFadeOut;

        [SerializeField, Min(0)]
        private float m_defaultWaitTimeBeforeFadeIn;

        [Header("Default Fade Out Time")]
        [SerializeField, Min(0)]
        private float m_defaultAudioFadeOutTime;

        [SerializeField, Min(0)]
        private float m_defaultScreenFadeOutTime;

        [Header("Default Fade In Time")]
        [SerializeField, Min(0)]
        private float m_defaultAudioFadeInTime;

        [SerializeField, Min(0)]
        private float m_defaultScreenFadeInTime;

        private bool m_isRestarting;

        public void AddDirtyScenes(IEnumerable<string> scenes)
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

        public void RemoveDirtyScenes(IEnumerable<string> scenes)
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

        public void RemoveAllDirtyScenes()
        {
            m_dirtyScenes.Clear();
        }

        public void RestartCheckpoint_DefaultFade()
        {
            RestartCheckpoint(m_defaultWaitTimeBeforeFadeOut, m_defaultAudioFadeOutTime, m_defaultScreenFadeOutTime,
                m_defaultWaitTimeBeforeFadeIn, m_defaultAudioFadeInTime, m_defaultScreenFadeInTime);
        }

        public void RestartCheckpoint_DefaultFade(float waitTime)
        {
            RestartCheckpoint(waitTime, m_defaultAudioFadeOutTime, m_defaultScreenFadeOutTime,
                m_defaultWaitTimeBeforeFadeIn, m_defaultAudioFadeInTime, m_defaultScreenFadeInTime);
        }

        public void RestartCheckpoint_Instant()
        {
            RestartCheckpoint(0, 0.1f, 0.1f,
                m_defaultWaitTimeBeforeFadeIn, m_defaultAudioFadeInTime, m_defaultScreenFadeInTime);
        }

        public void RestartCheckpoint(float waitTime, float audioFadeOutTime, float screenFadeOutTime,
            float postReloadWaitTime, float audioFadeInTime, float screenFadeInTime)
        {
            if (!m_isRestarting)
            {
                StartCoroutine(Coroutine());
            }
            IEnumerator Coroutine()
            {
                m_isRestarting = true;
                GameStateManager.SwitchGameState(GameState.GameOverWaitForReload);
                OnRestartCheckpointTriggered?.Invoke();
                yield return new WaitForSeconds(waitTime);

                OnFadeOutStarted?.Invoke();
                AudioManager.Instance.FadeOutGameBus(audioFadeOutTime);
                UIManager.Instance.FadeOutToBlack(screenFadeOutTime);
                yield return new WaitForSeconds(Mathf.Max(screenFadeOutTime, screenFadeOutTime));
                OnFadeOutCompleted?.Invoke();

                GameStateManager.SwitchGameState(GameState.Loading);
                if (!m_dirtyScenes.IsNullOrEmpty())
                {
                    yield return FoxySceneManager.UnloadScenes_Coroutine(m_dirtyScenes);
                    yield return FoxySceneManager.LoadScenes_Coroutine(m_dirtyScenes);
                }

                OnReloadedScenesReady?.Invoke();
                yield return new WaitForSeconds(postReloadWaitTime);

                OnFadeInStarted?.Invoke();

                // HACK: Makes audio fade in a little earlier because it responses slower
                AudioManager.Instance.FadeInGameBus(audioFadeInTime);
                yield return new WaitForSeconds(postReloadWaitTime);

                UIManager.Instance.FadeInFromBlack(screenFadeInTime);
                yield return new WaitForSeconds(Mathf.Max(audioFadeInTime, screenFadeInTime));
                OnFadeInCompleted?.Invoke();
                GameStateManager.SwitchGameState(GameState.WaitForInputToStartGame);
                m_isRestarting = false;
            }
        }

        private void Start()
        {
            UnloadAllButPersistentScenes(LoadInitialScenes);
        }

        private void UnloadAllButPersistentScenes(Action onComplete = null)
        {
            var scenes = new List<string>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && scene.buildIndex != 0)
                {
                    if (!m_persistentScenes.Contains(scene.name))
                    {
                        scenes.Add(scene.name);
                    }
                }
            }
            FoxySceneManager.UnloadScenes(scenes, onComplete);
        }

        private void LoadInitialScenes()
        {
            UIManager.Instance.FadeOutToBlack(1f, () =>
            {
                FoxySceneManager.LoadScenes(m_initialScenes, () =>
                {
                    RestartCheckpoint(0, 0.1f, 0.1f, 0, 1, 1);
                });
            });
        }
    }
}
