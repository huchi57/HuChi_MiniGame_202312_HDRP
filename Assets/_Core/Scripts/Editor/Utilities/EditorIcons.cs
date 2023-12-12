using UnityEngine;
using UnityEditor;

namespace UrbanFox.Editor
{
    public static class EditorIcons
    {
        public static readonly GUIContent InfoIcon = EditorGUIUtility.IconContent("console.infoicon");
        public static readonly GUIContent WarningIcon = EditorGUIUtility.IconContent("console.warnicon");
        public static readonly GUIContent ErrorIcon = EditorGUIUtility.IconContent("console.erroricon");
        public static readonly GUIContent SearchIcon = EditorGUIUtility.IconContent("d_Search Icon");
        public static readonly GUIContent DuplicateIcon = EditorGUIUtility.IconContent("TreeEditor.Duplicate");
    }
}
