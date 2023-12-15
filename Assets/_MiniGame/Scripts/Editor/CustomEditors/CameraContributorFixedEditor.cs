using UnityEngine;
using UnityEditor;
using UrbanFox.Editor;

namespace UrbanFox.MiniGame.Editor
{
    [CustomEditor(typeof(CameraContributorFixed))]
    public class CameraContributorFixedEditor : UnityEditor.Editor
    {
        private const string k_referencePoint = nameof(CameraContributorPointData.ReferencePoint);
        private const string k_distanceFromTargetToCamera = nameof(CameraContributorPointData.DistanceFromTargetToCamera);
        private const string k_positionOffsetAfterLookAt = nameof(CameraContributorPointData.PositionOffsetAfterLookAt);
        private const string k_positionLerpSpeed = nameof(CameraContributorPointData.PositionLerpSpeed);
        private const string k_lookAtOffsetDistanceFromTarget = nameof(CameraContributorPointData.LookAtOffsetDistanceFromTarget);
        private const string k_rotationSlerpSpeed = nameof(CameraContributorPointData.RotationSlerpSpeed);
        private const string k_FOV = nameof(CameraContributorPointData.FOV);
        private const string k_FOVLerpSpeed = nameof(CameraContributorPointData.FOVLerpSpeed);

        [SerializeField]
        private bool m_showPositionData = true;

        private SerializedProperty m_pointData;
        private Camera m_previewCamera;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            m_showPositionData = EditorGUILayout.Toggle("(Debug) Show Position Data", m_showPositionData);

            var referencePoint = m_pointData.FindPropertyRelative(k_referencePoint);
            var distanceFromTargetToCamera = m_pointData.FindPropertyRelative(k_distanceFromTargetToCamera);
            var positionOffsetAfterLookAt = m_pointData.FindPropertyRelative(k_positionOffsetAfterLookAt);
            var positionLerpSpeed = m_pointData.FindPropertyRelative(k_positionLerpSpeed);
            var lookAtOffsetDistanceFromTarget = m_pointData.FindPropertyRelative(k_lookAtOffsetDistanceFromTarget);
            var rotatioinSlerpSpeed = m_pointData.FindPropertyRelative(k_rotationSlerpSpeed);
            var fov = m_pointData.FindPropertyRelative(k_FOV);
            var fovLerpSpeed = m_pointData.FindPropertyRelative(k_FOVLerpSpeed);

            GUILayoutExtensions.HorizontalLine();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"POINT DATA", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.Label("Preview Cam:");
            if (GUILayoutExtensions.ColoredButton("Preview Here", Color.green, GUILayout.MaxWidth(100)))
            {
                if (!m_previewCamera)
                {
                    m_previewCamera = new GameObject().AddComponent<Camera>();
                    m_previewCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
                }
            }
            if (GUILayoutExtensions.ColoredButton("Clear Preview", Color.red, GUILayout.MaxWidth(100)))
            {
                if (m_previewCamera)
                {
                    DestroyImmediate(m_previewCamera.gameObject);
                    SceneView.RepaintAll();
                }
            }
            GUILayout.EndHorizontal();
            if (m_showPositionData)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(referencePoint);
                GUI.enabled = true;
                EditorGUILayout.Space();
            }

            GUILayout.Label("Position Effectors", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            if (m_showPositionData)
            {
                EditorGUILayout.PropertyField(distanceFromTargetToCamera);
            }
            EditorGUILayout.PropertyField(positionOffsetAfterLookAt, new GUIContent("Position Offset After Look At"));
            EditorGUILayout.PropertyField(positionLerpSpeed, new GUIContent("Lerp Speed"));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Rotation Effectors", EditorStyles.boldLabel);
            if (GUILayout.Button("Reset Look At Offset", GUILayout.MaxWidth(160)))
            {
                lookAtOffsetDistanceFromTarget.vector3Value = Vector3.zero;
            }
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            if (m_showPositionData)
            {
                EditorGUILayout.PropertyField(lookAtOffsetDistanceFromTarget);
            }
            EditorGUILayout.PropertyField(rotatioinSlerpSpeed, new GUIContent("Slerp Speed"));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            GUILayout.Label("Lens Effectors", EditorStyles.boldLabel);
            EditorGUILayout.Slider(fov, 0, 180);
            EditorGUILayout.PropertyField(fovLerpSpeed);

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            m_pointData = serializedObject.FindProperty(nameof(m_pointData));
        }

        private void OnDisable()
        {
            if (m_previewCamera)
            {
                DestroyImmediate(m_previewCamera.gameObject);
            }
        }

        private void OnSceneGUI()
        {
            serializedObject.Update();

            var referencePoint = m_pointData.FindPropertyRelative(k_referencePoint);
            var distanceFromTargetToCamera = m_pointData.FindPropertyRelative(k_distanceFromTargetToCamera);
            var positionOffsetAfterLookAt = m_pointData.FindPropertyRelative(k_positionOffsetAfterLookAt);
            var lookAtOffsetDistanceFromTarget = m_pointData.FindPropertyRelative(k_lookAtOffsetDistanceFromTarget);
            var fov = m_pointData.FindPropertyRelative(k_FOV);

            referencePoint.vector3Value = Handles.PositionHandle(referencePoint.vector3Value, Quaternion.identity);
            HandlesExtensions.DrawSolidSphere(referencePoint.vector3Value, 0.08f, Color.white);

            lookAtOffsetDistanceFromTarget.vector3Value += referencePoint.vector3Value;
            lookAtOffsetDistanceFromTarget.vector3Value = Handles.PositionHandle(lookAtOffsetDistanceFromTarget.vector3Value, Quaternion.identity);
            HandlesExtensions.DrawLine(referencePoint.vector3Value, lookAtOffsetDistanceFromTarget.vector3Value, Color.white);
            HandlesExtensions.DrawSolidSphere(lookAtOffsetDistanceFromTarget.vector3Value, 0.05f, Color.yellow);
            lookAtOffsetDistanceFromTarget.vector3Value -= referencePoint.vector3Value;

            distanceFromTargetToCamera.vector3Value += referencePoint.vector3Value;
            distanceFromTargetToCamera.vector3Value = Handles.PositionHandle(distanceFromTargetToCamera.vector3Value, Quaternion.identity);
            HandlesExtensions.DrawDottedLine(referencePoint.vector3Value, distanceFromTargetToCamera.vector3Value, 5, Color.white);
            HandlesExtensions.DrawWireSphere(distanceFromTargetToCamera.vector3Value, 0.05f, Color.cyan);
            distanceFromTargetToCamera.vector3Value -= referencePoint.vector3Value;

            serializedObject.ApplyModifiedProperties();

            if (!m_previewCamera)
            {
                return;
            }
            var cameraPosition = referencePoint.vector3Value + distanceFromTargetToCamera.vector3Value;
            var cameraRotation = Quaternion.LookRotation(lookAtOffsetDistanceFromTarget.vector3Value - distanceFromTargetToCamera.vector3Value);
            m_previewCamera.fieldOfView = fov.floatValue;
            m_previewCamera.transform.SetPositionAndRotation(cameraPosition + positionOffsetAfterLookAt.vector3Value, cameraRotation);
        }
    }
}
