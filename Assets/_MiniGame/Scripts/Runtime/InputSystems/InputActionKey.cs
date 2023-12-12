using System;
using UnityEngine.InputSystem;

namespace UrbanFox.MiniGame
{
    public class InputActionKey
    {
        public Action OnKeyDown;
        public Action OnKeyUp;

        private InputAction m_action;

        public void BindAction(InputAction action)
        {
            m_action = action;
        }

        public void Update()
        {
            if (m_action.WasPressedThisFrame())
            {
                OnKeyDown?.Invoke();
            }
            if (m_action.WasReleasedThisFrame())
            {
                OnKeyUp?.Invoke();
            }
        }
    }
}
