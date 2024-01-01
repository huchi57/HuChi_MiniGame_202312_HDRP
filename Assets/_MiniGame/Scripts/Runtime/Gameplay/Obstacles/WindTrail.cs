using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class WindTrail : MonoBehaviour
    {
        private const string k_color = "_TintColor";

        [SerializeField, NonEditable]
        private TrailRenderer m_trail;

        [SerializeField]
        private float m_colorLerpSpeed;

        [SerializeField]
        private float m_pathMaxHeightDelta = 1;

        [Header("One of these curves will be randomly picked.")]
        [SerializeField]
        private AnimationCurve[] m_lifespanToNormalizedHeightCurves;

        private Vector3 m_basePosition;
        private AnimationCurve m_currentHeightCurve;

        private Vector3 m_speedPerSecond;
        private float m_lifetime;
        private float m_timer;
        private float m_targetAlpha = 1;

        private float m_prewarmTime;

        public void Initialize(Vector3 spawnPosition, Vector3 speedPerSecond, float lifetime)
        {
            transform.position = spawnPosition;
            m_basePosition = transform.position;
            m_speedPerSecond = speedPerSecond;
            m_lifetime = lifetime;
            m_prewarmTime = Random.Range(0f, m_lifetime);
        }

        private void OnValidate()
        {
            m_trail = GetComponent<TrailRenderer>();
        }

        private void Awake()
        {
            var trailColor = m_trail.material.GetColor(k_color);
            m_trail.material.SetColor(k_color, new Color(trailColor.r, trailColor.g, trailColor.b, 0));
            m_currentHeightCurve = m_lifespanToNormalizedHeightCurves.SelectRandom();
        }

        private void LateUpdate()
        {
            if (m_prewarmTime > 0)
            {
                m_prewarmTime -= Time.deltaTime;
                return;
            }

            var trailColor = m_trail.material.GetColor(k_color);
            m_timer += Time.deltaTime;
            if (m_timer > m_lifetime)
            {
                if (trailColor.a < 0.01f)
                {
                    Destroy(gameObject);
                }
                else
                {
                    m_targetAlpha = 0;
                }
            }
            else
            {
                m_targetAlpha = 1;
            }
            m_trail.material.SetColor(k_color, Color.Lerp(trailColor, new Color(trailColor.r, trailColor.g, trailColor.b, m_targetAlpha), m_colorLerpSpeed * Time.deltaTime));

            m_basePosition += m_speedPerSecond * Time.deltaTime;
            transform.position = m_basePosition + m_currentHeightCurve.Evaluate(m_timer / m_lifetime) * m_pathMaxHeightDelta * Vector3.up;
        }
    }
}
