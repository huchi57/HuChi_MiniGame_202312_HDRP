using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace UrbanFox.MiniGame
{
    public class GameCompletedTrigger : MonoBehaviour
    {
        [Serializable]
        public struct CreditSlide
        {
            [Required]
            public CanvasGroup Slide;

            [Min(0)]
            public float FadeTime;

            [Min(0)]
            public float DisplayDuration;

            [Min(0)]
            public float IntervalUntillNextSlide;
        }

        [SerializeField]
        private float m_waitForSeconds;

        [SerializeField]
        private CreditSlide[] m_creditsSlides;

        private void Awake()
        {
            if (!m_creditsSlides.IsNullOrEmpty())
            {
                foreach (var slide in m_creditsSlides)
                {
                    slide.Slide.gameObject.SetActive(false);
                    slide.Slide.alpha = 0;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<PlayerController>(out var player) && GameInstance.CurrentGameState == GameState.GameplayPausable)
            {
                player.LockToPitchAngleForDuration(pitchAngle: 0, duration: 5, lerpSpeed: 3);
                StartCoroutine(PlayCredits());
            }
        }

        private IEnumerator PlayCredits()
        {
            yield return new WaitForSeconds(m_waitForSeconds);
            GameInstance.SwitchGameState(GameState.Loading);
            if (!m_creditsSlides.IsNullOrEmpty())
            {
                foreach (var slide in m_creditsSlides)
                {
                    slide.Slide.gameObject.SetActive(true);
                    slide.Slide.alpha = 0;
                    slide.Slide.DOFade(1, slide.FadeTime);
                    yield return new WaitForSeconds(slide.FadeTime + slide.DisplayDuration);
                    slide.Slide.DOFade(0, slide.FadeTime).OnComplete(() =>
                    {
                        slide.Slide.gameObject.SetActive(false);
                    });
                    yield return new WaitForSeconds(slide.FadeTime + slide.IntervalUntillNextSlide);
                }
            }
            GameInstance.SwitchGameState(GameState.GameCompletedWaitForInput);
            UIManager.Instance.EnableSplashScreen(true);
            InputManager.OnAnyKeyPressed += OnAnykeyPressed_OneOff;
        }

        private void OnAnykeyPressed_OneOff(UnityEngine.InputSystem.InputControl obj)
        {
            InputManager.OnAnyKeyPressed -= OnAnykeyPressed_OneOff;
            if (GameInstance.PlayerController)
            {
                GameInstance.PlayerController.UpdateRespawnPoint(Vector3.zero);
            }
            GameManager.OnFadeOutCompleted += ResetCameraCheckpoint_OneOff;
            GameManager.Instance.RestartCheckpoint_Fade();
        }

        private void ResetCameraCheckpoint_OneOff()
        {
            GameManager.OnFadeOutCompleted -= ResetCameraCheckpoint_OneOff;
            if (CameraBrain.Main)
            {
                CameraBrain.Main.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                CameraBrain.Main.SaveCameraCheckpointPosition();
            }
        }
    }
}
