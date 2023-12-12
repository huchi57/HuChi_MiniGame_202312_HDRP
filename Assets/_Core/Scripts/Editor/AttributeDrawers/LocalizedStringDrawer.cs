using System.Text;
using UnityEngine;
using UnityEditor;

namespace UrbanFox.Editor
{
    [CustomPropertyDrawer(typeof(LocalizedStringAttribute), true)]
    public class LocalizedStringDrawer : PropertyDrawer
    {
        private const int m_gap = 2;
        private const int m_searchIconWidth = 20;
        private const int m_localizationBrowserRectHeight = 200;

        [SerializeField] private bool m_isExpanded;

        private int m_numberOfKeysMatch = 0;
        private Vector2 m_scrollView;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                position.height = EditorGUIUtility.singleLineHeight;
                GUI.Label(position, $"{label}: SerializedString only works with string data type");
                return;
            }

            // Force single line property
            position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            var propertyRect = new Rect(position.x, position.y, position.width - m_searchIconWidth - m_gap, position.height);
            property.stringValue = EditorGUI.TextField(propertyRect, label, property.stringValue);

            // Draw search icon button
            var searchIconRect = new Rect(propertyRect.xMax + m_gap, propertyRect.y, m_searchIconWidth, propertyRect.height);
            if (GUI.Button(searchIconRect, EditorIcons.SearchIcon))
            {
                m_isExpanded = !m_isExpanded;
            }

            if (m_isExpanded)
            {
                if (!Localization.IsInitialized || Localization.Keys.Count <= 0)
                {
                    m_isExpanded = false;
                    FoxyLogger.LogWarning($"Cannot expand the browser because Localization files cannot be found under the Resources folder, or there are no Localization keys.");
                    return;
                }

                // Draw browser scroll view
                var positionRect = new Rect(position.x, position.yMax + m_gap, position.width, m_localizationBrowserRectHeight);
                var viewRect = new Rect(0, 0, position.width, m_numberOfKeysMatch * EditorGUIUtility.singleLineHeight);
                m_scrollView = GUI.BeginScrollView(positionRect, m_scrollView, viewRect);
                var keyRect = new Rect(0, 0, position.width, EditorGUIUtility.singleLineHeight);

                var currentKeyLowered = property.stringValue.ToLower();
                m_numberOfKeysMatch = 0;
                for (int i = 0; i < Localization.Keys.Count; i++)
                {
                    var key = Localization.Keys[i];
                    if (key.ToLower().Contains(currentKeyLowered))
                    {
                        if (GUI.Button(keyRect, new GUIContent(key, GetKeyPreviews(key))))
                        {
                            property.stringValue = key;
                            m_isExpanded = false;
                            GUI.FocusControl(null);
                        }
                        keyRect.y += EditorGUIUtility.singleLineHeight;
                        m_numberOfKeysMatch++;
                    }
                }
                GUI.EndScrollView();
            }
            else
            {
                // Not expanded: draw a second line showing the localization preview
                var previewRectWidth = Mathf.Min(300, position.width - 20);
                var localizationPreviewRect = new Rect(position.xMax - previewRectWidth, position.yMax + m_gap, previewRectWidth, EditorGUIUtility.singleLineHeight);
                if (property.stringValue.TryGetLocalization(out _))
                {
                    GUI.Label(localizationPreviewRect, new GUIContent(property.stringValue, GetKeyPreviews(property.stringValue)));
                }
                else
                {
                    var cacheGUIColor = GUI.contentColor;
                    GUI.contentColor = Color.yellow;
                    GUI.Label(localizationPreviewRect, $"No localization found.");
                    GUI.contentColor = cacheGUIColor;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            if (m_isExpanded)
            {
                if (!Localization.IsInitialized || Localization.Keys.Count <= 0)
                {
                    return 2 * EditorGUIUtility.singleLineHeight;
                }
                return EditorGUIUtility.singleLineHeight + m_localizationBrowserRectHeight;
            }
            return 2 * EditorGUIUtility.singleLineHeight;
        }

        private string GetKeyPreviews(string key)
        {
            var preview = new StringBuilder();
            if (Localization.IsInitialized && Localization.NumberOfLanguages > 0)
            {
                preview.Append($"{Localization.GetLanguageCodeNameByLanguageIndex(0)}: {key.GetLocalizationOverride(0)}");
                for (int i = 1; i < Localization.NumberOfLanguages; i++)
                {
                    preview.Append($"\n{Localization.GetLanguageCodeNameByLanguageIndex(i)}: {key.GetLocalizationOverride(i)}");
                }
            }
            return preview.ToString();
        }
    }
}
