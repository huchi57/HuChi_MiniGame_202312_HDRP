using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace UrbanFox.MiniGame
{
    public class CheckpointIndicator : MonoBehaviour
    {
        [SerializeField]
        private Light m_light;

        [SerializeField]
        private float m_lightFadeTime = 1;

        [SerializeField]
        private float m_delayTurnOnTime = 0;

        private Color m_cacheLightColor;

        public void TurnOnLight()
        {
            StartCoroutine(DoTurnOnLight());
            IEnumerator DoTurnOnLight()
            {
                yield return new WaitForSeconds(m_delayTurnOnTime);
                m_light.DOColor(m_cacheLightColor * 0.8f, m_lightFadeTime * 0.7f).OnComplete(() =>
                {
                    m_light.DOColor(m_cacheLightColor * 0.7f, m_lightFadeTime * 0.2f).OnComplete(() =>
                    {
                        m_light.DOColor(m_cacheLightColor, m_lightFadeTime * 0.1f);
                    });
                });
            }
        }

        private void Start()
        {
            if (m_light)
            {
                m_cacheLightColor = m_light.color;
                if (Vector3.Distance(GameManager.Player.position, transform.position) < 10f)
                {
                    TurnOnLight();
                }
                else
                {
                    m_light.color = Color.black;
                }
            }
        }
    }
}
