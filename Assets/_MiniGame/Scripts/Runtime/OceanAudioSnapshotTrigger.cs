using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

namespace UrbanFox.MiniGame
{
    public class OceanAudioSnapshotTrigger : MonoBehaviour
    {
        [SerializeField]
        private EventReference m_snapshot;

        [SerializeField]
        private string m_parameterName = "Intensity";

        [SerializeField]
        private float m_lerpSpeed = 1;

        private EventInstance m_instance;
        private int m_step = 0;

        private void Awake()
        {
            m_instance = RuntimeManager.CreateInstance(m_snapshot);
            m_step = 0;
        }

        private void OnDisable()
        {
            if (m_instance.isValid())
            {
                m_instance.getPlaybackState(out var state);
                if (state == PLAYBACK_STATE.PLAYING)
                {
                    m_instance.StopImmediately();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (m_step == 0 && other.TryGetComponent<PlayerController>(out var player) && player.IsAlive)
            {
                m_step = 1;
                m_instance.setParameterByName(m_parameterName, 100);
                m_instance.start();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out var player) && player.IsAlive)
            {
                m_step = 2;
            }
        }

        private void LateUpdate()
        {
            if (m_step == 2)
            {
                m_instance.getParameterByName(m_parameterName, out var currentValue);
                m_instance.setParameterByName(m_parameterName, Mathf.Lerp(currentValue, 0, m_lerpSpeed * Time.deltaTime)); ;
            }
        }
    }
}
