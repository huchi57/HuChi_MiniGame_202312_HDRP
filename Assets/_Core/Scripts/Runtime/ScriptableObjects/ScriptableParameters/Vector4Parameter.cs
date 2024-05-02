using System;
using UnityEngine;

namespace UrbanFox
{
    [Serializable]
    public class Vector4Parameter
    {
        [SerializeField]
        private ValueSource m_valueSource;

        [SerializeField]
        private Vector4 m_value;

        [SerializeField]
        private ScriptableVector4Parameter m_parameterAsset;

        public Vector4 Value => m_valueSource == ValueSource.Direct ? m_value : m_parameterAsset.Value;
    }
}
