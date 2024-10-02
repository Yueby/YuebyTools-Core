using UnityEngine;
using Yueby.EditorWindowExtends.Core;
using Yueby.EditorWindowExtends.HarmonyPatches.MapperObject;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core
{
    public class GraphGUIDrawer : EditorExtenderDrawer<GraphGUIExtender, GraphGUIDrawer>
    {
        public virtual void OnDrawGraphGUI(GraphGUI graphGUI, StateNode stateNode)
        {
        }

        public static class Styles
        {
            public static GUIStyle LabelStyle = new(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(GUI.skin.label.normal.textColor.r, GUI.skin.label.normal.textColor.g, GUI.skin.label.normal.textColor.b, 0.7f) },
            };

            public static GUIStyle WdLabelStyle = new(LabelStyle)
            {
                fontSize = 8
            };
        }
    }
}