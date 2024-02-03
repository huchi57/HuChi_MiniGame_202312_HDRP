using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPage : MonoBehaviour
    {
        [SerializeField, NonEditable]
        private CanvasGroup m_canvasGroup;

        [SerializeField]
        private GameObject m_optionalAutoSelectWhenPageOpen;

        [SerializeField]
        private bool m_rememberLastSelectedObjectWhenPageClose;

        [Header("Background")]

        [SerializeField]
        private bool m_showsBackground = true;

        private GameObject m_lastSelectedObject;

        public bool ShowsBackground => m_showsBackground;

        private void OnValidate()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OpenPage(float fadeDuration, Action onFadeComplete = null)
        {
            gameObject.SetActive(true);
            m_canvasGroup.alpha = 0;
            m_canvasGroup.DOFade(1, fadeDuration).SetUpdate(true).OnComplete(() =>
            {
                onFadeComplete?.Invoke();
            });
            if (EventSystemManager.IsInstanceExist)
            {
                if (m_rememberLastSelectedObjectWhenPageClose && m_lastSelectedObject)
                {
                    EventSystemManager.Instance.SelectGameObject(m_lastSelectedObject);
                }
                else if (m_optionalAutoSelectWhenPageOpen)
                {
                    EventSystemManager.Instance.SelectGameObject(m_optionalAutoSelectWhenPageOpen);
                }
            }
        }

        public void ClosePage(float fadeDuration, Action onFadeComplete = null)
        {
            if (m_rememberLastSelectedObjectWhenPageClose && EventSystem.current && EventSystem.current.currentSelectedGameObject)
            {
                m_lastSelectedObject = EventSystem.current.currentSelectedGameObject;
            }
            m_canvasGroup.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
            {
                onFadeComplete?.Invoke();
                gameObject.SetActive(false);
            });
        }
    }
}
