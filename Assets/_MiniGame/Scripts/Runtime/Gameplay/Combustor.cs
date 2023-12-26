using System.Collections;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace UrbanFox.MiniGame
{
    public class Combustor : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_closedPositionRelativePosition;

        [SerializeField]
        private bool m_startCombustingOnAwake;

        [SerializeField]
        private bool m_randomOffsetTime;

        [SerializeField, EnableIf(nameof(m_randomOffsetTime), false)]
        private float m_offsetTime;

        [SerializeField]
        private float m_impactDuracion;

        [SerializeField]
        private float m_combustorClosedHoldDuration;

        [SerializeField]
        private float m_retractDuration;

        [SerializeField]
        private float m_combustorOpenedHoldDuration;

        [SerializeField]
        private CinemachineImpulseSource m_cinemachineImpulseSource;

        private bool m_isCombusting;
        private Vector3 m_openedPosition;
        private Vector3 m_closedPosition;
        private TweenerCore<Vector3, Vector3, VectorOptions> m_tween;

        public void StartCombusting()
        {
            m_isCombusting = true;
        }

        private void Awake()
        {
            if (m_startCombustingOnAwake)
            {
                m_isCombusting = true;
            }
        }

        private IEnumerator Start()
        {
            m_openedPosition = transform.position;
            m_closedPosition = transform.position + m_closedPositionRelativePosition;

            while (!m_isCombusting)
            {
                yield return null;
            }

            yield return new WaitForSeconds(m_randomOffsetTime ? Random.Range(0, m_combustorClosedHoldDuration + m_combustorOpenedHoldDuration) : m_offsetTime);

            while (true)
            {
                m_tween = transform.DOMove(m_closedPosition, m_impactDuracion);
                yield return new WaitForSeconds(m_impactDuracion);
                if (m_cinemachineImpulseSource)
                {
                    m_cinemachineImpulseSource.GenerateImpulse();
                }
                yield return new WaitForSeconds(m_combustorClosedHoldDuration);
                m_tween = transform.DOMove(m_openedPosition, m_retractDuration);
                yield return new WaitForSeconds(m_retractDuration);
                yield return new WaitForSeconds(m_combustorOpenedHoldDuration);
            }
        }

        private void OnDestroy()
        {
            if (m_tween != null && m_tween.IsActive())
            {
                m_tween.Kill();
            }
        }

        private void OnDrawGizmosSelected()
        {
            GizmosExtensions.DrawWireSphere(transform.position, 0.15f, Color.white);
            GizmosExtensions.DrawWireSphere(transform.position + m_closedPositionRelativePosition, 0.15f, Color.white);
            GizmosExtensions.DrawLine(transform.position, transform.position + m_closedPositionRelativePosition, Color.white);
        }
    }
}
