using UnityEngine;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class InstantGameOverOnCollision : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerController>())
            {
                GameManager.Instance.RestartCheckpoint_Instant_Default();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.GetComponent<PlayerController>())
            {
                GameManager.Instance.RestartCheckpoint_Instant_Default();
            }
        }
    }
}
