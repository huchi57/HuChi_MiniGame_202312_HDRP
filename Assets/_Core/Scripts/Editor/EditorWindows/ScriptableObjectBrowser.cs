using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace UrbanFox.Editor
{
    public class ScriptableObjectBrowser : EditorWindow
    {
        private static Vector2 _masterScroll = default;
        private static Vector2 _classSelectorScroll = default;
        private static Vector2 _instanceSelectorScroll = default;
        private static Vector2 _instancePreviewScroll = default;
        private static GUIStyle _cachedButtonAlignLeft = null;

        private static List<Type> _allTypes = null;

        // Class selector column
        [SerializeField] private string _searchClassText = string.Empty;
        [SerializeField] private bool _ignoreUnityClasses = true;
        [SerializeField] private bool _showClassFullName = false;
        [SerializeField] private int _selectedTypeIndex = -1;

        // Instance selector column
        [SerializeField] private string _searchInstanceText = string.Empty;
        [SerializeField] private UnityEngine.Object _selectedInstance = null;

        private int _visibleClassCount = 0;
        private string _cachedSearchClassText = string.Empty;
        private string _cachedSearchInstanceText = string.Empty;

        private static GUIStyle ButtonAlignLeft
        {
            get
            {
                if (_cachedButtonAlignLeft == null)
                {
                    _cachedButtonAlignLeft = new GUIStyle(GUI.skin.button)
                    {
                        alignment = TextAnchor.MiddleLeft
                    };
                }
                return _cachedButtonAlignLeft;
            }
        }

        private Type SelectedType
        {
            get
            {
                if (_selectedTypeIndex.IsInRange(_allTypes))
                {
                    return _allTypes[_selectedTypeIndex];
                }
                return null;
            }
        }

        [MenuItem("OwO/Window/ScriptableObject Browser...")]
        private static void Init()
        {
            var window = GetWindow<ScriptableObjectBrowser>();
            window.titleContent = new GUIContent("Scriptable Browser");
            window.minSize = new Vector2(500, 500);
            window.Show();
        }

        private void OnEnable()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            _allTypes = new List<Type>();
            foreach (var assembly in allAssemblies)
            {
                var newTypes = assembly.GetTypes().Where(type => type.IsClass

                    // Include ScriptableObject classes
                    && type.IsSubclassOf(typeof(ScriptableObject))

                    // Exclude abstract and generic classes (they cannot be instantiated)
                    && !type.IsAbstract
                    && !type.IsGenericType

                    // Exclude special editor windows
                    && !type.IsSubclassOf(typeof(UnityEditor.Editor))
                    && !type.IsSubclassOf(typeof(EditorWindow)));

                _allTypes.AddRange(newTypes);
            }
        }

        private void OnGUI()
        {
            _masterScroll = GUILayout.BeginScrollView(_masterScroll);
            using (var _ = new GUILayout.HorizontalScope())
            {
                GUILayout.BeginVertical(GUILayout.Width(300));
                DrawClassSelectorColumn();
                GUILayout.EndVertical();

                GUILayout.BeginVertical(GUILayout.Width(300));
                DrawInstanceSelectorColumn();
                GUILayout.EndVertical();

                GUILayout.BeginVertical(GUILayout.MinWidth(300), GUILayout.MaxWidth(2000));
                DrawInstancePreviewColumn();
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
        }

        private void DrawClassSelectorColumn()
        {
            GUILayout.Label($"Class Selector", EditorStyles.boldLabel);
            GUILayoutExtensions.HorizontalLine();

            _searchClassText = EditorGUILayoutExtensions.SearchText("Search Type", _searchClassText);
            _ignoreUnityClasses = EditorGUILayout.Toggle("Ignore Unity Classes", _ignoreUnityClasses);
            _showClassFullName = EditorGUILayout.Toggle("Show Class Full Name", _showClassFullName);
            _cachedSearchClassText = _searchClassText.ToLower();

            EditorGUILayout.Space();

            GUILayout.Label($"Scriptable Objects ({_visibleClassCount})", EditorStyles.boldLabel);
            GUILayoutExtensions.HorizontalLine();
            _visibleClassCount = 0;

            _classSelectorScroll = GUILayout.BeginScrollView(_classSelectorScroll);
            if (!_allTypes.IsNullOrEmpty())
            {
                for (int i = 0; i < _allTypes.Count; i++)
                {
                    var type = _allTypes[i];
                    if ((!_ignoreUnityClasses || (_ignoreUnityClasses && !string.IsNullOrEmpty(type.Namespace) && !type.Namespace.Contains("Unity")))
                        && type.FullName.ToLower().Contains(_cachedSearchClassText))
                    {
                        _visibleClassCount++;
                        var buttonContent = new GUIContent(_showClassFullName ? type.FullName : type.Name, type.FullName);
                        GUILayout.BeginHorizontal();
                        if (GUILayoutExtensions.ColoredButton(buttonContent, type == SelectedType ? Color.yellow : Color.white, ButtonAlignLeft, GUILayout.Width(235)))
                        {
                            _selectedTypeIndex = i;
                            _selectedInstance = null;
                        }
                        if (GUILayoutExtensions.ColoredButton("New", Color.green, GUILayout.Width(40)))
                        {
                            var newInstance = CreateInstance(type);
                            var instancePath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(IOUtility.GetCurrentFolderInProjectPanel(), $"{type.Name}.asset"));
                            ProjectWindowUtil.CreateAsset(newInstance, instancePath);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        private void DrawInstanceSelectorColumn()
        {
            GUILayout.Label($"Instance Selector", EditorStyles.boldLabel);
            GUILayoutExtensions.HorizontalLine();

            if (SelectedType == null)
            {
                EditorGUILayout.HelpBox($"Select a valid type from the left column to browse all instances of the type.", MessageType.Info);
            }
            else
            {
                var instanceGUIDs = AssetDatabase.FindAssets($"{_cachedSearchInstanceText} t:{SelectedType.Name}");
                GUILayout.Label($"{SelectedType.Name} ({(instanceGUIDs.IsNullOrEmpty() ? "0" : instanceGUIDs.Length.ToString())})", EditorStyles.boldLabel);
                _searchInstanceText = EditorGUILayoutExtensions.SearchText("Search By Name", _searchInstanceText);
                _cachedSearchInstanceText = _searchInstanceText.ToLower();
                EditorGUILayout.Space();

                if (instanceGUIDs.IsNullOrEmpty())
                {
                    EditorGUILayout.HelpBox($"There are no instances of {SelectedType.Name}.", MessageType.Info);
                }
                else
                {
                    _instanceSelectorScroll = GUILayout.BeginScrollView(_instanceSelectorScroll);
                    for (int i = 0; i < instanceGUIDs.Length; i++)
                    {
                        var instancePath = AssetDatabase.GUIDToAssetPath(instanceGUIDs[i]);
                        var instance = AssetDatabase.LoadAssetAtPath(instancePath, SelectedType);
                        var buttonContent = new GUIContent(instance ? instance.name : Path.GetFileNameWithoutExtension(instancePath), instancePath);
                        GUILayout.BeginHorizontal();
                        if (GUILayoutExtensions.ColoredButton(buttonContent, instance != null && instance == _selectedInstance ? Color.yellow : Color.white, ButtonAlignLeft))
                        {
                            _selectedInstance = instance;
                        }
                        if (GUILayout.Button("Ping", GUILayout.Width(40)))
                        {
                            EditorGUIUtility.PingObject(instance);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndScrollView();
                }
            }
        }

        private void DrawInstancePreviewColumn()
        {
            GUILayout.Label($"Instance Preview", EditorStyles.boldLabel);
            GUILayoutExtensions.HorizontalLine();

            if (_selectedInstance == null)
            {
                EditorGUILayout.HelpBox($"Select a valid instace from the middle column to inspect the object.", MessageType.Info);
            }
            else
            {
                GUILayout.Label($"{_selectedInstance.name}", EditorStyles.boldLabel);
                GUILayout.Label($"Path: {AssetDatabase.GetAssetPath(_selectedInstance)}");
                GUILayoutExtensions.HorizontalLine();

                _instancePreviewScroll = GUILayout.BeginScrollView(_instancePreviewScroll);
                var editor = UnityEditor.Editor.CreateEditor(_selectedInstance);
                editor.OnInspectorGUI();
                GUILayout.EndScrollView();
            }
        }
    }
}
