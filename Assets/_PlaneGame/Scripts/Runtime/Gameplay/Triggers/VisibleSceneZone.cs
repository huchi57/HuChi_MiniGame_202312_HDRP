using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class VisibleSceneZone : MonoBehaviour
    {
        private const string k_mainCamera = "MainCamera";

        [SerializeField, Scene]
        private string[] m_activeScenes;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(k_mainCamera))
            {
                FoxySceneManager.LoadScenes(m_activeScenes);

                // HACK: Required so a scene reload can reset camera contributor tirggers
                GameManager.Instance.AddDirtyScenes(m_activeScenes);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(k_mainCamera))
            {
                FoxySceneManager.UnloadScenes(m_activeScenes);
                GameManager.Instance.RemoveDirtyScenes(m_activeScenes);
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
