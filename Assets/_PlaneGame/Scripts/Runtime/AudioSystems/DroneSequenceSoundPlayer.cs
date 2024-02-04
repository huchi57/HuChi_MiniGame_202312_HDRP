using System;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

namespace UrbanFox.MiniGame
{
    public class DroneSequenceSoundPlayer : MonoBehaviour
    {
        [Serializable]
        public struct Stage
        {
            [SerializeField]
            public EventReference BaseSound;

            [SerializeField]
            public EventReference Cadenza;

            public EventInstance BaseSoundInstance;
        }

        [SerializeField]
        private Stage[] m_stages;

        [SerializeField]
        private EventReference m_gameOverSound;

        private bool m_isPlayingCadenza;
        private int m_currentStage;

        public void IncrementStageByOne()
        {
            if (!m_isPlayingCadenza)
            {
                m_currentStage++;
                if (m_currentStage.IsInRange(m_stages))
                {
                    m_stages[m_currentStage].BaseSoundInstance = m_stages[m_currentStage].BaseSound.Play();
                }
            }
        }

        public void PlayGameOverCadenza()
        {
            if (!m_isPlayingCadenza)
            {
                m_isPlayingCadenza = true;
                for (int i = 0; i < m_currentStage + 1; i++)
                {
                    if (i.IsInRange(m_stages))
                    {
                        if (m_stages[i].BaseSoundInstance.isValid())
                        {
                            m_stages[i].BaseSoundInstance.getPlaybackState(out var state);
                            if (state == PLAYBACK_STATE.PLAYING)
                            {
                                m_stages[i].BaseSoundInstance.StopByFadeOut();
                            }
                        }
                        m_stages[i].Cadenza.PlayOneShot();
                    }
                }
                m_gameOverSound.PlayOneShot();
            }
        }

        private void Start()
        {
            NewGameManager.OnFadeOutCompleted += ResetSequenceState;
            ResetSequenceState();
        }

        private void OnDestroy()
        {
            NewGameManager.OnFadeOutCompleted -= ResetSequenceState;
        }

        private void ResetSequenceState()
        {
            m_isPlayingCadenza = false;
            m_currentStage = -1;
            foreach (var stage in m_stages)
            {
                if (stage.BaseSoundInstance.isValid())
                {
                    stage.BaseSoundInstance.StopByFadeOut();
                    stage.BaseSoundInstance.getPlaybackState(out var state);
                    if (state == PLAYBACK_STATE.PLAYING)
                    {
                        stage.BaseSoundInstance.StopByFadeOut();
                    }
                }
            }
        }
    }
}
