using UnityEngine;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Light))]
    public class DroneLight : MonoBehaviour
    {
        [SerializeField, NonEditable]
        private Light m_light;

        [SerializeField]
        private bool m_isOffByDefault;

        [Space]

        [SerializeField]
        private float m_trackSlerpSpeed;

        [Space]

        [SerializeField]
        private Color m_alertedColor = Color.red;

        [SerializeField]
        private float m_colorLerpSpeed;

        [Space]

        [SerializeField]
        private float m_alertedLightAngle;

        [SerializeField]
        private float m_angleLerpSpeed;

        [Space]

        [SerializeField]
        private float m_alertedIntensity = 1300;

        [SerializeField]
        private float m_intensityLerpSpeed;

        private Transform m_trackingTarget;
        private bool m_isTracking;

        public void StartTracking(Transform target)
        {
            m_trackingTarget = target;
            m_isTracking = true;
            m_light.enabled = true;
        }

        public void TurnOffLights()
        {
            m_light.enabled = false;
        }

        private void OnValidate()
        {
            m_light = GetComponent<Light>();
        }

        private void Awake()
        {
            if (m_isOffByDefault)
            {
                m_light.enabled = false;
            }
        }

        private void Update()
        {
            if (m_isTracking && m_trackingTarget)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_trackingTarget.position - transform.position), m_trackSlerpSpeed * Time.deltaTime);
                m_light.color = Color.Lerp(m_light.color, m_alertedColor, m_colorLerpSpeed * Time.deltaTime);
                m_light.spotAngle = Mathf.Lerp(m_light.spotAngle, m_alertedLightAngle, m_angleLerpSpeed * Time.deltaTime);
                m_light.intensity = Mathf.Lerp(m_light.intensity, m_alertedIntensity, m_intensityLerpSpeed * Time.deltaTime);
            }
        }
    }
}
