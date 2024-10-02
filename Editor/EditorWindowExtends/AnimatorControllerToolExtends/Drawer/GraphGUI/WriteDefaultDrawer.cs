using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core;
using Yueby.EditorWindowExtends.HarmonyPatches.MapperObject;
using YuebyTools.Core.Utils;

public class WriteDefaultDrawer : GraphGUIDrawer
{
    public override void OnDrawGraphGUI(GraphGUI graphGUI, StateNode stateNode)
    {
        base.OnDrawGraphGUI(graphGUI, stateNode);
        if (stateNode.State == null) return;
        var writeDefaultValues = stateNode.State.writeDefaultValues;
        var wdLabelRect = stateNode.Rect;

        var label = writeDefaultValues ? "WD On" : "WD Off";
        var labelSize = GUI.skin.label.CalcSize(new GUIContent(label));

        wdLabelRect.xMin = wdLabelRect.xMax - labelSize.x - 5;
        wdLabelRect.width = labelSize.x;
        wdLabelRect.height = labelSize.y;
        wdLabelRect.y += stateNode.Rect.height - wdLabelRect.height - 2;


        GUI.Label(wdLabelRect, label, Styles.WdLabelStyle);

        if (Event.current.type == EventType.MouseDown && wdLabelRect.Contains(Event.current.mousePosition))
        {
            writeDefaultValues = !writeDefaultValues;
            stateNode.State.writeDefaultValues = writeDefaultValues;
            Log.Info(graphGUI.Instance, stateNode.Instance);
        }
    }
}