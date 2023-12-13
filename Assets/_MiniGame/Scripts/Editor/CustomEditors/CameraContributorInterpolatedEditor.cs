using UnityEngine;
using UnityEditor;
using UrbanFox.Editor;

namespace UrbanFox.MiniGame.Editor
{
    [CustomEditor(typeof(CameraContributorInterpolated))]
    public class CameraContributorInterpolatedEditor : UnityEditor.Editor
    {
        private const string k_referencePoint = nameof(CameraContributorInterpolated.PointData.ReferencePoint);
        private const string k_distanceFromTargetToCamera = nameof(CameraContributorInterpolated.PointData.DistanceFromTargetToCamera);
        private const string k_positionLerpSpeed = nameof(CameraContributorInterpolated.PointData.PositionLerpSpeed);
        private const string k_lookAtOffsetDistanceFromTarget = nameof(CameraContributorInterpolated.PointData.LookAtOffsetDistanceFromTarget);
        private const string k_rotationSlerpSpeed = nameof(CameraContributorInterpolated.PointData.RotationSlerpSpeed);
        private const string k_FOV = nameof(CameraContributorInterpolated.PointData.FOV);
        private const string k_FOVLerpSpeed = nameof(CameraContributorInterpolated.PointData.FOVLerpSpeed);

        private CameraContributorInterpolated m_target;
        private SerializedProperty m_referencePoints;
        private SerializedProperty m_referenceSpace;

        private bool m_foldOut = true;
        private bool m_hidePositionData = true;

        private Camera m_previewCamera;
        private int m_currentPreviewIndex;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            m_foldOut = EditorGUILayout.Foldout(m_foldOut, $"Reference Points ({m_referencePoints.arraySize})", toggleOnLabelClick: true);

            if (m_foldOut)
            {
                m_hidePositionData = EditorGUILayout.Toggle("Hide Position Data (Debug Only)", m_hidePositionData);
                GUILayoutExtensions.HorizontalLine();
                if (m_referencePoints.arraySize > 0)
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < m_referencePoints.arraySize; i++)
                    {
                        var point = m_referencePoints.GetArrayElementAtIndex(i);
                        DrawInspectorReferencePointFields(point, i);
                    }
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUILayout.HelpBox($"No reference points found! Press + to start adding points.", MessageType.Error);
                }
            }
            GUILayout.BeginHorizontal();
            if (GUILayoutExtensions.ColoredButton("-", Color.red))
            {
                m_referencePoints.arraySize = m_referencePoints.arraySize > 0 ? m_referencePoints.arraySize - 1 : 0;
            }
            if (GUILayoutExtensions.ColoredButton("+", Color.green))
            {
                m_referencePoints.arraySize++;
                if (m_referencePoints.arraySize == 1)
                {
                    var newReferencePoint = m_referencePoints.GetArrayElementAtIndex(0);
                    newReferencePoint.FindPropertyRelative(k_referencePoint).vector3Value = new Vector3(1, 0, 0);
                    newReferencePoint.FindPropertyRelative(k_lookAtOffsetDistanceFromTarget).vector3Value = new Vector3(0.25f, 0, 0);
                    newReferencePoint.FindPropertyRelative(k_positionLerpSpeed).floatValue = 1;
                    newReferencePoint.FindPropertyRelative(k_distanceFromTargetToCamera).vector3Value = new Vector3(0, 2, -5);
                    newReferencePoint.FindPropertyRelative(k_rotationSlerpSpeed).floatValue = 1;
                    newReferencePoint.FindPropertyRelative(k_FOV).floatValue = 60;
                    newReferencePoint.FindPropertyRelative(k_FOVLerpSpeed).floatValue = 1;
                }
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button($"Convert Reference Points to {(m_referenceSpace.enumValueIndex == (int)Space.World ? "LOCAL" : "WORLD")}"))
            {
                m_referenceSpace.enumValueIndex = m_referenceSpace.enumValueIndex == (int)Space.World ? (int)Space.Self : (int)Space.World;
                if (m_referencePoints.arraySize > 0)
                {
                    for (int i = 0; i < m_referencePoints.arraySize; i++)
                    {
                        var referencePoint = m_referencePoints.GetArrayElementAtIndex(i).FindPropertyRelative(k_referencePoint); 
                        referencePoint.vector3Value += (m_referenceSpace.enumValueIndex == (int)Space.World ? 1 : -1) * m_target.transform.position;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInspectorReferencePointFields(SerializedProperty point, int index)
        {
            var referencePoint = point.FindPropertyRelative(k_referencePoint);
            var distanceFromTargetToCamera = point.FindPropertyRelative(k_distanceFromTargetToCamera);
            var positionLerpSpeed = point.FindPropertyRelative(k_positionLerpSpeed);
            var lookAtOffsetDistanceFromTarget = point.FindPropertyRelative(k_lookAtOffsetDistanceFromTarget);
            var rotatioinSlerpSpeed = point.FindPropertyRelative(k_rotationSlerpSpeed);
            var fov = point.FindPropertyRelative(k_FOV);
            var fovLerpSpeed = point.FindPropertyRelative(k_FOVLerpSpeed);

            GUILayout.BeginHorizontal();
            GUILayout.Label($"POINT {index}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.Label("Preview Cam:");
            if (GUILayoutExtensions.ColoredButton("Preview Here", Color.green, GUILayout.MaxWidth(100)))
            {
                m_currentPreviewIndex = index;
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
            if (!m_hidePositionData)
            {
                EditorGUILayout.PropertyField(referencePoint);
                EditorGUILayout.Space();
            }

            GUILayout.Label("Position Effectors", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            if (!m_hidePositionData)
            {
                EditorGUILayout.PropertyField(distanceFromTargetToCamera);
            }
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
            if (!m_hidePositionData)
            {
                EditorGUILayout.PropertyField(lookAtOffsetDistanceFromTarget);
            }
            EditorGUILayout.PropertyField(rotatioinSlerpSpeed, new GUIContent("Slerp Speed"));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            GUILayout.Label("Lens Effectors", EditorStyles.boldLabel);
            EditorGUILayout.Slider(fov, 0, 180);
            EditorGUILayout.PropertyField(fovLerpSpeed);

            GUILayoutExtensions.HorizontalLine();
        }

        private void OnEnable()
        {
            m_target = (CameraContributorInterpolated)target;
            m_referencePoints = serializedObject.FindProperty(nameof(m_referencePoints));
            m_referenceSpace = serializedObject.FindProperty(nameof(m_referenceSpace));
        }

        private void OnDestroy()
        {
            if (m_previewCamera)
            {
                DestroyImmediate(m_previewCamera.gameObject);
            }
        }

        private void OnSceneGUI()
        {
            serializedObject.Update();
            for (int i = 0; i < m_referencePoints.arraySize; i++)
            {
                var point = m_referencePoints.GetArrayElementAtIndex(i);
                DrawSceneReferencePoint(point);
            }
            UpdatePreviewCamera();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSceneReferencePoint(SerializedProperty point)
        {
            var referencePoint = point.FindPropertyRelative(k_referencePoint);
            var distanceFromTargetToCamera = point.FindPropertyRelative(k_distanceFromTargetToCamera);
            var lookAtOffsetDistanceFromTarget = point.FindPropertyRelative(k_lookAtOffsetDistanceFromTarget);

            if (m_referenceSpace.enumValueIndex == (int)Space.Self)
            {
                referencePoint.vector3Value += m_target.transform.position;
            }

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

            if (m_referenceSpace.enumValueIndex == (int)Space.Self)
            {
                referencePoint.vector3Value -= m_target.transform.position;
            }
        }

        private void UpdatePreviewCamera()
        {
            if (m_currentPreviewIndex >= m_referencePoints.arraySize && m_previewCamera)
            {
                DestroyImmediate(m_previewCamera.gameObject);
                SceneView.RepaintAll();
            }
            if (!m_previewCamera)
            {
                return;
            }
            var currentPreviewPoint = m_referencePoints.GetArrayElementAtIndex(m_currentPreviewIndex);
            var referencePoint = currentPreviewPoint.FindPropertyRelative(k_referencePoint);
            var distanceFromTargetToCamera = currentPreviewPoint.FindPropertyRelative(k_distanceFromTargetToCamera);
            var lookAtOffsetDistanceFromTarget = currentPreviewPoint.FindPropertyRelative(k_lookAtOffsetDistanceFromTarget);
            var fov = currentPreviewPoint.FindPropertyRelative(k_FOV);
            var offset = (m_referenceSpace.enumValueIndex == (int)Space.World ? Vector3.zero : m_target.transform.position);
            var cameraPosition = offset + referencePoint.vector3Value + distanceFromTargetToCamera.vector3Value;
            var cameraRotation = Quaternion.LookRotation(lookAtOffsetDistanceFromTarget.vector3Value - distanceFromTargetToCamera.vector3Value);
            m_previewCamera.fieldOfView = fov.floatValue;
            m_previewCamera.transform.SetPositionAndRotation(cameraPosition, cameraRotation);
        }
    }
}
