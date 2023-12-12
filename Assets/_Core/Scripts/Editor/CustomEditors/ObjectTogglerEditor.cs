using UnityEngine;
using UnityEditor;

namespace UrbanFox.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ObjectToggler), true)]
    public class ObjectTogglerEditor : UnityEditor.Editor
    {
        private ObjectToggler m_target;
        private SerializedProperty m_objects;

        private void OnEnable()
        {
            m_target = (ObjectToggler)target;
            m_objects = serializedObject.FindProperty(nameof(m_objects));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            GUILayout.Label("Objects", EditorStyles.boldLabel);

            if (m_objects.arraySize <= 0)
            {
                EditorGUILayout.HelpBox("List is empty.", MessageType.Info);
            }

            for (int i = 0; i < m_objects.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                var inspectorProperty = m_objects.GetArrayElementAtIndex(i);
                EditorGUILayout.ObjectField(inspectorProperty);

                var referenceObject = (GameObject)inspectorProperty.objectReferenceValue;
                var isTargetActive = referenceObject && referenceObject.activeSelf;
                GUI.enabled = referenceObject;
                if (GUILayoutExtensions.ColoredButton(!referenceObject ? "-" : isTargetActive ? "Active" : "Inactive", !referenceObject ? Color.gray : isTargetActive ? Color.green : Color.red, GUILayout.Width(60)))
                {
                    if (isTargetActive)
                    {
                        m_target.ToggleObjectOff(i);
                    }
                    else
                    {
                        m_target.ToggleObjectOn(i);
                    }
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUI.enabled = m_objects.arraySize >= 1;
            if (GUILayout.Button("-"))
            {
                if (m_objects.arraySize > 0)
                {
                    m_objects.arraySize--;
                }
            }
            GUI.enabled = true;
            if (GUILayout.Button("+"))
            {
                m_objects.arraySize++;
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.Label("Operations", EditorStyles.boldLabel);
            if(GUILayout.Button("Override Objects With Child Objects"))
            {
                m_target.PopulateObjectsWithChildren();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
