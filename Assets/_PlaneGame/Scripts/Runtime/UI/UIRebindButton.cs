using UnityEngine;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    public class UIRebindButton : MonoBehaviour
    {
        [SerializeField, Required]
        private Text m_actionText;

        [SerializeField, Required]
        private Text m_rebindScreenActionText;

        public void UpdateUI()
        {
            m_rebindScreenActionText.text = m_actionText.text;
        }
    }
}
