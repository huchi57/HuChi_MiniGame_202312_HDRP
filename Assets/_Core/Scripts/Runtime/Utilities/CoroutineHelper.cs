using System.Collections;
using UnityEngine;

namespace UrbanFox
{
    public static class CoroutineHelper
    {
        private class Container : MonoBehaviour { }

        private static Container m_container;

        private static Container CoroutineContainer
        {
            get
            {
                if (m_container == null)
                {
                    m_container = new GameObject("[Coroutine Helper Container]").AddComponent<Container>();
                    Object.DontDestroyOnLoad(m_container);
                }
                return m_container;
            }
        }

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            return CoroutineContainer.StartCoroutine(routine);
        }

        public static Coroutine StartCoroutine(string methodName)
        {
            return CoroutineContainer.StartCoroutine(methodName);
        }

        public static Coroutine StartCoroutine(string methodName, object value)
        {
            return CoroutineContainer.StartCoroutine(methodName, value);
        }

        public static void StopCoroutine(IEnumerator routine)
        {
            CoroutineContainer.StopCoroutine(routine);
        }

        public static void StopCoroutine(Coroutine routine)
        {
            CoroutineContainer.StopCoroutine(routine);
        }

        public static void StopCoroutine(string methodName)
        {
            CoroutineContainer.StopCoroutine(methodName);
        }
        
        public static void StopAllCoroutines()
        {
            CoroutineContainer.StopAllCoroutines();
        }
    }
}
