using System;
using System.Reflection;
using HarmonyLib;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections
{
    public static class AnimatorControllerToolReflect
    {
        public static readonly Type Type = AccessTools.TypeByName("UnityEditor.Graphs.AnimatorControllerTool");
        public static readonly FieldInfo LayerEditor = AccessTools.Field(Type, "m_LayerEditor");
        public static readonly FieldInfo ParameterEditor = AccessTools.Field(Type, "m_ParameterEditor");
        public static readonly FieldInfo ToolFieldInfo = AccessTools.Field(Type, "tool");
        public static readonly MethodInfo DoGraphToolbarMethodInfo = AccessTools.Method(Type, "DoGraphToolbar");

        public static readonly Type EdgeGUIType = AccessTools.TypeByName("UnityEditor.Graphs.AnimationStateMachine.EdgeGUI");
        public static readonly Type EdgeInfoType = AccessTools.TypeByName("UnityEditor.Graphs.AnimationStateMachine.EdgeInfo");

        public static readonly Type StateNodeType = AccessTools.TypeByName("UnityEditor.Graphs.AnimationStateMachine.StateNode");
        public static readonly Type GraphGUIType = AccessTools.TypeByName("UnityEditor.Graphs.AnimationStateMachine.GraphGUI");
    }
}