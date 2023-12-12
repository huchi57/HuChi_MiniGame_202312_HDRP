using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputDeviceManager : RuntimeManager<InputDeviceManager>
    {
        public static event Action<ControlScheme> OnControlSchemeChanged
        {
            add
            {
                m_onControlSchemeChanged += value;
                m_onControlSchemeChanged?.Invoke(Instance.m_currentControlScheme);
            }
            remove => m_onControlSchemeChanged -= value;
        }

        private static event Action<ControlScheme> m_onControlSchemeChanged;

        [SerializeField, NonEditable]
        private PlayerInput m_playerInput;

        private ControlScheme m_currentControlScheme;
        private ControlScheme m_cacheControlScheme;

        private void OnValidate()
        {
            m_playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            UpdateCurrentControlScheme();
            m_onControlSchemeChanged?.Invoke(m_currentControlScheme);
        }

        private void Update()
        {
            UpdateCurrentControlScheme();
            if (m_cacheControlScheme != m_currentControlScheme)
            {
                m_cacheControlScheme = m_currentControlScheme;
                m_onControlSchemeChanged?.Invoke(m_currentControlScheme);
            }
        }

        private void UpdateCurrentControlScheme()
        {
            if (m_playerInput.devices.Count > 0)
            {
                var currentDevice = m_playerInput.devices[0];
                if (currentDevice is SwitchProControllerHID)
                {
                    m_currentControlScheme = ControlScheme.Switch;
                }
                else if (currentDevice is XInputController)
                {
                    m_currentControlScheme = ControlScheme.XInput;
                }
                else if (currentDevice is DualSenseGamepadHID)
                {
                    m_currentControlScheme = ControlScheme.DualSense;
                }
                else if (currentDevice is DualShockGamepad)
                {
                    m_currentControlScheme = ControlScheme.DualShock;
                }
                else
                {
                    m_currentControlScheme = ControlScheme.KeyboardAndMouse;
                }
            }
            else
            {
                m_currentControlScheme = ControlScheme.KeyboardAndMouse;
            }
        }
    }
}
