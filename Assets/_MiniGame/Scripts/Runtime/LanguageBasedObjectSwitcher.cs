using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class LanguageBasedObjectSwitcher : MonoBehaviour
    {
        [SerializeField]
        private uint[] m_enableOnLanguageIndexes;

        private void Awake()
        {
            Localization.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnDestroy()
        {
            Localization.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged()
        {
            if (m_enableOnLanguageIndexes.IsNullOrEmpty())
            {
                gameObject.SetActive(true);
                return;
            }
            foreach (var index in m_enableOnLanguageIndexes)
            {
                if (Localization.CurrentLanguageIndex == index)
                {
                    gameObject.SetActive(true);
                    return;
                }
            }
            gameObject.SetActive(false);
        }
    }
}
