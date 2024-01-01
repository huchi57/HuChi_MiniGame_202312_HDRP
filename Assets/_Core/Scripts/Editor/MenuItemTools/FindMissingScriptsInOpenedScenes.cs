using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace UrbanFox.Editor
{
    public static class FindMissingScriptsInOpenedScenes
    {
        private static readonly List<GameObject> m_gameObjectsWithMissingScripts = new List<GameObject>();

        [MenuItem("OwO/Tools/Check Missing Scripts in Opened Scenes")]
        public static void FindAndSelectMissingScriptsInOpenedScenes()
        {
            m_gameObjectsWithMissingScripts.Clear();

            // Prefab Mode exception:
            var prefabState = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabState != null)
            {
                FindMissingScriptsFromRootObject(prefabState.prefabContentsRoot);
                HighlightGameObjectsWithMissingScripts(isPrefabMode: true);
                return;
            }

            // Default: check opened scenes:
            var currentLoadedScenes = SceneManager.sceneCount;
            var loadedScenes = new Scene[currentLoadedScenes];

            for (int i = 0; i < currentLoadedScenes; i++)
            {
                loadedScenes[i] = SceneManager.GetSceneAt(i);
            }

            foreach (var scene in loadedScenes)
            {
                FindMissingScriptsInScene(scene);
            }

            HighlightGameObjectsWithMissingScripts(isPrefabMode: false);
        }

        private static void FindMissingScriptsInScene(Scene scene)
        {
            if (scene.isLoaded)
            {
                foreach (var rootObject in scene.GetRootGameObjects())
                {
                    FindMissingScriptsFromRootObject(rootObject);
                }
            }
        }

        private static void FindMissingScriptsFromRootObject(GameObject rootObject)
        {
            var children = rootObject.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
            {
                var components = child.GetComponents<Component>();
                foreach (var component in components)
                {
                    if (component == null && !m_gameObjectsWithMissingScripts.Contains(child.gameObject))
                    {
                        m_gameObjectsWithMissingScripts.Add(child.gameObject);
                    }
                }
            }
        }

        private static void HighlightGameObjectsWithMissingScripts(bool isPrefabMode)
        {
            if (m_gameObjectsWithMissingScripts.Count > 0)
            {
                PrintMissingScriptList();
                Selection.objects = m_gameObjectsWithMissingScripts.ToArray();
            }
            else
            {
                FoxyLogger.Log($"There are no missing scripts in {(isPrefabMode ? "this prefab." : "opened scenes.")}");
            }
        }

        private static void PrintMissingScriptList()
        {
            if (m_gameObjectsWithMissingScripts.Count <= 0)
            {
                return;
            }

            //var objectList = new StringBuilder();
            //foreach (var obj in m_gameObjectsWithMissingScripts)
            //{
            //    objectList.AppendLine($"- {obj.name}");
            //}

            FoxyLogger.LogWarning($"Selecting {m_gameObjectsWithMissingScripts.Count} object(s) with missing scripts:\n- {string.Join("\n- ", m_gameObjectsWithMissingScripts)}");
        }
    }
}
