using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class GameOverTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GameManager.PlayerTag))
            {
                // Game Over
            }
        }
    }
}
