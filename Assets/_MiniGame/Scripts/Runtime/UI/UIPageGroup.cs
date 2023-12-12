using System;
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

        [Space]

        [SerializeField, Info("Debug only - Set to false in builds.")]
        private bool m_autoOpenOnAwake;

        private readonly Stack<UIPage> m_pageHistory = new Stack<UIPage>();

        public void OpenPageGroup(Action onCompleted = null)
        {
            gameObject.SetActive(true);
            if (m_pageHistory.TryPeek(out var lastOpenedPage))
            {
                lastOpenedPage.OpenPage(m_pageFadeInTime, onCompleted);
            }
            else
            {
                m_defaultFirstPage.OpenPage(m_pageFadeInTime, onCompleted);
                m_pageHistory.Push(m_defaultFirstPage);
            }
        }

        public void ClosePageGroup(Action onCompleted = null)
        {
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
                });
            }
            else
            {
                page.OpenPage(m_pageFadeInTime);
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
            InputManager.Back.OnKeyDown += TryGotoPreviousPage;
        }

        private void OnDisable()
        {
            InputManager.Back.OnKeyDown -= TryGotoPreviousPage;
        }
    }
}
