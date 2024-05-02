using System;
using UnityEngine;

namespace UrbanFox
{
    [Serializable]
    public class IntParameter
    {
        [SerializeField]
        private ValueSource m_valueSource;

        [SerializeField]
        private int m_value;

        [SerializeField]
        private ScriptableIntParameter m_parameterAsset;

        public int Value => m_valueSource == ValueSource.Direct ? m_value : m_parameterAsset.Value;
    }
}
