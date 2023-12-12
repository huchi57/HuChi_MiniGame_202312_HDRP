using System.Linq;
using UnityEngine;
using UnityEditor;

namespace UrbanFox
{
    public static class EditorGUILayoutExtensions
    {
        public static int LayerMaskField(GUIContent content, int value, GUIStyle style, params GUILayoutOption[] options)
        { 
            var allValidLayerMasks = Enumerable.Range(0, 31).Select(index => LayerMask.LayerToName(index)).ToArray();
            return EditorGUILayout.MaskField(content, value, allValidLayerMasks, style, options);
        }

        public static int LayerMaskField(GUIContent content, int value, params GUILayoutOption[] options)
        {
            return LayerMaskField(content, value, EditorStyles.popup, options);
        }

        public static int LayerMaskField(string label, int value, GUIStyle style, params GUILayoutOption[] options)
        {
            return LayerMaskField(new GUIContent(label), value, style, options);
        }

        public static int LayerMaskField(string label, int value, params GUILayoutOption[] options)
        {
            return LayerMaskField(label, value, EditorStyles.popup, options);
        }

        public static string SearchText(GUIContent content, string searchText, string clearButtonText = "Clear", int clearButtonWidth = 50)
        {
            GUILayout.BeginHorizontal();
            searchText = EditorGUILayout.TextField(content, searchText);
            if (GUILayout.Button(clearButtonText, GUILayout.Width(clearButtonWidth)))
            {
                searchText = string.Empty;
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();
            return searchText;
        }

        public static string SearchText(string label, string searchText)
        {
            return SearchText(new GUIContent(label), searchText);
        }
    }
}
