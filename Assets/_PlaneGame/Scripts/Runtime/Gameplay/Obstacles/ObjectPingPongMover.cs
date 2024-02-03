using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class ObjectPingPongMover : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_endPointDeltaPosition;

        [SerializeField]
        private float m_targetPositionMoveVelocity = 1;

        [SerializeField]
        private float m_positionLerpRate = 1;

        [SerializeField]
        private float m_restTimeWhenReachesEndPoint = 1;

        private bool m_isAtEndAndResting;
        private bool m_isGoingFromStartToEnd;
        private Vector3 m_targetPosition;
        private Vector3 m_startPoint;
        private Vector3 m_endPoint;
        private Vector3 m_directionNormalized;
        private float m_restingTimer;

        private void Awake()
        {
            m_isAtEndAndResting = false;
            m_isGoingFromStartToEnd = true;
            m_targetPosition = transform.position;
            m_startPoint = transform.position;
            m_endPoint = transform.position + m_endPointDeltaPosition;
            m_directionNormalized = (m_endPoint - m_startPoint).normalized;
        }

        private void Update()
        {
            if (m_isAtEndAndResting)
            {
                m_restingTimer += Time.deltaTime;
                if (m_restingTimer > m_restTimeWhenReachesEndPoint)
                {
                    m_restingTimer = 0;
                    m_isAtEndAndResting = false;
                }
            }
            else
            {
                if (m_isGoingFromStartToEnd)
                {
                    m_targetPosition += m_targetPositionMoveVelocity * Time.deltaTime * m_directionNormalized;
                    if (Vector3.Dot(m_targetPosition - m_endPoint, m_directionNormalized) > 0)
                    {
                        m_isAtEndAndResting = true;
                        m_targetPosition = m_endPoint;
                        m_isGoingFromStartToEnd = !m_isGoingFromStartToEnd;
                    }
                }
                else
                {
                    m_targetPosition -= m_targetPositionMoveVelocity * Time.deltaTime * m_directionNormalized;
                    if (Vector3.Dot(m_targetPosition - m_startPoint, m_directionNormalized) < 0)
                    {
                        m_isAtEndAndResting = true;
                        m_targetPosition = m_startPoint;
                        m_isGoingFromStartToEnd = !m_isGoingFromStartToEnd;
                    }
                }
            }
            transform.position = Vector3.Lerp(transform.position, m_targetPosition, m_positionLerpRate * Time.deltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                Awake();
            }
            GizmosExtensions.DrawLine(m_startPoint, m_endPoint, Color.white);
            GizmosExtensions.DrawSolidSphere(m_startPoint, 0.25f, Color.white);
            GizmosExtensions.DrawSolidSphere(m_endPoint, 0.25f, Color.white);
        }
    }
}
