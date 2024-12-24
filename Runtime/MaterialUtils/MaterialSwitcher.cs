using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yueby
{
#if VRC_SDK_VRCSDK3
    public class MaterialSwitcher : MonoBehaviour, VRC.SDKBase.IEditorOnly
#else
    public class MaterialSwitcher : MonoBehaviour
#endif
    {
        [Serializable]
        public class RendererConfig
        {
            public string rendererPath;
            public string name;
            public int selectedMaterialIndex;
            public int currentMaterialIndex;
            public List<Material> materials = new List<Material>();
            public bool isFoldout = true;
        }

        public string configName = "Default Config";
        public List<RendererConfig> rendererConfigs;

        private void Reset()
        {
            rendererConfigs = new List<RendererConfig>();
            rendererConfigs.Add(new RendererConfig());
        }
    }
}