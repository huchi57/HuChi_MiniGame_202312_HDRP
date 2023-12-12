using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace UrbanFox.Editor
{
    public class SceneBrowser : EditorWindow
    {
        private const int _contextButtonWidth = 110;
        private static Vector2 _scrollView = default;

        [SerializeField] private string _searchText = string.Empty;
        [SerializeField] private bool _showScenesNotInBuildSettings = false;
        [SerializeField] private bool _notInBuildSettingsFoldout = false;

        private string _cacheSearchText = string.Empty;

        [MenuItem("OwO/Window/Scene Browser...")]
        private static void Init()
        {
            var window = GetWindow<SceneBrowser>();
            window.titleContent = new GUIContent("Scene Browser");
            window.minSize = window.minSize.SetX(590);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Scene Browser", EditorStyles.boldLabel);
            _searchText = EditorGUILayoutExtensions.SearchText("Search Scene Name", _searchText);
            _showScenesNotInBuildSettings = EditorGUILayout.Toggle("Show Non-Built Scenes", _showScenesNotInBuildSettings);
            _cacheSearchText = _searchText.ToLower();

            if (GUILayout.Button("Build Settings...", GUILayout.Height(25)))
            {
                GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }

            EditorGUILayout.Space();

            _scrollView = GUILayout.BeginScrollView(_scrollView);
            GUILayout.Label("Scenes in Build Settings", EditorStyles.boldLabel);
            var scenes = EditorBuildSettings.scenes;
            if (scenes.IsNullOrEmpty())
            {
                EditorGUILayout.HelpBox("There are no scenes in the Build Settings.", MessageType.Info);
            }
            else
            {
                GUILayoutExtensions.HorizontalLine();
                for (int i = 0; i < scenes.Length; i++)
                {
                    DrawSceneSlot(scenes[i].path, i);
                }
            }

            if (_showScenesNotInBuildSettings)
            {
                EditorGUILayout.Space();
                _notInBuildSettingsFoldout = EditorGUILayout.Foldout(_notInBuildSettingsFoldout, "Scenes Not in Build Settings", true);
                if (_notInBuildSettingsFoldout)
                {
                    var allScenesGUID = AssetDatabase.FindAssets("t:scene");
                    var scenesNotInBuildSettings = new List<string>();
                    if (!allScenesGUID.IsNullOrEmpty())
                    {
                        foreach (var sceneGUID in allScenesGUID)
                        {
                            var sceneAssetPath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                            if (!IsSceneAssetInBuildSettings(sceneAssetPath))
                            {
                                scenesNotInBuildSettings.Add(sceneAssetPath);
                            }
                        }
                    }
                    if (scenesNotInBuildSettings.IsNullOrEmpty())
                    {
                        EditorGUILayout.HelpBox($"There are no scenes in the project, or all scenes in the project are in Build Settings.", MessageType.Info);
                    }
                    else
                    {
                        EditorGUI.indentLevel++;
                        foreach (var scene in scenesNotInBuildSettings)
                        {
                            DrawSceneSlot(scene);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
            }
            GUILayout.EndScrollView();
        }

        private void DrawSceneSlot(string sceneAssetPath, int? buildIndex = null)
        {
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath);
            if (sceneAsset == null)
            {
                return;
            }

            var sceneName = sceneAsset.name;
            if (sceneAsset == null || !sceneAsset.name.ToLower().Contains(_cacheSearchText))
            {
                return;
            }

            var label = buildIndex != null ? $"[{buildIndex}] {sceneName}" : sceneName;
            if (IsSceneActive(sceneName))
            {
                GUILayout.Label(label, EditorStyles.boldLabel);
            }
            else
            {
                GUILayout.Label(label);
            }

            GUILayout.BeginHorizontal();
            var cacheGUIState = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.ObjectField(sceneAsset, typeof(SceneAsset), true, GUILayout.Width(200));
            GUILayout.FlexibleSpace();

            var isSceneLoaded = IsSceneLoaded(sceneName);

            GUI.enabled = !isSceneLoaded;
            if (isSceneLoaded)
            {
                if (IsSceneActive(sceneName))
                {
                    GUILayout.Label("Is Active Scene", GUILayout.Width(_contextButtonWidth));
                }
                else
                {
                    GUI.enabled = true;
                    if (GUILayoutExtensions.ColoredButton("Set Active", Color.yellow, GUILayout.Width(_contextButtonWidth)))
                    {
                        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
                    }
                }
            }
            else
            {
                if (GUILayoutExtensions.ColoredButton("Open (Additive)", Color.green, GUILayout.Width(_contextButtonWidth)))
                {
                    EditorSceneManager.OpenScene(sceneAssetPath, OpenSceneMode.Additive);
                }
            }

            GUI.enabled = !isSceneLoaded;
            if (GUILayoutExtensions.ColoredButton("Open (Single)", Color.green, GUILayout.Width(_contextButtonWidth)))
            {
                EditorSceneManager.OpenScene(sceneAssetPath, OpenSceneMode.Single);
            }

            GUI.enabled = GetNumberOfEnabledScenes() > 1;
            if (isSceneLoaded)
            {
                if (GUILayoutExtensions.ColoredButton("Close", Color.red, GUILayout.Width(_contextButtonWidth)))
                {
                    EditorSceneManager.CloseScene(SceneManager.GetSceneByName(sceneAsset.name), false);
                }
            }
            else
            {
                if (GUILayoutExtensions.ColoredButton("Remove", Color.red, GUILayout.Width(_contextButtonWidth)))
                {
                    EditorSceneManager.CloseScene(SceneManager.GetSceneByName(sceneAsset.name), true);
                }
            }

            GUI.enabled = cacheGUIState;
            GUILayout.EndHorizontal();
            GUILayoutExtensions.HorizontalLine();
        }

        private bool IsSceneAssetInBuildSettings(string sceneAssetPath)
        {
            if (!EditorBuildSettings.scenes.IsNullOrEmpty())
            {
                foreach (var scene in EditorBuildSettings.scenes)
                {
                    if (scene.path == sceneAssetPath)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsSceneActive(string sceneName)
        {
            return SceneManager.GetActiveScene() == SceneManager.GetSceneByName(sceneName);
        }

        private bool IsSceneLoaded(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            return scene != null && scene.isLoaded;
        }

        private int GetNumberOfEnabledScenes()
        {
            int result = 0;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene != null && scene.isLoaded)
                {
                    result++;
                }
            }
            return result;
        }
    }
}
