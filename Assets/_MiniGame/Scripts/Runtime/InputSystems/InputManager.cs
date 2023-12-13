using UnityEngine;
using UnityEngine.InputSystem;

namespace UrbanFox.MiniGame
{
    public class InputManager : RuntimeManager<InputManager>
    {
        public static Vector2 Move { get; private set; }
        public static readonly InputActionKey Escape = new InputActionKey();
        public static readonly InputActionKey Back = new InputActionKey();
        public static readonly InputActionKey Submit = new InputActionKey();

        [SerializeField, Required]
        private InputActionAsset m_inputActions;

        [SerializeField]
        private bool m_hideCursorOnStart;

        private InputAction m_moveAction;

        private void Start()
        {
            if (m_hideCursorOnStart)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            m_moveAction = m_inputActions.FindAction(nameof(Move));
            Escape.BindAction(m_inputActions.FindAction(nameof(Escape)));
            Back.BindAction(m_inputActions.FindAction(nameof(Back)));
            Submit.BindAction(m_inputActions.FindAction(nameof(Submit)));
        }

        private void Update()
        {
            Move = m_moveAction.ReadValue<Vector2>();
            Escape.Update();
            Back.Update();
            Submit.Update();
        }
    }
}
