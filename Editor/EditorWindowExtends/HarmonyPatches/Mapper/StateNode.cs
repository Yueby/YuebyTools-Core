using UnityEditor.Animations;
using UnityEngine;
using Yueby.Utils.Reflections;

namespace Yueby.EditorWindowExtends.HarmonyPatches.Mapper
{
    [MappingClass]
    public class StateNode
    {
        public AnimatorState state;

        public Rect position;
        public string title { get; set; }

        public int m_InstanceID;

        public int GetInstanceID()
        {
            return m_InstanceID;
        }
    }
}