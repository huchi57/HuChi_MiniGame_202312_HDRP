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
                NewGameManager.Instance.RestartCheckpoint_Instant();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.GetComponent<PlayerController>())
            {
                NewGameManager.Instance.RestartCheckpoint_Instant();
            }
        }
    }
}
