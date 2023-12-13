using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;

namespace UrbanFox.MiniGame.Editor
{
    [CustomEditor(typeof(SettingsManager))]
    public class SettingsManagerEditor : UnityEditor.Editor
    {
        private SettingsManager m_target;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            if (GUILayoutExtensions.ColoredButton("Open Settings File Folder", Color.white))
            {
                EditorUtility.RevealInFinder(Application.persistentDataPath);
            }

            GUI.enabled = File.Exists(m_target.SettingsFilePath);
            if (GUILayoutExtensions.ColoredButton("Delete Settings File", Color.red))
            {
                if (File.Exists(m_target.SettingsFilePath))
                {
                    File.Delete(m_target.SettingsFilePath);
                    FoxyLogger.Log($"File {m_target.SettingsFilePath} has been removed.");
                }
            }
            GUI.enabled = true;

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            m_target = (SettingsManager)target;
        }
    }
}
