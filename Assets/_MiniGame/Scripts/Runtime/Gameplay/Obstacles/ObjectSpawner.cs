using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class ObjectSpawner : MonoBehaviour
    {
        private readonly Queue<GameObject> m_spawnedInstances = new Queue<GameObject>();

        [SerializeField, Required]
        private GameObject[] m_candidatesToSpawn;

        [SerializeField]
        private uint m_maxObjectCount = 100;

        [SerializeField, Min(0)]
        private float m_spawnIntervalMin = 0.5f;

        [SerializeField, Min(0)]
        private float m_spawnIntervalMax = 1;

        [SerializeField]
        private int m_objectsToPrewarmSpawn;

        [SerializeField]
        private Vector3 m_objectDistanceIntervals;

        [SerializeField]
        private bool m_randomizeRotation;

        [SerializeField, Indent, ShowIf(nameof(m_randomizeRotation), true)]
        private float m_eulerAngleXMin;

        [SerializeField, Indent, ShowIf(nameof(m_randomizeRotation), true)]
        private float m_eulerAngleXMax;

        [SerializeField, Indent, ShowIf(nameof(m_randomizeRotation), true)]
        private float m_eulerAngleYMin;

        [SerializeField, Indent, ShowIf(nameof(m_randomizeRotation), true)]
        private float m_eulerAngleYMax;

        [SerializeField, Indent, ShowIf(nameof(m_randomizeRotation), true)]
        private float m_eulerAngleZMin;

        [SerializeField, Indent, ShowIf(nameof(m_randomizeRotation), true)]
        private float m_eulerAngleZMax;

        public GameObject SpawnAnObject()
        {
            var rotation = Quaternion.identity;
            if (m_randomizeRotation)
            {
                rotation = Quaternion.Euler(Random.Range(m_eulerAngleXMin, m_eulerAngleXMax), Random.Range(m_eulerAngleYMin, m_eulerAngleYMax), Random.Range(m_eulerAngleZMin, m_eulerAngleZMax));
            }
            var instance = Instantiate(m_candidatesToSpawn.SelectRandom(), transform.position, rotation);
            if (instance)
            {
                instance.transform.SetParent(transform, worldPositionStays: true);
                instance.SetActive(true);
                m_spawnedInstances.Enqueue(instance);
                if (m_spawnedInstances.Count > m_maxObjectCount)
                {
                    Destroy(m_spawnedInstances.Dequeue());
                }
            }
            return instance;
        }

        private IEnumerator Start()
        {
            if (!m_candidatesToSpawn.IsNullOrEmpty())
            {
                foreach (var item in m_candidatesToSpawn)
                {
                    if (item)
                    {
                        item.SetActive(false);
                    }
                }
                if (m_objectsToPrewarmSpawn > 0)
                {
                    for (int i = 0; i < m_objectsToPrewarmSpawn; i++)
                    {
                        var instance = SpawnAnObject();
                        if (instance)
                        {
                            instance.transform.position += i * m_objectDistanceIntervals;
                        }
                    }
                }
                while (true)
                {
                    yield return new WaitForSeconds(Random.Range(m_spawnIntervalMin, m_spawnIntervalMax));
                    SpawnAnObject();
                }
            }
        }
    }
}
