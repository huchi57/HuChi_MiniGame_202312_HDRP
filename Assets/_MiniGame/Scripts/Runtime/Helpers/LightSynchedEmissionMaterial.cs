using UnityEngine;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Light))]
    public class LightSynchedEmissionMaterial : MonoBehaviour
    {
        [SerializeField, NonEditable]
        private Light m_light;

        [SerializeField, Required]
        private Renderer m_renderer;

        [SerializeField]
        private string m_emissionParameterName = "_EmissiveColor";

        private Material m_materialInstance;

        private void OnValidate()
        {
            m_light = GetComponent<Light>();
        }

        private void Start()
        {
            m_materialInstance = m_renderer.material;
            m_renderer.material = m_materialInstance;
        }

        private void LateUpdate()
        {
            var lightColor = m_light.color.linear * m_light.intensity;
            if (m_light.useColorTemperature)
            {
                lightColor *= Mathf.CorrelatedColorTemperatureToRGB(m_light.colorTemperature);
            }
            m_materialInstance.SetColor(m_emissionParameterName, lightColor);
        }
    }
}
