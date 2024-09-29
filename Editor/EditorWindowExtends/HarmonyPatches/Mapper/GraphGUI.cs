using System;
using UnityEditor.Graphs;
using Yueby.Utils.Reflections;

namespace Yueby.EditorWindowExtends.HarmonyPatches.Mapper
{
    [MappingClass]
    [Serializable]
    public class GraphGUI : Object
    {
        [CustomMapping("m_Graph")] public Graph Graph;
    }
}