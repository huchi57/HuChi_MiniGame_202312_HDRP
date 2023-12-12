using System;
using UnityEngine;

namespace UrbanFox
{
    public class GizmosScope : IDisposable
    {
        private Matrix4x4 m_cachedMatrix;
        private Color m_cachedColor;

        public GizmosScope(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            Initialize(Matrix4x4.TRS(position, rotation, scale), color);
        }

        public GizmosScope(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Initialize(Matrix4x4.TRS(position, rotation, scale), Gizmos.color);
        }

        public GizmosScope(Color color)
        {
            Initialize(Gizmos.matrix, color);
        }

        public void Dispose()
        {
            Gizmos.matrix = m_cachedMatrix;
            Gizmos.color = m_cachedColor;
        }

        private void Initialize(Matrix4x4 matrix, Color color)
        {
            m_cachedMatrix = Gizmos.matrix;
            m_cachedColor = Gizmos.color;
            Gizmos.matrix = matrix;
            Gizmos.color = color;
        }
    }
}
