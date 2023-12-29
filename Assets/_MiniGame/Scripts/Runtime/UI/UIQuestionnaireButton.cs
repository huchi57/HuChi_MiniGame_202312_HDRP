using UnityEngine;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Button))]
    public class UIQuestionnaireButton : MonoBehaviour
    {
        [SerializeField, NonEditable]
        private Button m_button;

        [SerializeField]
        private string m_questionnaireURL;

        [SerializeField]
        private string m_gameBuildKey;

        private void OnValidate()
        {
            m_button = GetComponent<Button>();
        }

        private void Awake()
        {
            m_button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDestroy()
        {
            m_button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            if (m_questionnaireURL.Contains(m_gameBuildKey) && ApplicationBuildData.Instance)
            {
                Application.OpenURL(m_questionnaireURL.Replace(m_gameBuildKey, ApplicationBuildData.Instance.GetBuildInfoText()));
            }
        }
    }
}
