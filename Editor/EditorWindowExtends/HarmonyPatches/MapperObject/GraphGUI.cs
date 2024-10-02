using System;
using Yueby.Utils.Reflections;

namespace Yueby.EditorWindowExtends.HarmonyPatches.MapperObject
{
    [MappingClass]
    [Serializable]
    public class GraphGUI : Object
    {
        public object Instance;
        [CustomMapping("m_Graph")] public Graph Graph;
    }
}