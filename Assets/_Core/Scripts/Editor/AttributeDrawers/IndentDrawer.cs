using UnityEngine;
using UnityEditor;

namespace UrbanFox.Editor
{
    [CustomPropertyDrawer(typeof(IndentAttribute), true)]
    public class IndentDrawer : PropertyDrawer
    {
        private IndentAttribute Indent => (IndentAttribute)attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var cacheGUIIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel += Indent.IndentIncrementLevel;
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.indentLevel = cacheGUIIndentLevel;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
