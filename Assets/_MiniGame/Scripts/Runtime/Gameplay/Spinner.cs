using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class Spinner : MonoBehaviour
    {
        [SerializeField] private bool m_preWarm;
        [SerializeField] private float m_targetSpinningSpeed;
        [SerializeField] private float m_acceleration;
        [SerializeField] private Vector3 m_spinningAxis;

        private float m_currentSpinningSpeed;

        public float TargetSpinningSpeed
        {
            get => m_currentSpinningSpeed;
            set => m_currentSpinningSpeed = value;
        }

        public float Acceleration
        {
            get => m_acceleration;
            set => m_acceleration = value;
        }

        private void Start()
        {
            m_currentSpinningSpeed = m_preWarm ? m_targetSpinningSpeed : 0;
        }

        private void Update()
        {
            m_currentSpinningSpeed = Mathf.Lerp(m_currentSpinningSpeed, m_targetSpinningSpeed, m_acceleration * Time.deltaTime);
            transform.localEulerAngles += m_currentSpinningSpeed * Time.deltaTime * m_spinningAxis;
        }
    }
}
