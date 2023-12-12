using UnityEngine;
using UnityEditor;

namespace UrbanFox.Editor
{
    [CustomPropertyDrawer(typeof(LayerAttribute), true)]
    class LayerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, $"{label}: LayerAttribute only works on integers.");
                return;
            }
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
    }
}
