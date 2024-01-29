using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class SceneLoadingTrigger : MonoBehaviour
    {
        [SerializeField, Scene, Info("Dirty scenes will be reloaded automatically when game restarts from checkpoints.")]
        private string[] m_dirtyScenes;

        [SerializeField, Scene, Info("Immutable scenes will only be unloaded when player leaves this trigger.")]
        private string[] m_immutableScenes;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GameManager.PlayerTag))
            {
                GameManager.Instance.AddDirtyScenes(m_dirtyScenes);
                GameManager.Instance.LoadScenesInBackground(m_dirtyScenes, true);
                GameManager.Instance.LoadScenesInBackground(m_immutableScenes, false);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(GameManager.PlayerTag))
            {
                GameManager.Instance.RemoveDirtyScenes(m_dirtyScenes);
                GameManager.Instance.UnloadScenes(m_dirtyScenes);
                GameManager.Instance.UnloadScenes(m_immutableScenes);
            }
        }

        private void OnValidate()
        {
            if (TryGetComponent<Collider>(out var collider))
            {
                collider.isTrigger = true;
            }
        }
    }
}
