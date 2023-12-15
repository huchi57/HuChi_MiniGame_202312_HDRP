using System.Text;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class DebugManager : RuntimeManager<DebugManager>
    {
        private static readonly Rect k_debugRect = new Rect(100, 100, 1000, 1000);

        [SerializeField] private bool m_enableDebug;
        [SerializeField] private KeyCode m_debugKeyCode = KeyCode.F6;

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
                // TODO: Add more texts
            }
            return content.ToString();
        }
    }
}
