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

        public static event Action<GameState> OnGameStateChanged;

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
        private string m_sceneToLoadOnStart;

        [SerializeField, NonEditable]
        private GameState m_currentGameState;

        public void SwitchGameState(GameState gameState)
        {
            m_currentGameState = gameState;
            OnGameStateChanged?.Invoke(m_currentGameState);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        public void LoadScenesWithLoadingBar(string[] scenes, bool shouldTheFirstSceneInTheArrayBeActive)
        {
            StartCoroutine(DoLoadScenesWithLoadingBar());
            IEnumerator DoLoadScenesWithLoadingBar()
            {
                m_fullscreenBlack.DOFade(1, m_fullscreenBlackFadeTime);
                yield return new WaitForSeconds(m_fullscreenBlackIdleTime + m_fullscreenBlackIdleTime / 2);
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
                m_fullscreenBlack.DOFade(0, m_fullscreenBlackFadeTime);
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

        private void Start()
        {
            m_fullscreenBlack.alpha = 1;
            m_loadingBarCanvasGroup.alpha = 0;
            m_loadingWheelCanvasGroup.alpha = 0;
            LoadScenesWithLoadingBar(new string[] { m_sceneToLoadOnStart }, true);
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

        private void UnloadScenes(string[] scenes)
        {
            foreach (var scene in scenes)
            {
                if (IsSceneLoaded(scene))
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
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
