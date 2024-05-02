using System;
using UnityEngine;

namespace UrbanFox
{
    [Serializable]
    public class ColorParameter
    {
        [SerializeField]
        private ValueSource m_valueSource;

        [SerializeField]
        private Color m_value;

        [SerializeField]
        private ScriptableColorParameter m_parameterAsset;

        public Color Value => m_valueSource == ValueSource.Direct ? m_value : m_parameterAsset.Value;
    }
}
