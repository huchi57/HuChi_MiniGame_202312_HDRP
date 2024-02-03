using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.TextCore.Text;

namespace UrbanFox.MiniGame.Editor
{
    [CustomEditor(typeof(LanguageAndFontStyleData))]
    public class LanguageAndFontStyleDataEditor : UnityEditor.Editor
    {
        private const string k_fontStyles = "FontStyles";

        private SerializedProperty m_fontStyleNames;
        private SerializedProperty m_fallbackLanguage;
        private SerializedProperty m_languages;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_fontStyleNames);
            EditorGUILayout.Space();

            GUILayout.Label("Fallback Font Data", EditorStyles.boldLabel);
            var fontStyles = m_fallbackLanguage.FindPropertyRelative(k_fontStyles);
            fontStyles.arraySize = m_fontStyleNames.arraySize;
            for (int i = 0; i < fontStyles.arraySize; i++)
            {
                EditorGUILayout.PropertyField(fontStyles.GetArrayElementAtIndex(i), new GUIContent(m_fontStyleNames.GetArrayElementAtIndex(i).stringValue));
            }
            GUILayoutExtensions.HorizontalLine();
            EditorGUILayout.Space();

            GUILayout.Label("Font Data", EditorStyles.boldLabel);

            if (m_languages.arraySize > 0)
            {
                for (int i = 0; i < m_languages.arraySize; i++)
                {
                    var languageData = m_languages.GetArrayElementAtIndex(i);
                    GUILayout.Label($"Language {i}", EditorStyles.boldLabel);
                    fontStyles = languageData.FindPropertyRelative(k_fontStyles);
                    fontStyles.arraySize = m_fontStyleNames.arraySize;
                    for (int j = 0; j < fontStyles.arraySize; j++)
                    {
                        EditorGUILayout.PropertyField(fontStyles.GetArrayElementAtIndex(j), new GUIContent(m_fontStyleNames.GetArrayElementAtIndex(j).stringValue));
                    }
                    GUILayoutExtensions.HorizontalLine();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Add a language font data by pressing the + button.", MessageType.Info);
            }

            GUILayout.BeginHorizontal();
            if (GUILayoutExtensions.ColoredButton("+", Color.green))
            {
                m_languages.arraySize++;
            }
            GUI.enabled = m_languages.arraySize > 0;
            if (GUILayoutExtensions.ColoredButton("-", Color.red))
            {
                m_languages.arraySize = m_languages.arraySize > 0 ? m_languages.arraySize - 1 : 0;
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            m_fontStyleNames = serializedObject.FindProperty(nameof(m_fontStyleNames));
            m_fallbackLanguage = serializedObject.FindProperty(nameof(m_fallbackLanguage));
            m_languages = serializedObject.FindProperty(nameof(m_languages));
        }
    }
}
