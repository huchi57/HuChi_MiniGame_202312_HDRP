using UnityEngine;

namespace UrbanFox
{
    [AddComponentMenu("OwO/Object Toggler")]
    public class ObjectToggler : MonoBehaviour
    {
        [SerializeField, Info("When enabled, only one object will be activated when toggled. The rest will be set as inactive.")]
        private bool m_singleActiveMode = true;

        [SerializeField, HideInInspector]
        private GameObject[] m_objects;

        public void ToggleObjectOn(int objectIndex)
        {
            if (!objectIndex.IsInRange(m_objects) || m_objects.IsNullOrEmpty())
            {
                return;
            }

            for (int i = 0; i < m_objects.Length; i++)
            {
                if (m_objects[i])
                {
                    if (i == objectIndex)
                    {
                        m_objects[i].SetActive(true);
                    }
                    else
                    {
                        if (m_singleActiveMode)
                        {
                            m_objects[i].SetActive(false);
                        }
                    }
                }
            }
        }

        public void ToggleObjectOff(int objectIndex)
        {
            if (!objectIndex.IsInRange(m_objects) || m_objects.IsNullOrEmpty())
            {
                return;
            }

            if (m_objects[objectIndex])
            {
                m_objects[objectIndex].SetActive(false);
            }
        }

        public void PopulateObjectsWithChildren()
        {
            m_objects = new GameObject[transform.childCount];

            for (int i = 0; i < m_objects.Length; i++)
            {
                m_objects[i] = transform.GetChild(i).gameObject;
            }
        }
    }
}
