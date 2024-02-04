using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace UrbanFox.MiniGame
{
    public class SplashScreenScene : MonoBehaviour
    {
        [Serializable]
        public struct Page
        {
            [Required]
            public CanvasGroup CanvasGroup;

            public float Duration;
        }

        [SerializeField]
        private Page[] m_pages;

        [SerializeField]
        private float m_idleDurationBetweenPages;

        [SerializeField, Scene]
        private string m_sceneToLoadWhenSplashScreenEnds;

        private bool m_forceGameLoadingState;

        private IEnumerator Start()
        {
            m_forceGameLoadingState = true;
            if (!m_pages.IsNullOrEmpty())
            {
                foreach (var page in m_pages)
                {
                    if (page.CanvasGroup)
                    {
                        page.CanvasGroup.alpha = 0;
                    }
                }
                foreach (var page in m_pages)
                {
                    if (page.CanvasGroup)
                    {
                        page.CanvasGroup.DOFade(1, 1);
                        for (float t = 0; t < page.Duration + 1; t += Time.unscaledDeltaTime)
                        {
                            if (Input.anyKeyDown)
                            {
                                break;
                            }
                            yield return null;
                        }
                        page.CanvasGroup.DOFade(0, 1);
                        yield return new WaitForSeconds(1);
                    }
                    yield return new WaitForSeconds(m_idleDurationBetweenPages);
                }
            }
            AudioManager.Instance.FadeOutGameBus(1);
            UIManager.Instance.FadeOutToBlack(1, () =>
            {
                m_forceGameLoadingState = false;
                FoxySceneManager.UnloadScene(gameObject.scene.name, () =>
                {
                    FoxySceneManager.LoadScene(m_sceneToLoadWhenSplashScreenEnds, () =>
                    {
                        NewGameManager.Instance.RestartCheckpoint(0, 0.1f, 0.1f, 2, 1, 2);
                    });
                });
            });
        }

        private void Update()
        {
            if (m_forceGameLoadingState)
            {
                GameStateManager.SwitchGameState(GameState.Loading);
            }
        }
    }
}
