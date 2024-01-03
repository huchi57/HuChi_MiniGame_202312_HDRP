using UnityEngine;

namespace UrbanFox
{
    public class RuntimeManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_instance;
        private static bool m_isApplicationQuitting = false;

        [SerializeField] private bool m_dontDestroyOnLoad;

        public static bool IsInstanceExist => !m_isApplicationQuitting && m_instance != null;
        public static T Instance => GetInstance();

        public static T GetInstance()
        {
            if (m_isApplicationQuitting)
            {
                FoxyLogger.LogWarning($"An instance of {typeof(T)} will not be returned because the application is quitting.");
                return null;
            }
            if (m_instance == null)
            {
                m_instance = FindFirstObjectByType<T>();
                if (m_instance == null)
                {
                    m_instance = new GameObject($"[{typeof(T).Name}]").AddComponent<T>();
                    FoxyLogger.Log($"An instance of {typeof(T)} has been automatically created because an instance could not be found.");
                }
                if (m_instance is RuntimeManager<T> instance && instance.m_dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(m_instance.transform.root);
                }
            }
            return m_instance;
        }

        public virtual void Awake()
        {
            GetInstance();
        }

        public virtual void OnApplicationQuit()
        {
            m_isApplicationQuitting = false;
        }
    }
}
