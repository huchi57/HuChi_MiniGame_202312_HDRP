using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UrbanFox
{
    public static class FoxySceneManager
    {
        public static bool IsSceneLoaded(string scene)
        {
            return IsSceneLoaded(SceneManager.GetSceneByName(scene));
        }

        public static bool IsSceneLoaded(Scene scene)
        {
            if (scene != null && scene.isLoaded)
            {
                return true;
            }
            return false;
        }

        public static void LoadScene(string scene, Action onComplete = null, Action<float> onProgress = null)
        {
            CoroutineHelper.StartCoroutine(LoadScene_Coroutine(scene, onComplete, onProgress));
        }

        public static void LoadScenes(IEnumerable<string> scenes, Action onComplete = null, Action<float> onProgress = null)
        {
            CoroutineHelper.StartCoroutine(LoadScenes_Coroutine(scenes, onComplete, onProgress));
        }

        public static void UnloadScene(string scene, Action onComplete = null, Action<float> onProgress = null)
        {
            CoroutineHelper.StartCoroutine(UnloadScene_Coroutine(scene, onComplete, onProgress));
        }

        public static void UnloadScene(Scene scene, Action onComplete = null, Action<float> onProgress = null)
        {
            CoroutineHelper.StartCoroutine(UnloadScene_Coroutine(scene, onComplete, onProgress));
        }

        public static void UnloadScenes(IEnumerable<string> scenes, Action onComplete = null, Action<float> onProgress = null)
        {
            CoroutineHelper.StartCoroutine(UnloadScenes_Coroutine(scenes, onComplete, onProgress));
        }

        public static void UnloadScenes(IEnumerable<Scene> scenes, Action onComplete = null, Action<float> onProgress = null)
        {
            CoroutineHelper.StartCoroutine(UnloadScenes_Coroutine(scenes, onComplete, onProgress));
        }

        public static IEnumerator LoadScene_Coroutine(string scene, Action onComplete = null, Action<float> onProgress = null)
        {
            if (!scene.IsNullOrEmpty() && !IsSceneLoaded(scene))
            {
                var operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                yield return null;
                while (!operation.isDone)
                {
                    onProgress?.Invoke(operation.progress / 0.9f);
                    yield return null;
                }
            }
            onProgress?.Invoke(1);
            onComplete?.Invoke();
        }

        public static IEnumerator LoadScenes_Coroutine(IEnumerable<string> scenes, Action onComplete = null, Action<float> onProgress = null)
        {
            if (!scenes.IsNullOrEmpty())
            {
                var operations = new List<AsyncOperation>();
                foreach (var scene in scenes)
                {
                    if (!IsSceneLoaded(scene))
                    {
                        operations.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
                    }
                }
                yield return null;
                while (!AreOperationsDone(operations, out var progress))
                {
                    onProgress?.Invoke(progress);
                    yield return null;
                }
            }
            onProgress?.Invoke(1);
            onComplete?.Invoke();
        }

        public static IEnumerator UnloadScene_Coroutine(string scene, Action onComplete = null, Action<float> onProgress = null)
        {
            yield return UnloadScene_Coroutine(SceneManager.GetSceneByName(scene), onComplete, onProgress);
        }

        public static IEnumerator UnloadScene_Coroutine(Scene scene, Action onComplete = null, Action<float> onProgress = null)
        {
            if (IsSceneLoaded(scene))
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                yield return null;
                while (!operation.isDone)
                {
                    onProgress?.Invoke(operation.progress / 0.9f);
                    yield return null;
                }
            }
            onProgress?.Invoke(1);
            onComplete?.Invoke();
        }

        public static IEnumerator UnloadScenes_Coroutine(IEnumerable<string> scenes, Action onComplete = null, Action<float> onProgress = null)
        {
            if (!scenes.IsNullOrEmpty())
            {
                var sceneInstances = new List<Scene>();
                foreach (var scene in scenes)
                {
                    sceneInstances.Add(SceneManager.GetSceneByName(scene));
                }
                yield return UnloadScenes_Coroutine(sceneInstances, onComplete, onProgress);
            }
        }

        public static IEnumerator UnloadScenes_Coroutine(IEnumerable<Scene> scenes, Action onComplete = null, Action<float> onProgress = null)
        {
            if (!scenes.IsNullOrEmpty())
            {
                var operations = new List<AsyncOperation>();
                foreach (var scene in scenes)
                {
                    if (IsSceneLoaded(scene))
                    {
                        operations.Add(SceneManager.UnloadSceneAsync(scene));
                    }
                }
                yield return null;
                while (!AreOperationsDone(operations, out var progress))
                {
                    onProgress?.Invoke(progress);
                    yield return null;
                }
            }
            onProgress?.Invoke(1);
            onComplete?.Invoke();
        }

        private static bool AreOperationsDone(List<AsyncOperation> operations, out float progress)
        {
            if (operations.IsNullOrEmpty())
            {
                progress = 1;
                return true;
            }
            bool isDone = true;
            progress = 0;
            foreach (var operation in operations)
            {
                progress += operation.progress / 0.9f;
                if (!operation.isDone)
                {
                    isDone = false;
                }
            }
            progress /= operations.Count;
            return isDone;
        }
    }
}
