using UnityEngine;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Text))]
    public class UILocalizedText : MonoBehaviour
    {
        [SerializeField, NonEditable] private Text m_text;
        [SerializeField, LocalizedString] private string m_localizedKey;

        private void OnValidate()
        {
            m_text = GetComponent<Text>();
        }

        private void Start()
        {
            Localization.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnDestroy()
        {
            Localization.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged()
        {
            if (m_localizedKey.TryGetLocalization(out var value))
            {
                m_text.text = value;
            }
            else
            {
                m_text.text = $"!!{m_localizedKey}";
            }
        }
    }
}
