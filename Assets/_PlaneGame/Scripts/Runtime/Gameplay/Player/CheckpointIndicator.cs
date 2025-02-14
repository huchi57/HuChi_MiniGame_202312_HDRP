using System.Collections;
using UnityEngine;
using DG.Tweening;
using FMODUnity;

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

        [SerializeField]
        private EventReference m_lightOnSound;

        private bool m_isTurningOn;
        private Color m_cacheLightColor;

        public void TurnOnLight(bool playSound = true)
        {
            StartCoroutine(DoTurnOnLight());
            IEnumerator DoTurnOnLight()
            {
                yield return new WaitForSeconds(m_delayTurnOnTime);
                if (playSound && !m_isTurningOn)
                {
                    m_lightOnSound.PlayOneShot(m_light ? m_light.transform.position : transform.position);
                }
                m_light.DOColor(m_cacheLightColor * 0.8f, m_lightFadeTime * 0.7f).OnComplete(() =>
                {
                    m_light.DOColor(m_cacheLightColor * 0.7f, m_lightFadeTime * 0.2f).OnComplete(() =>
                    {
                        m_light.DOColor(m_cacheLightColor, m_lightFadeTime * 0.1f);
                    });
                });
                m_isTurningOn = true;
            }
        }

        private void Start()
        {
            if (m_light)
            {
                m_cacheLightColor = m_light.color;
                if (Vector3.Distance(GameInstance.PlayerTransform.position, transform.position) < 10f)
                {
                    TurnOnLight(playSound: false);
                }
                else
                {
                    m_light.color = Color.black;
                }
            }
        }
    }
}
