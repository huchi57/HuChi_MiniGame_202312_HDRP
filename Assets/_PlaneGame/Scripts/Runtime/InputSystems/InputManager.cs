using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace UrbanFox.MiniGame
{
    public class InputManager : RuntimeManager<InputManager>
    {
        public static event Action<InputControl> OnAnyKeyPressed;
        public static event Action<InputControl> OnAnyKeyButReservedKeysPressed;
        public static event Action<Vector2> OnMove;
        public static Vector2 Move { get; private set; }
        public static readonly InputActionKey Escape = new InputActionKey();
        public static readonly InputActionKey Back = new InputActionKey();
        public static readonly InputActionKey Submit = new InputActionKey();

        [SerializeField, Required]
        private InputActionAsset m_inputActions;

        [SerializeField]
        private bool m_hideCursorOnStart;

        private InputAction m_moveAction;

        public InputActionAsset InputActions => m_inputActions;

        private void Start()
        {
            if (m_hideCursorOnStart)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            InputSystem.onAnyButtonPress.Call((control) =>
            {
                OnAnyKeyPressed?.Invoke(control);
                if (!control.name.ToLower().Contains("esc") && !control.name.ToLower().Contains("back") && !control.name.ToLower().Contains("f6"))
                {
                    OnAnyKeyButReservedKeysPressed?.Invoke(control);
                }
            });
            if (!SettingsManager.Instance.InputBindingsOverride.IsNullOrEmpty())
            {
                m_inputActions.LoadBindingOverridesFromJson(SettingsManager.Instance.InputBindingsOverride);
            }
            m_moveAction = m_inputActions.FindAction(nameof(Move));
            Escape.BindAction(m_inputActions.FindAction(nameof(Escape)));
            Back.BindAction(m_inputActions.FindAction(nameof(Back)));
            Submit.BindAction(m_inputActions.FindAction(nameof(Submit)));
        }

        private void Update()
        {
            Move = m_moveAction.ReadValue<Vector2>();
            OnMove?.Invoke(Move);
            Escape.Update();
            Back.Update();
            Submit.Update();
        }
    }
}
