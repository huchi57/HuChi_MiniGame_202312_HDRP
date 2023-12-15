using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class UIPauseMenu : MonoBehaviour
    {
        [SerializeField, Required]
        private UIPageGroup m_pageGroup;

        private void Awake()
        {
            InputManager.Escape.OnKeyDown += OnEscapePressed;
        }

        private void OnDestroy()
        {
            InputManager.Escape.OnKeyDown -= OnEscapePressed;
        }

        private void OnEscapePressed()
        {
            switch (GameManager.Instance.CurrentGameState)
            {
                case GameState.Loading:
                    break;
                case GameState.WaitForInputToStartGame:
                    break;
                case GameState.GameplayPausable:
                    if (!m_pageGroup.gameObject.activeSelf)
                    {
                        m_pageGroup.OpenPageGroup();
                    }
                    break;
                case GameState.GameplayNonPausable:
                    break;
                case GameState.GameOverWaitForReload:
                    break;
                case GameState.Paused:
                    break;
                default:
                    break;
            }
        }
    }
}
