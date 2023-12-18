using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class NetInteractionSimulation : MonoBehaviour
    {
        private bool m_hitCloth = false;

        private Rigidbody m_rigidBody;
        private Vector3 m_lastFramePosition;
        private Vector3 m_deltaPosition;

        private Vector3 m_cacheSpeed;

        private void Awake()
        {
            m_rigidBody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.TryGetComponent<Cloth>(out _))
            {
                m_hitCloth = true;
                m_cacheSpeed = m_deltaPosition / Time.fixedDeltaTime;
                //m_rigidBody.useGravity = false;
            }
        }

        private void FixedUpdate()
        {
            if (!m_hitCloth)
            {
                m_deltaPosition = m_rigidBody.position - m_lastFramePosition;
                m_lastFramePosition = m_rigidBody.position;
                return;
            }

            m_cacheSpeed = Vector3.Lerp(m_cacheSpeed, Vector3.zero, 1 * Time.fixedDeltaTime);
            m_rigidBody.position += (m_cacheSpeed * Time.fixedDeltaTime);
        }
    }
}
