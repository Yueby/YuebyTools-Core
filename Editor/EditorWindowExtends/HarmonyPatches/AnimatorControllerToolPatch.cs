using System.Collections.Generic;
using HarmonyLib;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections;
using Yueby.EditorWindowExtends.HarmonyPatches.Mapper;
using Yueby.Utils.Reflections;
using YuebyTools.Core.Utils;
using GraphGUI = Yueby.EditorWindowExtends.HarmonyPatches.Mapper.GraphGUI;

namespace Yueby.EditorWindowExtends.HarmonyPatches
{
    public static class AnimatorControllerToolPatch
    {
        private static Dictionary<int, StateNode> _stateNodeCaches = new();
        private static Object _graphUIInstance = null;

        internal static void Patch(Harmony harmony)
        {
            var nodeUIMethodPrefix = AccessTools.Method(typeof(AnimatorControllerToolPatch), nameof(OnGraphGUIPrefix));
            var nodeUIMethodPostfix = AccessTools.Method(typeof(AnimatorControllerToolPatch), nameof(OnGraphGUIPostfix));

            harmony.Patch(AnimatorControllerToolReflect.GraphGUIType.Method("OnGraphGUI"), new HarmonyMethod(nodeUIMethodPrefix), new HarmonyMethod(nodeUIMethodPostfix));
        }

        // private static bool DrawEdge(object __instance, Edge edge, Texture2D tex, Color color, object info, bool viewHasLiveLinkExactEdge)
        // {
        //     Log.Info(edge.toSlotName);
        //     return true;
        // }


        private static StateNode _currentStateNode;

        private static void OnGraphGUIPrefix(Object __instance)
        {
            if (__instance == null) return;

            var graphGUI = AutoMapper.Map<GraphGUI>(__instance);

            Log.Info(JsonUtility.ToJson(graphGUI));
        }

        private static void OnGraphGUIPostfix()
        {
            // if (_graphUIInstance == null) return;

            // AutoMapper.Map(n, _currentStateNode ??= new StateNode());

            // var label = _currentStateNode.state.motion == null ? "None" : _currentStateNode.state.motion.name;
            // GUI.Label(_currentStateNode.position, label, EditorStyles.centeredGreyMiniLabel);
        }
    }
}