using UnityEditor.Graphs;
using Yueby.Utils.Reflections;

namespace Yueby.EditorWindowExtends.HarmonyPatches.Mapper
{
    [MappingClass]
    public class GraphGUI
    {
        [CustomMapping("m_Graph")] public Graph Graph;
        public string name;
    }
}