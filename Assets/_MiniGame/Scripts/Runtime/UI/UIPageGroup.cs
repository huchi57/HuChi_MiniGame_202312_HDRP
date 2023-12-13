using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class UIPageGroup : MonoBehaviour
    {
        [SerializeField, Required]
        private UIPage m_defaultFirstPage;

        [SerializeField]
        private float m_pageFadeInTime;

        [SerializeField]
        private float m_pageFadeOutTime;

        [SerializeField]
        private bool m_savePageHistoryWhenGroupClosed = false;

        [Header("Background")]

        [SerializeField, Required]
        private CanvasGroup m_background;

        [SerializeField]
        private float m_backgroundFadeSpeed;

        [Space]

        [SerializeField, Info("Debug only - Set to false in builds.")]
        private bool m_autoOpenOnAwake;

        private readonly Stack<UIPage> m_pageHistory = new Stack<UIPage>();

        private float m_targetBackgroundAlpha = 0;

        public void OpenPageGroup(Action onCompleted = null)
        {
            gameObject.SetActive(true);
            if (m_pageHistory.TryPeek(out var lastOpenedPage))
            {
                lastOpenedPage.OpenPage(m_pageFadeInTime, onCompleted);
                m_targetBackgroundAlpha = lastOpenedPage.ShowsBackground ? 1 : 0;
            }
            else
            {
                m_defaultFirstPage.OpenPage(m_pageFadeInTime, onCompleted);
                m_targetBackgroundAlpha = m_defaultFirstPage.ShowsBackground ? 1 : 0;
                m_pageHistory.Push(m_defaultFirstPage);
            }
        }

        public void ClosePageGroup(Action onCompleted = null)
        {
            m_targetBackgroundAlpha = 0;
            m_background.alpha = 0;
            if (m_pageHistory.TryPeek(out var currentPage))
            {
                currentPage.ClosePage(m_pageFadeOutTime, () =>
                {
                    onCompleted?.Invoke();
                    gameObject.SetActive(false);
                });
            }
            else
            {
                onCompleted?.Invoke();
                gameObject.SetActive(false);
            }
            if (!m_savePageHistoryWhenGroupClosed)
            {
                m_pageHistory.Clear();
            }
        }

        public void GotoPage(UIPage page)
        {
            if (!page)
            {
                return;
            }
            if (m_pageHistory.TryPeek(out var currentPage))
            {
                currentPage.ClosePage(m_pageFadeOutTime, () =>
                {
                    page.OpenPage(m_pageFadeInTime);
                    m_targetBackgroundAlpha = page.ShowsBackground ? 1 : 0;
                });
            }
            else
            {
                page.OpenPage(m_pageFadeInTime);
                m_targetBackgroundAlpha = page.ShowsBackground ? 1 : 0;
            }
            m_pageHistory.Push(page);
        }

        public void TryGotoPreviousPage()
        {
            if (m_pageHistory.TryPop(out var currentPage))
            {
                currentPage.ClosePage(m_pageFadeOutTime, () =>
                {
                    if (m_pageHistory.TryPeek(out var previousPage))
                    {
                        previousPage.OpenPage(m_pageFadeInTime);
                        m_targetBackgroundAlpha = previousPage.ShowsBackground ? 1 : 0;
                    }
                    else
                    {
                        ClosePageGroup();
                    }
                });
            }
        }

        private void Awake()
        {
            if (m_autoOpenOnAwake)
            {
                OpenPageGroup();
            }
        }

        private void OnEnable()
        {
            StartCoroutine(DelayDoOnEnable());
            IEnumerator DelayDoOnEnable()
            {
                yield return null;
                InputManager.Back.OnKeyDown += TryGotoPreviousPage;
            }
        }

        private void OnDisable()
        {
            InputManager.Back.OnKeyDown -= TryGotoPreviousPage;
        }

        private void LateUpdate()
        {
            m_background.alpha = Mathf.Lerp(m_background.alpha, m_targetBackgroundAlpha, m_backgroundFadeSpeed * Time.unscaledDeltaTime);
        }
    }
}
