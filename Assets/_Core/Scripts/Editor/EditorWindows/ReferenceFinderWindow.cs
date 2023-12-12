using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace UrbanFox.Editor
{
    public class ReferenceFinderWindow : EditorWindow
    {
        private static Vector2 _scroll = default;

        [SerializeField] private int _mode = 0;

        // Find what
        [SerializeField] private UnityEngine.Object _findWhatObject = null;
        [SerializeField] private string _findWhatObjectGUID = string.Empty;

        // Replace with
        [SerializeField] private UnityEngine.Object _replaceWithObject = null;
        [SerializeField] private string _replaceWithObjectGUID = string.Empty;

        [SerializeField] private string _searchFolder = "Assets/";
        [SerializeField] private bool _searchFilterFoldout = false;

        // Search types
        [SerializeField] private bool _searchScenes = false;
        [SerializeField] private bool _searchPrefabs = false;
        [SerializeField] private bool _searchScriptableObjects = false;

        [SerializeField] private List<UnityEngine.Object> _searchedResults = new List<UnityEngine.Object>();

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private int _searchTimeInMilliseconds = 0;

        [MenuItem("OwO/Window/Reference Finder...")]
        private static void Init()
        {
            var window = GetWindow<ReferenceFinderWindow>();
            window.titleContent = new GUIContent("Reference Finder");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayoutExtensions.ColoredButton("Find", _mode == 0 ? Color.yellow : Color.white, GUILayout.Height(40)))
            {
                _mode = 0;
            }
            if (GUILayoutExtensions.ColoredButton("Find & Replace", _mode == 1 ? Color.yellow : Color.white, GUILayout.Height(40)))
            {
                _mode = 1;
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            GUILayout.Label($"{(_mode == 0 ? "Find" : _mode == 1 ? "Find & Replace" : "")}", EditorStyles.boldLabel);
            _findWhatObject = EditorGUILayout.ObjectField("Find What", _findWhatObject, typeof(UnityEngine.Object), allowSceneObjects: true);
            _findWhatObjectGUID = _findWhatObject ? AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(_findWhatObject)).ToString() : "<None>";

            GUI.enabled = false;
            EditorGUI.indentLevel++;
            EditorGUILayout.TextField("GUID (Read Only)", _findWhatObjectGUID);
            EditorGUI.indentLevel--;
            GUI.enabled = true;

            if (_mode == 1)
            {
                EditorGUILayout.Space();

                _replaceWithObject = EditorGUILayout.ObjectField("Replace With", _replaceWithObject, typeof(UnityEngine.Object), allowSceneObjects: true);
                _replaceWithObjectGUID = _replaceWithObject ? AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(_replaceWithObject)).ToString() : "<None>";

                GUI.enabled = false;
                EditorGUI.indentLevel++;
                EditorGUILayout.TextField("GUID (Read Only)", _replaceWithObjectGUID);
                EditorGUI.indentLevel--;
                GUI.enabled = true;
            }

            EditorGUILayout.Space();

            GUILayoutExtensions.HorizontalLine();
            GUILayout.Label("Options", EditorStyles.boldLabel);

            using (var _ = new GUILayout.HorizontalScope())
            {
                _searchFolder = EditorGUILayout.TextField("Search Folder", _searchFolder);
                if (GUILayout.Button("Browse", GUILayout.Width(60)))
                {
                    _searchFolder = EditorUtility.OpenFolderPanel("Browse Search Folder", _searchFolder, string.Empty);
                    if (_searchFolder.ToLower().Contains("assets"))
                    {
                        _searchFolder = _searchFolder.Substring(_searchFolder.ToLower().IndexOf("assets"));
                    }
                }
            }

            _searchFilterFoldout = EditorGUILayout.Foldout(_searchFilterFoldout, "Only Search in Types:", true);
            if (_searchFilterFoldout)
            {
                EditorGUI.indentLevel++;
                _searchScenes = EditorGUILayout.Toggle(new GUIContent("Scenes", ".unity"), _searchScenes);
                _searchPrefabs = EditorGUILayout.Toggle(new GUIContent("Prefabs", ".prefab"), _searchPrefabs);
                _searchScriptableObjects = EditorGUILayout.Toggle(new GUIContent("ScriptableObjects", ".asset"), _searchScriptableObjects);
                EditorGUI.indentLevel--;
            }

            GUI.enabled = _mode == 0 ? _findWhatObject : _mode == 1 ? _findWhatObject && _replaceWithObject : false;
            if (GUILayoutExtensions.ColoredButton($"{(_mode == 0 ? "Search" : _mode == 1 ? "Search & Replace" : "")}", Color.green, GUILayout.Height(35)))
            {
                if (_mode == 1)
                {
                    if (EditorUtility.DisplayDialog("Confirm Find and Replace", $"Are you sure you want to find \"{_findWhatObject.name}\" and replace all its references with \"{_replaceWithObject.name}\"?\nYou cannot undo this operation!", "Ok", "Cancel"))
                    {
                        _stopwatch.Reset();
                        _stopwatch.Start();
                        _searchedResults = Search(_findWhatObjectGUID, _searchFolder, GetFilters());
                        if (_mode == 1)
                        {
                            Replace(_searchedResults, _findWhatObjectGUID, _replaceWithObjectGUID);
                        }
                        _stopwatch.Stop();
                        _searchTimeInMilliseconds = (int)_stopwatch.ElapsedMilliseconds;
                    }
                }
                else
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _searchedResults = Search(_findWhatObjectGUID, _searchFolder, GetFilters());
                    _stopwatch.Stop();
                    _searchTimeInMilliseconds = (int)_stopwatch.ElapsedMilliseconds;
                }
            }
            GUI.enabled = true;

            EditorGUILayout.Space();
            GUILayoutExtensions.HorizontalLine();

            using (var _ = new GUILayout.HorizontalScope())
            {
                GUILayout.Label($"Search Results ({_searchedResults.Count}):", EditorStyles.boldLabel);
                if (GUILayout.Button("Clear Results"))
                {
                    _searchedResults.Clear();
                }
            }

            _scroll = GUILayout.BeginScrollView(_scroll);
            if (_searchedResults.IsNullOrEmpty())
            {
                GUILayout.Label("No search results are found.");
            }
            else
            {
                GUILayout.Label($"Search time: {_searchTimeInMilliseconds} ms.");
                GUI.enabled = false;
                foreach (var result in _searchedResults)
                {
                    EditorGUILayout.ObjectField(result.name, result, typeof(UnityEngine.Object), allowSceneObjects: true);
                }
                GUI.enabled = true;
            }
            GUILayout.EndScrollView();
        }

        private string[] GetFilters()
        {
            var filters = new List<string>();
            if (_searchScenes)
            {
                filters.Add("t:scene");
            }
            if (_searchPrefabs)
            {
                filters.Add("t:prefab");
            }
            if (_searchScriptableObjects)
            {
                filters.Add("t:scriptableobject");
            }
            return filters.ToArray();
        }

        private List<UnityEngine.Object> Search(string targetAssetGUID, string searchInFolder, params string[] filters)
        {
            var results = new List<UnityEngine.Object>();
            var pendingGUIDs = AssetDatabase.FindAssets(string.Join(" ", filters), new string[] { searchInFolder });
            if (!pendingGUIDs.IsNullOrEmpty())
            {
                for (int i = 0; i < pendingGUIDs.Length; i++)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(pendingGUIDs[i]);
                    if (EditorUtility.DisplayCancelableProgressBar($"Searching {assetPath}...", $"Searching for {i} of {pendingGUIDs.Length} asset(s)... {_searchedResults.Count} found.", (float)i / pendingGUIDs.Length))
                    {
                        break;
                    }
                    try
                    {
                        // Do not parse paths that are directories. They throw errors when parsing.
                        if (!Directory.Exists(assetPath) && File.ReadAllText(assetPath).Contains(targetAssetGUID))
                        {
                            results.Add(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath));
                        }
                    }
                    catch (Exception exception)
                    {
                        FoxyLogger.LogException(exception);
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            return results;
        }

        private void Replace(List<UnityEngine.Object> listOfObjectsContainingTargetGUID, string targetGUIDToBeReplaced, string replaceWithThisGUID)
        {
            if (listOfObjectsContainingTargetGUID.IsNullOrEmpty() && string.IsNullOrEmpty(targetGUIDToBeReplaced) || string.IsNullOrEmpty(replaceWithThisGUID))
            {
                FoxyLogger.Log($"Replace function is ignored because there are no targets to be replaced.");
                return;
            }

            for (int i = 0; i < listOfObjectsContainingTargetGUID.Count; i++)
            {
                var obj = listOfObjectsContainingTargetGUID[i];
                var assetPath = AssetDatabase.GetAssetPath(obj);
                if (EditorUtility.DisplayCancelableProgressBar($"Replacing {assetPath}...", $"Replacing {i} of {listOfObjectsContainingTargetGUID.Count} asset(s)...", (float)i / listOfObjectsContainingTargetGUID.Count))
                {
                    break;
                }
                try
                {
                    var newTextContent = File.ReadAllText(assetPath).Replace(targetGUIDToBeReplaced, replaceWithThisGUID);
                    File.WriteAllText(assetPath, newTextContent);
                }
                catch (Exception exception)
                {
                    FoxyLogger.LogException(exception);
                }
            }
            FoxyLogger.Log($"Successfully edited {listOfObjectsContainingTargetGUID.Count} file(s).");
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }
}
