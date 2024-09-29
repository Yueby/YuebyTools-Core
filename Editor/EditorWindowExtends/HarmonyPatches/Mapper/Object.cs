using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yueby.Utils.Reflections;

namespace Yueby.EditorWindowExtends.HarmonyPatches.Mapper
{
    [MappingClass]
    [Serializable]
    public class Object
    {
        [CustomMapping("m_InstanceID")]
        public int InstanceID;
        [CustomMapping("name")] public string Name { get; set; }
    }
}