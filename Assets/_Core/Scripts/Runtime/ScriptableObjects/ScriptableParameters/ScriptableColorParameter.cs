using UnityEngine;

namespace UrbanFox
{
    public class ScriptableColorParameter : ScriptableParameter<Color>
    {
        [SerializeField]
        private Color m_value;

        public override Color Value => m_value;
    }
}
