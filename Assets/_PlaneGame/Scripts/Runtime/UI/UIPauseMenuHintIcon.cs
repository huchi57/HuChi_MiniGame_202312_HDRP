using UnityEngine;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPauseMenuHintIcon : MonoBehaviour
    {
        [SerializeField, NonEditable]
        private CanvasGroup m_canvasGroup;

        [SerializeField]
        private GameState[] m_visibleGameStates;

        private float m_targetAlpha;

        private void OnValidate()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            if (m_visibleGameStates.IsNullOrEmpty())
            {
                gameObject.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            m_targetAlpha = 0;
            foreach (var state in m_visibleGameStates)
            {
                if (GameInstance.CurrentGameState == state)
                {
                    m_targetAlpha = 1;
                    break;
                }
            }
            m_canvasGroup.alpha = Mathf.Lerp(m_canvasGroup.alpha, m_targetAlpha, 5 * Time.unscaledDeltaTime);
        }
    }
}
