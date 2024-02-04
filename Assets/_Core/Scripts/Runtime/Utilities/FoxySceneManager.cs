using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UrbanFox
{
    public static class FoxySceneManager
    {
        public static void LoadActiveScene(string scene, Action onComplete = null, Action<float> progress = null)
        {
            CoroutineHelper.StartCoroutine(LoadScene_Coroutine(scene, SetLoadedSceneAsActive, progress));
            void SetLoadedSceneAsActive()
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
                onComplete?.Invoke();
            }
        }

        public static void LoadScenes(IEnumerable<string> scenes, Action onComplete = null, Action<float> progress = null)
        {
            CoroutineHelper.StartCoroutine(LoadScenes_Coroutine(scenes, onComplete, progress));
        }

        public static void LoadScene(string scene, Action onComplete = null, Action<float> progress = null)
        {
            CoroutineHelper.StartCoroutine(LoadScene_Coroutine(scene, onComplete, progress));
        }

        public static void UnloadScenes(IEnumerable<string> scenes, Action onComplete = null, Action<float> progress = null)
        {
            CoroutineHelper.StartCoroutine(UnloadScenes_Coroutine(scenes, onComplete, progress));
        }

        public static void UnloadScene(string scene, Action onComplete = null, Action<float> progress = null)
        {
            CoroutineHelper.StartCoroutine(UnloadScene_Coroutine(scene, onComplete, progress));
        }

        public static IEnumerator LoadScenes_Coroutine(IEnumerable<string> scenes, Action onComplete = null, Action<float> progress = null)
        {
            if (!scenes.IsNullOrEmpty())
            {
                yield return null;
                var operations = new List<AsyncOperation>();
                foreach (var scene in scenes)
                {
                    if (!IsSceneLoaded(scene))
                    {
                        operations.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
                    }
                }
                while (!AreAsyncOperationsCompleted(operations, out var currentProgress))
                {
                    progress?.Invoke(currentProgress);
                    yield return null;
                }
                yield return null;
            }
            progress?.Invoke(1);
            onComplete?.Invoke();
        }

        public static IEnumerator LoadScene_Coroutine(string scene, Action onComplete = null, Action<float> progress = null)
        {
            if (!scene.IsNullOrEmpty())
            {
                yield return null;
                if (!IsSceneLoaded(scene))
                {
                    var operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                    while (!operation.isDone)
                    {
                        progress?.Invoke(operation.progress / 0.9f);
                        yield return null;
                    }
                    yield return null;
                }
            }
            progress?.Invoke(1);
            onComplete?.Invoke();
        }

        public static IEnumerator UnloadScenes_Coroutine(IEnumerable<string> scenes, Action onComplete = null, Action<float> progress = null)
        {
            if (!scenes.IsNullOrEmpty())
            {
                yield return null;
                var operations = new List<AsyncOperation>();
                foreach (var scene in scenes)
                {
                    if (IsSceneLoaded(scene))
                    {
                        operations.Add(SceneManager.UnloadSceneAsync(scene));
                    }
                }
                while (!AreAsyncOperationsCompleted(operations, out var currentProgress))
                {
                    progress?.Invoke(currentProgress);
                    yield return null;
                }
                yield return null;
            }
            progress?.Invoke(1);
            onComplete?.Invoke();
        }

        public static IEnumerator UnloadScene_Coroutine(string scene, Action onComplete = null, Action<float> progress = null)
        {
            if (!scene.IsNullOrEmpty())
            {
                yield return null;
                if (IsSceneLoaded(scene))
                {
                    var operation = SceneManager.UnloadSceneAsync(scene);
                    while (!operation.isDone)
                    {
                        progress?.Invoke(operation.progress / 0.9f);
                        yield return null;
                    }
                    yield return null;
                }
            }
            progress?.Invoke(1);
            onComplete?.Invoke();
        }

        public static bool IsSceneLoaded(string scene)
        {
            var target = SceneManager.GetSceneByName(scene);
            if (target != null && target.isLoaded)
            {
                return true;
            }
            return false;
        }

        private static bool AreAsyncOperationsCompleted(List<AsyncOperation> operations, out float currentProgress)
        {
            if (operations.IsNullOrEmpty())
            {
                currentProgress = 1;
                return true;
            }
            bool isDone = true;
            currentProgress = 0;
            foreach (var operation in operations)
            {
                currentProgress += operation.progress;
                if (!operation.isDone)
                {
                    isDone = false;
                }
            }
            currentProgress /= operations.Count;
            return isDone;
        }
    }
}
