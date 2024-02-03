using System;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class ConveyorBelt : MonoBehaviour
    {
        [Serializable]
        public enum TextureAxis
        {
            PositiveX,
            NegativeX,
            PositiveY,
            NegativeY
        }

        [Serializable]
        public enum MaterialType
        {
            UseMainMaterial,
            UseSubMaterial
        }

        [SerializeField, NonEditable]
        private Rigidbody m_rigidBody;

        [SerializeField]
        private Vector3 m_moveSpeed;

        [Header("Texture Tiling (Optional)")]
        [SerializeField]
        private Renderer m_renderer;

        [SerializeField]
        private TextureAxis m_textureMovingDirection;

        [SerializeField]
        private float m_textureSpeedMultiplier = 1;

        [SerializeField]
        private MaterialType m_materialType;

        [SerializeField, ShowIf(nameof(m_materialType), MaterialType.UseSubMaterial), Info("The material index to instantiate on the target renderer.")]
        private uint m_materialIndex;

        private Material m_materialInstance;

        public Vector3 MoveSpeed => m_moveSpeed;

        private void OnValidate()
        {
            m_rigidBody = GetComponent<Rigidbody>();
            m_rigidBody.isKinematic = true;
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

        private void FixedUpdate()
        {
            var cachePosition = m_rigidBody.position;
            m_rigidBody.position += Time.fixedDeltaTime * m_moveSpeed;
            m_rigidBody.MovePosition(cachePosition);
        }

        private void LateUpdate()
        {
            if (m_materialInstance)
            {
                m_materialInstance.mainTextureOffset += m_textureSpeedMultiplier * Time.deltaTime * m_moveSpeed.magnitude * GetDirection(m_textureMovingDirection);
            }
        }

        private Vector2 GetDirection(TextureAxis axis)
        {
            return axis switch
            {
                TextureAxis.PositiveX => Vector2.right,
                TextureAxis.NegativeX => Vector2.left,
                TextureAxis.PositiveY => Vector2.up,
                TextureAxis.NegativeY => Vector2.down,
                _ => Vector2.zero
            };
        }
    }
}
