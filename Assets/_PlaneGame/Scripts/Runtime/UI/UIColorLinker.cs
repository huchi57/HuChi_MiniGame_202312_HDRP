using UnityEngine;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Graphic))]
    public class UIColorLinker : MonoBehaviour
    {
        [SerializeField] private Graphic m_graphic;
        [SerializeField] private Graphic m_graphicToLinkColorWith;

        private void OnValidate()
        {
            m_graphic = GetComponent<Graphic>();
        }

        private void LateUpdate()
        {
            if (m_graphic && m_graphicToLinkColorWith)
            {
                m_graphic.color = m_graphicToLinkColorWith.color;
            }
        }
    }
}
