using UnityEngine;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Text))]
    public class UIBuildDescriptionText : MonoBehaviour
    {
        [SerializeField, NonEditable] private Text m_text;

        private void Start()
        {
            m_text = GetComponent<Text>();
            if (ApplicationBuildData.Instance)
            {
                m_text.text = ApplicationBuildData.Instance.GetBuildInfoText();
            }
        }
    }
}
