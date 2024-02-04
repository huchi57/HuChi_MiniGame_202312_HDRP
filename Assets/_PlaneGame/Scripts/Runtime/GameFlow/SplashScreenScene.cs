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

        private IEnumerator Start()
        {
            GameInstance.SwitchGameState(GameState.Loading);
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
                        page.CanvasGroup.DOFade(1, 0.25f);
                        for (float t = 0; t < page.Duration + 1; t += Time.unscaledDeltaTime)
                        {
                            if (Input.anyKeyDown)
                            {
                                break;
                            }
                            yield return null;
                        }
                        page.CanvasGroup.DOFade(0, 0.25f);
                        yield return new WaitForSeconds(1);
                    }
                    yield return new WaitForSeconds(m_idleDurationBetweenPages);
                }
            }
            GameManager.Instance.UnloadAndReloadScenes(new string[] { gameObject.scene.name }, new string[] { m_sceneToLoadWhenSplashScreenEnds }, 0.1f, 2, enableDefaultCallbacks: false, () =>
            {
                GameInstance.SwitchGameState(GameState.WaitForInputToStartGame);
            });
        }
    }
}
