using UnityEditor;
using UnityEngine;

namespace UrbanFox.Editor
{
    [CustomPropertyDrawer(typeof(IntParameter), true)]
    public class IntParameterDrawer : PropertyDrawer
    {
        private const string k_valueSource = "m_valueSource";
        private const string k_value = "m_value";
        private const string k_parameterAsset = "m_parameterAsset";

        private const string k_direct = "Direct";
        private const string k_asset = "Asset";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelRect = new Rect(position) { width = position.width / 3 };
            var toggleRect = new Rect(position) { width = position.width / 3, x = labelRect.x + labelRect.width };
            var valueRect = new Rect(position) { width = position.width / 3 - 5, x = toggleRect.x + toggleRect.width + 5};

            var valueSource = property.FindPropertyRelative(k_valueSource);
            var button1Rect = new Rect(toggleRect) { width = toggleRect.width / 2 };
            var button2Rect = new Rect(button1Rect) { x = button1Rect.x + button1Rect.width };

            GUI.Label(labelRect, label);

            var cacheColor = GUI.backgroundColor;
            if ((ValueSource)valueSource.enumValueIndex == ValueSource.Direct)
            {
                GUI.backgroundColor = Color.yellow;
                GUI.Button(button1Rect, k_direct);
                GUI.backgroundColor = cacheColor;
                if (GUI.Button(button2Rect, k_asset))
                {
                    valueSource.enumValueIndex = (int)ValueSource.ScriptableObject;
                }
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative(k_value), GUIContent.none);
            }
            else
            {
                if (GUI.Button(button1Rect, k_direct))
                {
                    valueSource.enumValueIndex = (int)ValueSource.Direct;
                }
                GUI.backgroundColor = Color.yellow;
                GUI.Button(button2Rect, k_asset);
                GUI.backgroundColor = cacheColor;
                var parameterAsset = property.FindPropertyRelative(k_parameterAsset);
                if (parameterAsset.objectReferenceValue == null)
                {
                    GUI.backgroundColor = Color.red;
                }
                EditorGUI.PropertyField(valueRect, parameterAsset, GUIContent.none);
                GUI.backgroundColor = cacheColor;
            }
        }
    }
}
