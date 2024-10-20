using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core;
using Yueby.EditorWindowExtends.HarmonyPatches.MapperObject;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Drawer.GraphGUI
{
    public class MotionNameDrawer : GraphGUIDrawer
    {
        public override string DrawerName => "Display Motion Name";
        public override string Tooltip => "Display the name of the motion in the state node";

        public override void OnDrawGraphGUI(HarmonyPatches.MapperObject.GraphGUI graphGUI, StateNode stateNode)
        {
            base.OnDrawGraphGUI(graphGUI, stateNode);
            if (stateNode.State == null) return;
            var label = stateNode.State.motion == null ? "None" : stateNode.State.motion.name;
            var labelSize = GUI.skin.label.CalcSize(new GUIContent(label));
            var rect = stateNode.Rect;
            rect.height = labelSize.y;
            rect.width = labelSize.x;
            rect.x = stateNode.Rect.x + stateNode.Rect.width / 2 - rect.width / 2;

            rect.y += stateNode.Rect.height - rect.height - 2;

            GUI.Label(rect, label, Styles.LabelStyle);
        }
    }
}