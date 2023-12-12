using UnityEngine;
using UnityEditor;

namespace UrbanFox.Editor
{
    [CustomPropertyDrawer(typeof(TagAttribute), true)]
    public class TagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, $"{label}: TagAttribute only works on strings.");
                return;
            }
            property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
        }
    }
}
