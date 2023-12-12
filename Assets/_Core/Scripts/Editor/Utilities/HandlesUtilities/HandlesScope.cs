using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace UrbanFox.Editor
{
    public class HandlesScope : IDisposable
    {
        private Matrix4x4 m_cachedMatrix;
        private Color m_cachedColor;
        private CompareFunction m_cachedZTest;

        public HandlesScope(Vector3 position, Quaternion rotation, Vector3 scale, Color color, CompareFunction zTest)
        {
            Initialize(Matrix4x4.TRS(position, rotation, scale), color, zTest);
        }

        public HandlesScope(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            Initialize(Matrix4x4.TRS(position, rotation, scale), color, Handles.zTest);
        }

        public HandlesScope(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Initialize(Matrix4x4.TRS(position, rotation, scale), Handles.color, Handles.zTest);
        }

        public HandlesScope(Color color, CompareFunction zTest)
        {
            Initialize(Handles.matrix, color, zTest);
        }

        public HandlesScope(Color color)
        {
            Initialize(Handles.matrix, color, Handles.zTest);
        }

        public void Dispose()
        {
            Handles.matrix = m_cachedMatrix;
            Handles.color = m_cachedColor;
            Handles.zTest = m_cachedZTest;
        }

        private void Initialize(Matrix4x4 matrix, Color color, CompareFunction zTest)
        {
            m_cachedMatrix = Handles.matrix;
            m_cachedColor = Handles.color;
            m_cachedZTest = Handles.zTest;
            Handles.matrix = matrix;
            Handles.color = color;
            Handles.zTest = zTest;
        }
    }
}
