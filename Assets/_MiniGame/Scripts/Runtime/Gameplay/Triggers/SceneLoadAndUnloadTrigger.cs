using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class SceneLoadAndUnloadTrigger : MonoBehaviour
    {
        [SerializeField] private bool m_shouldFirstSceneToLoadBeActive = true;
        [SerializeField, Scene] private string[] m_scenesToLoad;
        [SerializeField, Scene] private string[] m_scenesToUnload;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GameManager.PlayerTag))
            {
                GameManager.Instance.LoadScenesInBackground(m_scenesToLoad, m_shouldFirstSceneToLoadBeActive);
                GameManager.Instance.UnloadScenes(m_scenesToUnload);
            }
        }
    }
}
