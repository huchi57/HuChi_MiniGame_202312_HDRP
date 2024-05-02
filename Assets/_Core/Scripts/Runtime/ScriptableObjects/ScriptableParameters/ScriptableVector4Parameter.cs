using UnityEngine;

namespace UrbanFox
{
    public class ScriptableVector4Parameter : ScriptableParameter<Vector4>
    {
        [SerializeField]
        private Vector4 m_value;

        public override Vector4 Value => m_value;
    }
}
