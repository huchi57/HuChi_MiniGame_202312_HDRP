using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class CheckpointIndicator : MonoBehaviour
    {
        [SerializeField]
        private LightSynchedEmissionMaterial m_lightMaterial;

        [SerializeField, NonEditable]
        private Light m_light;

        private Color m_cacheLightColor;

        public void TurnOnLight()
        {
            m_light.color = m_cacheLightColor;
        }

        private void OnValidate()
        {
            if (m_lightMaterial)
            {
                m_light = m_lightMaterial.GetComponent<Light>();
            }
        }

        private void Start()
        {
            if (m_light)
            {
                m_cacheLightColor = m_light.color;
                if (Vector3.Distance(GameManager.Player.position, transform.position) < 10f)
                {
                    TurnOnLight();
                }
                else
                {
                    m_light.color = Color.black;
                }
            }
        }
    }
}
