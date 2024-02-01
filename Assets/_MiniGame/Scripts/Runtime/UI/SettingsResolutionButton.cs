using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Button))]
    public class SettingsResolutionButton : MonoBehaviour, IPointerEnterHandler, ISelectHandler, ISubmitHandler
    {
        [SerializeField, NonEditable]
        private Button m_button;

        [SerializeField, Required]
        private UIPageGroup m_pageGroup;

        private Text m_resolutionText;
        private int m_width;
        private int m_height;

        public void SetUpButtonValue(Text resolutionText, int width, int height)
        {
            m_resolutionText = resolutionText;
            m_width = width;
            m_height = height;
            m_button.onClick.AddListener(Select);
        }

        public void Select()
        {
            if (EventSystemManager.IsInstanceExist)
            {
                m_resolutionText.text = $"{m_width} x {m_height}";
                EventSystemManager.Instance.SelectGameObject(gameObject);
                SettingsManager.Instance.ScreenWidth = m_width;
                SettingsManager.Instance.ScreenHeight = m_height;
                Screen.SetResolution(m_width, m_height, Screen.fullScreen);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Select();
        }

        public void OnSelect(BaseEventData eventData)
        {
            Select();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            m_pageGroup.TryGotoPreviousPage();
        }

        private void OnValidate()
        {
            m_button = GetComponent<Button>();
        }
    }
}
