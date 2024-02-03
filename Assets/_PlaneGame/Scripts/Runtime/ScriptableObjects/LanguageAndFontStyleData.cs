using System;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class LanguageAndFontStyleData : ScriptableObjectSingleton<LanguageAndFontStyleData>
    {
        [Serializable]
        public struct LanguageFontData
        {
            public Font[] FontStyles;
        }

        [SerializeField]
        private string[] m_fontStyleNames;

        [SerializeField]
        private LanguageFontData m_fallbackLanguage;

        [SerializeField]
        private LanguageFontData[] m_languages;

        public string[] FontStyleNames => m_fontStyleNames;

        public bool TryGetFontByLanguageAndStyleIndex(int languageIndex, int styleIndex, out Font font)
        {
            if (!languageIndex.IsInRange(m_languages))
            {
                font = null;
                return false;
            }
            if (styleIndex.IsInRange(m_languages[languageIndex].FontStyles))
            {
                font = m_languages[languageIndex].FontStyles[styleIndex];
                return true;
            }
            if (styleIndex.IsInRange(m_fallbackLanguage.FontStyles))
            {
                font = m_fallbackLanguage.FontStyles[styleIndex];
                return true;
            }
            font = null;
            return false;
        }
    }
}
