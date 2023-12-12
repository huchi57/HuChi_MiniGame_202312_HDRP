using UnityEngine;
using UnityEditor;

namespace UrbanFox.Editor
{
    [CustomPropertyDrawer(typeof(InfoAttribute), true)]
    public class InfoDrawer : PropertyDrawer
    {
        private const int _gap = 2;
        private const int _iconSize = 12;

        private InfoAttribute Info => (InfoAttribute)attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var indentDistance = (EditorGUIUtility.singleLineHeight - _iconSize) / 2;
            var iconRect = new Rect(position)
            {
                x = position.x - _gap + indentDistance,
                y = position.y + indentDistance,
                width = _iconSize,
                height = _iconSize
            };

            EditorGUI.indentLevel--;
            EditorGUI.LabelField(iconRect, new GUIContent(EditorIcons.InfoIcon.image, Info.TooltipContent));
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(position, property, new GUIContent(label.text, Info.TooltipContent), true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
