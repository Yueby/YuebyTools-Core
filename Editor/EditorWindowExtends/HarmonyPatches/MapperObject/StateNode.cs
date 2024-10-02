using UnityEditor.Animations;
using UnityEngine;
using Yueby.Utils.Reflections;

namespace Yueby.EditorWindowExtends.HarmonyPatches.MapperObject
{
    [MappingClass]
    public class StateNode : Object
    {
        [CustomMapping("state")] public AnimatorState State;

        [CustomMapping("position")] public Rect Position;

        [CustomMapping("title")] public string Title { get; set; }

        private Rect _rect;

        public Rect Rect
        {
            get
            {
                if (_rect == default)
                {
                    _rect = new Rect(Position.x, Position.y, Position.width, Position.height);
                }

                return _rect;
            }
        }
    }
}