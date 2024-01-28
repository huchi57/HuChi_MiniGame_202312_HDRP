using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class SceneReferencesData : ScriptableObject
    {
        [Serializable]
        public struct SubareaData
        {
            //[Scene]
            public string EnvironmentScene;

            [Scene]
            public string GameplayScene;
        }

        [Serializable]
        public struct AreaData
        {
            [Scene]
            public string BackdropScene;

            public SubareaData[] SubAreas;
        }

        [SerializeField]
        private AreaData[] m_areaData;

        [Scene]
        public string gT;
    }
}
