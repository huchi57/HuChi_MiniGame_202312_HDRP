using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

namespace UrbanFox.MiniGame
{
    public class DebugManager : RuntimeManager<DebugManager>
    {
        private static readonly Rect k_debugRect = new Rect(100, 100, 1000, 1000);

        [Header("Components")]
        [SerializeField, Required] private CinemachineVirtualCamera m_cinemachineVirtualCamera;
        [SerializeField] private GameObject m_debugGraph;

        [Header("Display Options")]
        [SerializeField] private bool m_enableDebug;

        [Space]

        [SerializeField] private int m_warningFPS = 60;
        [SerializeField] private Color m_warningFPSColor = Color.red;

        [SerializeField] private int m_criticalFPS = 30;
        [SerializeField] private Color m_criticalFPSColor = Color.red;

        private void Start()
        {
            if (m_debugGraph)
            {
                m_debugGraph.SetActive(m_enableDebug);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                m_enableDebug = !m_enableDebug;
                if (m_debugGraph)
                {
                    m_debugGraph.SetActive(m_enableDebug);
                }
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
            if (GameManager.IsInstanceExist && ApplicationBuildData.Instance)
            {
                content.AppendLine($"Press F6 to toggle display ON/OFF.");
                content.AppendLine($"{ApplicationBuildData.Instance.GetBuildInfoText()}");
                content.AppendLine($"Game State: {GameManager.Instance.CurrentGameState}");
                content.AppendLine($"FOV: {m_cinemachineVirtualCamera.m_Lens.FieldOfView:F2}");

                var fps = 1 / Time.unscaledDeltaTime;
                content.AppendLine($"FPS: {(int)fps} ({Time.unscaledDeltaTime:F2}ms)".Color(fps < m_criticalFPS ? m_criticalFPSColor : fps < m_warningFPS ? m_warningFPSColor : Color.white));
                // TODO: Add more texts
            }
            return content.ToString();
        }
    }
}
