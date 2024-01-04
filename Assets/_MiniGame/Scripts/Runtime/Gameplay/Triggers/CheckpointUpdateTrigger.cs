using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class CheckpointUpdateTrigger : MonoBehaviour
    {
        [SerializeField]
        private CheckpointIndicator m_checkpointIndicator;

        [SerializeField]
        private bool m_overrideSpawnPoint;

        [SerializeField, ShowIf(nameof(m_overrideSpawnPoint), true)]
        private Transform m_spawnPointOverride;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out var player) && GameManager.Instance.CurrentGameState == GameState.GameplayPausable)
            {
                player.UpdateRespawnPoint(m_overrideSpawnPoint && m_spawnPointOverride ? m_spawnPointOverride.position : transform.position);
                if (CameraBrain.Main)
                {
                    CameraBrain.Main.SaveCameraCheckpointPosition();
                }
                if (m_checkpointIndicator)
                {
                    m_checkpointIndicator.TurnOnLight();
                }
            }
        }
    }
}
