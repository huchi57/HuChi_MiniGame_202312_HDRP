using System;
using UnityEngine;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    public class AutoHideOnStart : MonoBehaviour
    {
        [Serializable]
        public enum Type
        {
            DoNothing,
            HideRenderer,
            HideGraphic,
            Destroy,
            SetInactive
        }

        [SerializeField, NonEditable, ShowIf(nameof(m_onStart), Type.HideRenderer)]
        private Renderer m_renderer;

        [SerializeField, NonEditable, ShowIf(nameof(m_onStart), Type.HideGraphic)]
        private Graphic m_graphic;

        [SerializeField]
        private Type m_onStart;

        private void OnValidate()
        {
            m_renderer = GetComponent<Renderer>();
            m_graphic = GetComponent<Graphic>();
        }

        private void Start()
        {
            switch (m_onStart)
            {
                case Type.DoNothing:
                    break;
                case Type.HideRenderer:
                    if (m_renderer)
                    {
                        m_renderer.enabled = false;
                    }
                    break;
                case Type.HideGraphic:
                    if (m_graphic)
                    {
                        m_graphic.enabled = false;
                    }
                    break;
                case Type.Destroy:
                    Destroy(gameObject);
                    break;
                case Type.SetInactive:
                    gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }
}
