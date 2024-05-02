using UnityEngine;

namespace UrbanFox
{
    public class ScriptableVector2Parameter : ScriptableParameter<Vector2>
    {
        [SerializeField]
        private Vector2 m_value;

        public override Vector2 Value => m_value;
    }
}
