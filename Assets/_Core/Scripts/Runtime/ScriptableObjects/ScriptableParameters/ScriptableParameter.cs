using UnityEngine;

namespace UrbanFox
{
    public abstract class ScriptableParameter<T> : ScriptableObject
    {
        public abstract T Value { get; }
    }
}
