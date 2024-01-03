using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class CrushableObject : MonoBehaviour
    {
        [SerializeField, Required]
        private MeshRenderer m_originalMesh;

        [SerializeField, Required]
        private MeshRenderer[] m_crushedMeshCandidates;

        [SerializeField]
        private string m_crushZoneTag = "CrushZone";

        public void CrushObject()
        {
            if (m_originalMesh)
            {
                m_originalMesh.enabled = false;
            }
            var target = m_crushedMeshCandidates.SelectRandom();
            if (target)
            {
                target.enabled = true;
            }
        }

        private void Awake()
        {
            RestoreOriginalObject();
        }

        private void Start()
        {
            GameManager.OnGameFullyFadeOutAndReloadStarted += RestoreOriginalObject;
        }

        private void OnDestroy()
        {
            GameManager.OnGameFullyFadeOutAndReloadStarted -= RestoreOriginalObject;
        }

        private void OnCollisionEnter(Collision collision)
        {
            //if (collision.gameObject.GetComponentInParent<Combustor>() || collision.gameObject.GetComponentInChildren<Combustor>())
            //{
            //    CrushObject();
            //}
            if (collision.gameObject.CompareTag(m_crushZoneTag))
            {
                CrushObject();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //if (other.GetComponentInParent<Combustor>() || other.GetComponentInChildren<Combustor>())
            //{
            //    CrushObject();
            //}
            if (other.CompareTag(m_crushZoneTag))
            {
                CrushObject();
            }
        }

        private void RestoreOriginalObject()
        {
            if (!m_crushedMeshCandidates.IsNullOrEmpty())
            {
                foreach (var item in m_crushedMeshCandidates)
                {
                    if (item)
                    {
                        item.enabled = false;
                    }
                }
            }
            if (m_originalMesh)
            {
                m_originalMesh.enabled = true;
            }
        }
    }
}
