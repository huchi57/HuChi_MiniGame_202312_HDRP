using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    public class SettingsKeyRebindingPanel : MonoBehaviour
    {
        [SerializeField, LocalizedString]
        private string m_actionToRebindPrefixText;

        [SerializeField, Required]
        private Text m_actionToRebindText;

        public void UpdateActionToRebindText(string actionName)
        {
            if (m_actionToRebindPrefixText.TryGetLocalization(out var value))
            {
                m_actionToRebindText.text = $"{value} {actionName}";
            }
            else
            {
                m_actionToRebindText.text = $"!!{m_actionToRebindPrefixText} {actionName}";
            }
        }

        public void StartRebindingAction(InputAction action)
        {

        }
    }
}
