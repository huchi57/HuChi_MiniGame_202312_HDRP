using System;
using UnityEngine;
using UnityEditor;

namespace UrbanFox.Editor
{
    [CustomPropertyDrawer(typeof(GUIDAttribute), true)]
    public class GUIDDrawer : PropertyDrawer
    {
        private const int _buttonWidth = 70;
        private const int _gap = 2;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String || property.propertyType != SerializedPropertyType.Integer)
            {
                GUI.Label(position, "GUID is only available with string / integer types.");
                return;
            }

            var propertyRect = new Rect(position)
            {
                width = position.width - _buttonWidth - _gap
            };

            var buttonRect = new Rect(position)
            {
                x = position.xMax - _buttonWidth,
                width = _buttonWidth
            };

            EditorGUI.PropertyField(propertyRect, property, label, true);
            if (GUI.Button(buttonRect, "New ID"))
            {
                if (property.propertyType == SerializedPropertyType.String)
                {
                    property.stringValue = Guid.NewGuid().ToString();
                }
                else if (property.propertyType == SerializedPropertyType.Integer)
                {
                    property.intValue = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                }
                GUI.FocusControl(null);
            }

            // Check for empty values
            if (property.propertyType == SerializedPropertyType.String && string.IsNullOrWhiteSpace(property.stringValue))
            {
                property.stringValue = Guid.NewGuid().ToString();
            }
            else if (property.propertyType == SerializedPropertyType.Integer && property.intValue == 0)
            {
                property.intValue = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
