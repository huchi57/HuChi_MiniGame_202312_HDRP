using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class CameraBrain : MonoBehaviour
    {
        public static CameraBrain Main { get; private set; }

        private readonly List<CameraContributorBase> m_cameraContributors = new List<CameraContributorBase>();

        public void AddContributor(CameraContributorBase contributor)
        {
            if (contributor && !m_cameraContributors.Contains(contributor))
            {
                m_cameraContributors.Add(contributor);
            }
        }

        public void RemoveContributor(CameraContributorBase contributor)
        {
            if (contributor && m_cameraContributors.Contains(contributor))
            {
                m_cameraContributors.Remove(contributor);
            }
        }

        public void ClearAllContributors()
        {
            m_cameraContributors.Clear();
        }

        private void Awake()
        {
            if (Main)
            {
                FoxyLogger.LogWarning($"A duplicated camera has been found. Only one should be present.");
            }
            Main = this;
        }
    }
}
