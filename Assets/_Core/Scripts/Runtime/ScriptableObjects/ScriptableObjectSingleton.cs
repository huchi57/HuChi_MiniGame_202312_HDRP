using UnityEngine;

namespace UrbanFox
{
    public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T m_instance;

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    var candidates = Resources.LoadAll<T>(string.Empty);
                    if (!candidates.IsNullOrEmpty())
                    {
                        if (candidates.Length > 1)
                        {
                            FoxyLogger.Log($"2 or more {typeof(T).Name} instances found. Only the first found instance will be referenced. Consider removing other files of the same type.");
                        }
                        m_instance = candidates[0];
                        if (m_instance != null)
                        {
                            return m_instance;
                        }
                    }

                    m_instance = CreateInstance<T>();
                    FoxyLogger.Log($"A new instance of {typeof(T).Name} has been created in runtime. A runtime-generated instance will only be saved as an asset in Editor.");
#if UNITY_EDITOR
                    var newAssetPath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath($"Assets/Resources/{typeof(T).Name}.asset");
                    UnityEditor.AssetDatabase.CreateAsset(m_instance, newAssetPath);
                    FoxyLogger.Log($"A new instance of {typeof(T).Name} has been saved to the asset at {newAssetPath}.");
#endif
                }
                return m_instance;
            }
        }
    }
}
