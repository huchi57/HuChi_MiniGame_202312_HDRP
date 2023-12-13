using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Text))]
    public class SettingsLanguageButton : MonoBehaviour, IPointerEnterHandler, ISelectHandler
    {
        private const string k_languageNameKey = "LanguageName";

        [SerializeField, NonEditable]
        private Text m_text;

        private int m_targetLanguageIndex;

        public void SetUpButtonValue(int targetLanguageIndex)
        {
            m_targetLanguageIndex = targetLanguageIndex;
            if (k_languageNameKey.TryGetLocalizationOverride(m_targetLanguageIndex, out var value))
            {
                m_text.text = value;
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

        private void OnValidate()
        {
            m_text = GetComponent<Text>();
        }
    }
}
