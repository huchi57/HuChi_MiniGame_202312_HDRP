using System.Collections.Generic;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class WindTrailController : MonoBehaviour
    {
        [SerializeField]
        private WindTrail m_trailTemplate;

        [SerializeField]
        private AnimationCurve m_windVelocityToTrailNumberCurve;

        [SerializeField]
        private float m_generationZoneMaxDistance;

        [SerializeField]
        private float m_generationZoneRadius;

        [SerializeField]
        private AnimationCurve m_windVelocityToTrailLifetimeCurve;

        [SerializeField, Range(0, 1)]
        private float m_trailLifetimeMinMultiplier = 1;

        [SerializeField]
        private float m_physicsSpeedToScreenSpeedMultiplier = 1;

        [Header("Controlled by Parent Wind Zone")]
        [SerializeField, NonEditable]
        private Vector3 m_windSpeed;

        private readonly List<WindTrail> m_trails = new List<WindTrail>();

        public void SetParameters(Vector3 windSpeed)
        {
            m_windSpeed = windSpeed;
        }

        private void OnValidate()
        {
            if (m_trailTemplate)
            {
                m_trailTemplate.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            var targetTrailNumber = m_windVelocityToTrailNumberCurve.Evaluate(m_windSpeed.magnitude);
            if (m_trails.Count < targetTrailNumber)
            {
                var newTrail = Instantiate(m_trailTemplate, transform);
                newTrail.gameObject.SetActive(true);
                var spawnPosition = transform.position + m_generationZoneMaxDistance * Random.Range(0f, 1f) * m_windSpeed.normalized;
                spawnPosition += Random.Range(0, m_generationZoneRadius) * m_windSpeed.GetRandomPerpendicularVector().normalized;
                var lifeTime = m_windVelocityToTrailLifetimeCurve.Evaluate(m_windSpeed.magnitude) * Random.Range(m_trailLifetimeMinMultiplier, 1);
                newTrail.Initialize(spawnPosition, m_physicsSpeedToScreenSpeedMultiplier * m_windSpeed, lifeTime);
                m_trails.Add(newTrail);
            }

            if (!m_trails.IsNullOrEmpty())
            {
                for (int i = m_trails.Count - 1; i > 0; i--)
                {
                    if (m_trails[i] == null)
                    {
                        m_trails.RemoveAt(i);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            GizmosExtensions.DrawSphere(transform.position, 0.5f, Color.white);
            if (m_windSpeed.magnitude > 0)
            {
                GizmosExtensions.DrawLine(transform.position, transform.position + m_generationZoneMaxDistance * m_windSpeed.normalized, Color.white);
                GizmosExtensions.DrawWireDisc(transform.position, m_windSpeed, m_generationZoneRadius, Color.white);
                GizmosExtensions.DrawWireDisc(transform.position + m_generationZoneMaxDistance * m_windSpeed.normalized, m_windSpeed, m_generationZoneRadius, Color.white);
            }
        }
    }
}
