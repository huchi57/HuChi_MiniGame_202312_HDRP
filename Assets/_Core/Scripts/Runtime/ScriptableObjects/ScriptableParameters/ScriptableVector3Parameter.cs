using UnityEngine;

namespace UrbanFox
{
    public class ScriptableVector3Parameter : ScriptableParameter<Vector3>
    {
        [SerializeField]
        private Vector3 m_value;

        public override Vector3 Value => m_value;
    }
}
