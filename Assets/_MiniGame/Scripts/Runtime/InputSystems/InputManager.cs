using UnityEngine;
using UnityEngine.InputSystem;

namespace UrbanFox.MiniGame
{
    public class InputManager : RuntimeManager<InputManager>
    {
        public static Vector2 Move { get; private set; }
        public static readonly InputActionKey Escape = new InputActionKey();
        public static readonly InputActionKey Back = new InputActionKey();

        [SerializeField, Required]
        private InputActionAsset m_inputActions;

        private InputAction m_moveAction;

        private void Start()
        {
            m_moveAction = m_inputActions.FindAction(nameof(Move));
            Escape.BindAction(m_inputActions.FindAction(nameof(Escape)));
            Back.BindAction(m_inputActions.FindAction(nameof(Back)));
        }

        private void Update()
        {
            Move = m_moveAction.ReadValue<Vector2>();
            Escape.Update();
            Back.Update();
        }
    }
}
