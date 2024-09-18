using UnityEditorInternal;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core;
using Yueby.EditorWindowExtends.Reflections;
using Yueby.EditorWindowExtends.Utils;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Drawer.ParameterControllerView
{
    public class HoverBackgroundDrawer : ParameterControllerViewDrawer
    {
        public override string DrawerName => "Hover Background";

        public override void Init(ParameterControllerViewExtender extender, ReorderableList reorderableList)
        {
            base.Init(extender, reorderableList);
        }

        public override void OnDrawElementBackground(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (rect.Contains(Event.current.mousePosition) && !isactive)
            {
                using (new BackgroundColorScope(GameObjectStylesReflect.GetHoveredBackgroundColor()))
                {
                    GUI.Label(rect, GUIContent.none, GameObjectStylesReflect.GetHoveredItemBackgroundStyle());
                }
            }
        }
    }
}