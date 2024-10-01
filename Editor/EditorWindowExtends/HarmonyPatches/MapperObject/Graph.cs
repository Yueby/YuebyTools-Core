using System;
using System.Collections.Generic;
using Yueby.Utils.Reflections;

namespace Yueby.EditorWindowExtends.HarmonyPatches.MapperObject
{

    [MappingClass]
    [Serializable]
    public class Graph : Object
    {
        public List<StateNode> nodes = new List<StateNode>();
    }
}