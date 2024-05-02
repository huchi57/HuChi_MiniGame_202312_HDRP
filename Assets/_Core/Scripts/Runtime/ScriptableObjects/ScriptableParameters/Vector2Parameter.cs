using System;
using UnityEngine;

namespace UrbanFox
{
    [Serializable]
    public class Vector2Parameter
    {
        [SerializeField]
        private ValueSource m_valueSource;

        [SerializeField]
        private Vector2 m_value;

        [SerializeField]
        private ScriptableVector2Parameter m_parameterAsset;

        public Vector2 Value => m_valueSource == ValueSource.Direct ? m_value : m_parameterAsset.Value;
    }
}
