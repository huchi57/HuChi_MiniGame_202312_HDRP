using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    public class SettingsResolutionSelector : MonoBehaviour
    {
        [SerializeField, Required]
        private SettingsResolutionButton m_buttonTemplate;

        [SerializeField, Required]
        private Transform m_buttonHolder;

        [SerializeField, Required]
        private Text m_resolutionText;

        private readonly List<SettingsResolutionButton> m_instantiatedButtons = new List<SettingsResolutionButton>();

        private void Awake()
        {
            m_buttonTemplate.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (!m_instantiatedButtons.IsNullOrEmpty())
            {
                foreach (var button in m_instantiatedButtons)
                {
                    Destroy(button.gameObject);
                }
                m_instantiatedButtons.Clear();
            }
            var resolutions = Screen.resolutions;
            var currentSettingsFound = false;
            for(int i = 0; i < resolutions.Length; i++)
            {
                var newButton = Instantiate(m_buttonTemplate, m_buttonHolder);
                newButton.gameObject.SetActive(true);
                newButton.SetUpButtonValue(m_resolutionText, resolutions[i].width, resolutions[i].height);
                m_instantiatedButtons.Add(newButton);
                if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                {
                    currentSettingsFound = true;
                    newButton.Select();
                }
            }
            if (!currentSettingsFound)
            {
                m_instantiatedButtons[m_instantiatedButtons.Count - 1].Select();
            }
        }
    }
}
