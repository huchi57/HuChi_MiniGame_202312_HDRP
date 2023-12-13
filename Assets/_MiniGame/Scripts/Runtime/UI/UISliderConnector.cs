using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    public class UISliderConnector : MonoBehaviour
    {
        [SerializeField, Required] private Slider m_slider;
        [SerializeField] private float m_timeInterval;

        private bool m_isHoldingInput = false;
        private float m_timer = 0;

        private void Update()
        {
            if (EventSystem.current && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                if (InputManager.Move.x > 0.1f)
                {
                    if (!m_isHoldingInput)
                    {
                        m_isHoldingInput = true;
                        m_slider.value++;
                    }
                    else
                    {
                        m_timer += Time.deltaTime;
                        if (m_timer > m_timeInterval)
                        {
                            m_timer = 0;
                            m_slider.value++;
                        }
                    }
                }
                else if (InputManager.Move.x < -0.1f)
                {
                    if (!m_isHoldingInput)
                    {
                        m_isHoldingInput = true;
                        m_slider.value--;
                    }
                    else
                    {
                        m_timer += Time.deltaTime;
                        if (m_timer > m_timeInterval)
                        {
                            m_timer = 0;
                            m_slider.value--;
                        }
                    }
                }
                else
                {
                    m_isHoldingInput = false;
                    m_timer = 0;
                }
            }
            else
            {
                m_isHoldingInput = false;
                m_timer = 0;
            }
        }
    }
}
