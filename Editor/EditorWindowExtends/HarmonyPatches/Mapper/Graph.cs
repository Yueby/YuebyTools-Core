using System;
using System.Collections.Generic;
using UnityEditor.Graphs;
using Yueby.Utils.Reflections;

namespace Yueby.EditorWindowExtends.HarmonyPatches.Mapper
{

    [MappingClass]
    [Serializable]
    public class Graph : Object
    {
        public List<StateNode> nodes = new List<StateNode>();
    }
}