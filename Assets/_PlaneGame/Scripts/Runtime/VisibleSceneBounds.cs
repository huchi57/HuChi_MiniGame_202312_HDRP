using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class VisibleSceneBounds : MonoBehaviour
    {
        private const string k_mainCamera = "MainCamera";

        [SerializeField, Scene]
        private string[] m_dirtyScenes;

        [SerializeField, Scene]
        private string[] m_backgroundScenes;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(k_mainCamera))
            {
                NewGameManager.Instance.AddDirtyScenes(m_dirtyScenes);
                FoxySceneManager.LoadScenes(m_dirtyScenes);
                FoxySceneManager.LoadScenes(m_backgroundScenes);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(k_mainCamera))
            {
                NewGameManager.Instance.RemoveDirtyScenes(m_dirtyScenes);
                FoxySceneManager.UnloadScenes(m_dirtyScenes);
                FoxySceneManager.UnloadScenes(m_backgroundScenes);
            }
        }
    }
}
