using System;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Light))]
    public class LightSynchedEmissionMaterial : MonoBehaviour
    {
        [Serializable]
        public enum MaterialType
        {
            UseMainMaterial,
            UseSubMaterial
        }

        [SerializeField, NonEditable]
        private Light m_light;

        [SerializeField, Required]
        private Renderer m_renderer;

        [SerializeField]
        private MaterialType m_materialType;

        [SerializeField, ShowIf(nameof(m_materialType), MaterialType.UseSubMaterial), Info("The material index to instantiate on the target renderer.")]
        private uint m_materialIndex;

        [SerializeField]
        private string m_emissionParameterName = "_EmissiveColor";

        [SerializeField]
        private float m_emissionMultiplier = 1;

        private Material m_materialInstance;

        private void OnValidate()
        {
            m_light = GetComponent<Light>();
        }

        private void Start()
        {
            if (m_renderer)
            {
                switch (m_materialType)
                {
                    case MaterialType.UseMainMaterial:
                        m_materialInstance = m_renderer.material;
                        m_renderer.material = m_materialInstance;
                        break;
                    case MaterialType.UseSubMaterial:
                        m_materialInstance = m_renderer.materials[m_materialIndex];
                        m_renderer.materials[m_materialIndex] = m_materialInstance;
                        break;
                    default:
                        return;
                }
            }
        }

        private void LateUpdate()
        {
            var lightColor = m_light.color.linear * m_light.intensity;
            if (m_light.useColorTemperature)
            {
                lightColor *= Mathf.CorrelatedColorTemperatureToRGB(m_light.colorTemperature);
            }
            m_materialInstance.SetColor(m_emissionParameterName, m_emissionMultiplier * lightColor);
        }
    }
}
