using UnityEngine;
using DG.Tweening;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIFadeInOnEnable : MonoBehaviour
    {
        [SerializeField, NonEditable]
        private CanvasGroup m_canvasGroup;

        [SerializeField]
        private bool m_enableEffect = true;

        [SerializeField, Indent, EnableIf(nameof(m_enableEffect), true), Min(0)]
        private float m_fadeDuration = 1;

        [SerializeField, Indent, EnableIf(nameof(m_enableEffect), true)]
        private Ease m_easeFunction = Ease.InOutCubic;

        private void OnValidate()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            if (m_enableEffect)
            {
                m_canvasGroup.alpha = 0;
                m_canvasGroup.DOFade(1, m_fadeDuration).SetEase(m_easeFunction).SetUpdate(true);
            }
        }
    }
}
