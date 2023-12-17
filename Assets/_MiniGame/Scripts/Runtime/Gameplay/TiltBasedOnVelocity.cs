using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class TiltBasedOnVelocity : MonoBehaviour
    {
        [SerializeField] private AnimationCurve m_tiltAngleBasedOnVelocity;
        [SerializeField] private float m_rotationSlerpSpeed = 5;

        private Vector3 m_lastFramePosition;
        private Vector3 m_currentSpeed;

        private void Awake()
        {
            m_lastFramePosition = transform.position;
        }

        private void Update()
        {
            m_currentSpeed = (transform.position - m_lastFramePosition) / Time.deltaTime;
            m_lastFramePosition = transform.position;
            var axisToTilt = Vector3.Cross(m_currentSpeed, Vector3.up).normalized;
            var targetRotation = Quaternion.AngleAxis(-m_tiltAngleBasedOnVelocity.Evaluate(m_currentSpeed.magnitude), axisToTilt);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_rotationSlerpSpeed * Time.deltaTime);
        }
    }
}
