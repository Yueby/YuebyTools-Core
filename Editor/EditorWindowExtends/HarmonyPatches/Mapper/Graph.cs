using System.Collections.Generic;
using UnityEditor.Graphs;
using Yueby.Utils.Reflections;

namespace Yueby.EditorWindowExtends.HarmonyPatches.Mapper
{
    [MappingClass]
    public class Graph
    {
        public List<Node> nodes = new List<Node>();
        public string name;
    }
}