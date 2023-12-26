using UnityEngine;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class TrainObject : MonoBehaviour
    {
        [SerializeField, NonEditable]
        private Rigidbody m_rigidBody;

        [SerializeField]
        private Vector3 m_speed;

        public void MoveTrain()
        {
            m_rigidBody.AddForce(m_speed, ForceMode.VelocityChange);
        }

        private void OnValidate()
        {
            m_rigidBody = GetComponent<Rigidbody>();
            m_rigidBody.useGravity = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerController>())
            {
                GameManager.Instance.GameOverAndRestartCheckpoint_Instant();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.GetComponent<PlayerController>())
            {
                GameManager.Instance.GameOverAndRestartCheckpoint_Instant();
            }
        }
    }
}
