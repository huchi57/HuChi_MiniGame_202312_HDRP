using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class WindTrigger : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_windSpeed;

        [SerializeField]
        private WindTrailController m_windTrailController;

        private void OnValidate()
        {
            if (m_windTrailController)
            {
                m_windTrailController.SetParameters(m_windSpeed);
            }
        }

        private void Update()
        {
            if (m_windTrailController)
            {
                m_windTrailController.SetParameters(m_windSpeed);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out var player))
            {
                player.TriggerGameOverByEnteringWindTrigger(m_windSpeed);
            }
        }
    }
}
