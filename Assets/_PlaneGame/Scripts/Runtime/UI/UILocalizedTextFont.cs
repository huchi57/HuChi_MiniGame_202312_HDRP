using UnityEngine;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Text))]
    public class UILocalizedTextFont : MonoBehaviour
    {
        [SerializeField, NonEditable] private Text m_text;
        [SerializeField] private int m_fontStyleIndex;

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
            if (LanguageAndFontStyleData.Instance.TryGetFontByLanguageAndStyleIndex(Localization.CurrentLanguageIndex, m_fontStyleIndex, out var font))
            {
                m_text.font = font;
            }
        }
    }
}
