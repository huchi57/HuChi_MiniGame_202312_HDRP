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
            SetInactive,
            ShadowsOnly
        }

        [SerializeField]
        private Type m_onStart;

        [SerializeField, NonEditable, HideInInspector]
        private Renderer m_renderer;

        [SerializeField, NonEditable, HideInInspector]
        private Graphic m_graphic;

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
                case Type.ShadowsOnly:
                    if(m_renderer)
                    {
                        m_renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
