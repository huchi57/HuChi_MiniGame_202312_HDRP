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
            if (other.CompareTag(NewGameManager.PlayerTag))
            {
                NewGameManager.Instance.AddDirtyScenes(m_dirtyScenes);
                FoxySceneManager.LoadScenes(m_dirtyScenes);
                FoxySceneManager.LoadScenes(m_immutableScenes);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(NewGameManager.PlayerTag))
            {
                NewGameManager.Instance.RemoveDirtyScenes(m_dirtyScenes);
                FoxySceneManager.UnloadScenes(m_dirtyScenes);
                FoxySceneManager.UnloadScenes(m_immutableScenes);
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
