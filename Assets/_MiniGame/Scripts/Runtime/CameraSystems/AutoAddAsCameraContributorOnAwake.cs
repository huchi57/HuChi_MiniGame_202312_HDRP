using UnityEngine;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(CameraContributorBase))]
    public class AutoAddAsCameraContributorOnAwake : MonoBehaviour
    {
        [SerializeField, NonEditable]
        private CameraContributorBase m_cameraContributor;

        [SerializeField]
        private bool m_autoAddAsContributorOnStart = true;

        private void OnValidate()
        {
            m_cameraContributor = GetComponent<CameraContributorBase>();
        }

        private void Start()
        {
            if (m_autoAddAsContributorOnStart && CameraBrain.Main)
            {
                CameraBrain.Main.AddContributor(m_cameraContributor);
            }
        }
    }
}
