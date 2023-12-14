using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class CameraContributorTrigger : MonoBehaviour
    {
        [SerializeField] private CameraContributorBase m_cameraContributor;

        private void OnTriggerEnter(Collider other)
        {
            if (CameraBrain.Main && other.CompareTag(GameManager.PlayerTag))
            {
                CameraBrain.Main.AddContributor(m_cameraContributor);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (CameraBrain.Main && other.CompareTag(GameManager.PlayerTag))
            {
                CameraBrain.Main.RemoveContributor(m_cameraContributor);
            }
        }
    }
}
