using UnityEditorInternal;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Drawer.LayerControllerView
{
    public class HoverBackgroundDrawer : LayerControllerViewDrawer
    {
        public override string DrawerName => "Hover Background";

        public override void OnDrawElementBackground(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (rect.Contains(Event.current.mousePosition) && !isactive)
            {
                // EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f,));
                GUI.Box(rect, "");
            }
        }
    }
}