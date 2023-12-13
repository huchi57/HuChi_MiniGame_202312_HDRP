using System.Collections.Generic;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class SettingsLanguageSelector : MonoBehaviour
    {
        [SerializeField, Required]
        private SettingsLanguageButton m_buttonTemplate;

        [SerializeField, Required]
        private Transform m_buttonHolder;

        private readonly List<SettingsLanguageButton> m_buttons = new List<SettingsLanguageButton>();

        private void Awake()
        {
            m_buttonTemplate.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (!m_buttons.IsNullOrEmpty())
            {
                foreach (var button in m_buttons)
                {
                    Destroy(button.gameObject);
                }
            }
            m_buttons.Clear();
            var currentSettingsFound = false;
            for (int i = 0; i < Localization.NumberOfLanguages; i++)
            {
                var newButton = Instantiate(m_buttonTemplate, m_buttonHolder);
                newButton.gameObject.SetActive(true);
                newButton.SetUpButtonValue(i);
                m_buttons.Add(newButton);
                if (i == Localization.CurrentLanguageIndex)
                {
                    currentSettingsFound = true;
                    newButton.Select();
                }
            }
            if (!currentSettingsFound)
            {
                m_buttons[0].Select();
            }
        }
    }
}
