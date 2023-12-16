using UnityEditor;

namespace UrbanFox.MiniGame.Editor
{
    [CustomEditor(typeof(UILocalizedTextFont))]
    public class UILocalizedTextFontEditor : UnityEditor.Editor
    {
        private SerializedProperty m_fontStyleIndex;

        private string[] m_fontStylePopUpNames;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            m_fontStyleIndex.intValue = EditorGUILayout.Popup(m_fontStyleIndex.displayName, m_fontStyleIndex.intValue, m_fontStylePopUpNames);
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            m_fontStyleIndex = serializedObject.FindProperty(nameof(m_fontStyleIndex));
            m_fontStylePopUpNames = LanguageAndFontStyleData.Instance.FontStyleNames;
        }
    }
}
