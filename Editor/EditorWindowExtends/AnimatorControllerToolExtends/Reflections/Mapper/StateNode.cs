using UnityEditor.Animations;
using UnityEngine;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections.Mapper
{
    public class StateNode
    {
        public AnimatorState state;

        public Rect position;
        public string title;

        public int m_InstanceID;

        public int GetInstanceID()
        {
            return m_InstanceID;
        }


    }
}