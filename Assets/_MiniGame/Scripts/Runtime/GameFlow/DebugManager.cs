using System.Text;
using UnityEngine;
using Cinemachine;

namespace UrbanFox.MiniGame
{
    public class DebugManager : RuntimeManager<DebugManager>
    {
        private static readonly Rect k_debugRect = new Rect(100, 100, 1000, 1000);

        [SerializeField] private bool m_enableDebug;
        [SerializeField] private KeyCode m_debugKeyCode = KeyCode.F6;

        [Header("Components")]
        [SerializeField, Required] private CinemachineVirtualCamera m_cinemachineVirtualCamera;

        private void Update()
        {
            if (Input.GetKeyDown(m_debugKeyCode))
            {
                m_enableDebug = !m_enableDebug;
            }
        }

        private void OnGUI()
        {
            if (m_enableDebug && Application.isPlaying)
            {
                GUI.Label(k_debugRect, GetDebugContent());
            }
        }

        private string GetDebugContent()
        {
            var content = new StringBuilder();
            if (GameManager.IsInstanceExist)
            {
                content.AppendLine($"Game State: {GameManager.Instance.CurrentGameState}");
                content.AppendLine($"FOV: {m_cinemachineVirtualCamera.m_Lens.FieldOfView:F2}");
                // TODO: Add more texts
            }
            return content.ToString();
        }
    }
}
