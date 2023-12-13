using System;
using UnityEngine.InputSystem;

namespace UrbanFox.MiniGame
{
    public class InputActionKey
    {
        public Action OnKeyDown;
        public Action OnKeyUp;

        public bool WasPressedThisFrame => m_action.WasPressedThisFrame();
        public bool WasReleasedThisFrame => m_action.WasReleasedThisFrame();

        private InputAction m_action;

        public void BindAction(InputAction action)
        {
            m_action = action;
        }

        public void Update()
        {
            if (WasPressedThisFrame)
            {
                OnKeyDown?.Invoke();
            }
            if (WasReleasedThisFrame)
            {
                OnKeyUp?.Invoke();
            }
        }
    }
}
