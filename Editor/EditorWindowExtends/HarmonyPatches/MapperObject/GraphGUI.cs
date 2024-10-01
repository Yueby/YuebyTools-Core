using System;
using Yueby.Utils.Reflections;

namespace Yueby.EditorWindowExtends.HarmonyPatches.MapperObject
{
    [MappingClass]
    [Serializable]
    public class GraphGUI : Object
    {
        [CustomMapping("m_Graph")] public Graph Graph;
    }
}