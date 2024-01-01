using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Text))]
    public class SettingsLanguageButton : MonoBehaviour, IPointerEnterHandler, ISelectHandler, ISubmitHandler
    {
        private const string k_languageNameKey = "LanguageName";

        [SerializeField, NonEditable]
        private Text m_text;

        [SerializeField, Required]
        private UIPageGroup m_pageGroup;

        private int m_targetLanguageIndex;

        public void SetUpButtonValue(int targetLanguageIndex)
        {
            m_targetLanguageIndex = targetLanguageIndex;
            if (k_languageNameKey.TryGetLocalizationOverride(m_targetLanguageIndex, out var value))
            {
                m_text.text = value;
            }
            if (LanguageAndFontStyleData.Instance.TryGetFontByLanguageAndStyleIndex(targetLanguageIndex, 0, out var font))
            {
                m_text.font = font;
            }
        }

        public void Select()
        {
            EventSystemManager.Instance.SelectGameObject(gameObject);
            Localization.SetLanguage(m_targetLanguageIndex);
            SettingsManager.Instance.LanguageIndex = m_targetLanguageIndex;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Select();
        }

        public void OnSelect(BaseEventData eventData)
        {
            Select();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            m_pageGroup.TryGotoPreviousPage();
        }

        private void OnValidate()
        {
            m_text = GetComponent<Text>();
        }
    }
}
