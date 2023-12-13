using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UrbanFox.MiniGame
{
    public class SettingsKeyRebindButton : MonoBehaviour
    {
        [SerializeField, LocalizedString]
        private string m_actionName;

        [SerializeField]
        private InputAction m_action;
    }
}
