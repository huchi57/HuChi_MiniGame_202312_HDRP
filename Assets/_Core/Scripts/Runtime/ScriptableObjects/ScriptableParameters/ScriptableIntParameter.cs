using UnityEngine;

namespace UrbanFox
{
    public class ScriptableFloatParameter : ScriptableParameter<float>
    {
        [SerializeField]
        private float m_value;

        public override float Value => m_value;
    }
}
