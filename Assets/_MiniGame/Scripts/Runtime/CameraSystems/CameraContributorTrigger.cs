using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class CameraContributorTrigger : MonoBehaviour
    {
        [SerializeField] private CameraContributorBase m_cameraContributor;

        private void OnValidate()
        {
            if (!m_cameraContributor)
            {
                TryGetComponent(out m_cameraContributor);
            }
        }

        private void OnDestroy()
        {
            if (CameraBrain.Main)
            {
                CameraBrain.Main.RemoveContributor(m_cameraContributor);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (CameraBrain.Main)
            {
                if (other.TryGetComponent<PlayerController>(out var player) && player.IsAlive)
                {
                    CameraBrain.Main.AddContributor(m_cameraContributor);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (CameraBrain.Main && other.GetComponent<PlayerController>())
            {
                CameraBrain.Main.RemoveContributor(m_cameraContributor);
            }
        }
    }
}
