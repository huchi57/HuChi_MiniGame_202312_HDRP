using UnityEngine;

namespace UrbanFox
{
    public class ScriptableIntParameter : ScriptableParameter<int>
    {
        [SerializeField]
        private int m_value;

        public override int Value => m_value;
    }
}
