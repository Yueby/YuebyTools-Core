using UnityEditor;
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

            var label = stateNode.State.motion == null ? "None" : stateNode.State.motion.name;
            var rect = stateNode.Position;
            rect.y += EditorGUIUtility.singleLineHeight * 0.5f;

            GUI.Label(rect, label, new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(GUI.skin.label.normal.textColor.r, GUI.skin.label.normal.textColor.g, GUI.skin.label.normal.textColor.b, 0.7f) },
            });
        }
    }
}