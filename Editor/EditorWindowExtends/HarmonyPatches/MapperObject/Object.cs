using System;
using Yueby.Utils.Reflections;

namespace Yueby.EditorWindowExtends.HarmonyPatches.MapperObject
{
    [MappingClass]
    [Serializable]
    public class Object
    {
        [CustomMapping("m_InstanceID")] public int InstanceID;
        [CustomMapping("name")] public string Name { get; set; }

        public object Instance;
    }
}