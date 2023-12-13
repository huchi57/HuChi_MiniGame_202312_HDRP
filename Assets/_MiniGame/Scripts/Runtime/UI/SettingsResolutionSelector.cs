using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    public class SettingsResolutionSelector : MonoBehaviour
    {
        [SerializeField, Required] private SettingsResolutionButton m_buttonTemplate;
        [SerializeField, Required] private Transform m_buttonHolder;
        [SerializeField, Required] private Text m_resolutionText;

        private readonly List<GameObject> m_instantiatedButtons = new List<GameObject>();

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
                    Destroy(button);
                }
                m_instantiatedButtons.Clear();
            }
            var resolutions = Screen.resolutions;
            for(int i = 0; i < resolutions.Length; i++)
            {
                var newButton = Instantiate(m_buttonTemplate, m_buttonHolder);
                newButton.gameObject.SetActive(true);
                newButton.SetUpButtonValue(m_resolutionText, resolutions[i].width, resolutions[i].height);
                m_instantiatedButtons.Add(newButton.gameObject);
                if (i == resolutions.Length - 1)
                {
                    newButton.Select();
                }
            }
        }
    }
}
