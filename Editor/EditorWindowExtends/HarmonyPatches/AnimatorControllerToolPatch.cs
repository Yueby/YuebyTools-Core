using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Graphs;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections.Mapper;
using Yueby.Utils.Reflections;
using YuebyTools.Core.Utils;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.HarmonyPatches
{
    public static class AnimatorControllerToolPatch
    {
        internal static void Patch(Harmony harmony)
        {
            var nodeUIMethod = AccessTools.Method(typeof(AnimatorControllerToolPatch), nameof(NodeUI));
            harmony.Patch(AnimatorControllerToolReflect.StateNodeType.Method("NodeUI", new[]
            {
                typeof(GraphGUI),
            }), new HarmonyMethod(nodeUIMethod));

        }

        // private static bool DrawEdge(object __instance, Edge edge, Texture2D tex, Color color, object info, bool viewHasLiveLinkExactEdge)
        // {
        //     Log.Info(edge.toSlotName);
        //     return true;
        // }

        private static bool NodeUI(object __instance, GraphGUI host)
        {
            var stateNode = ReflectionUtil.MapToInterface<StateNode>(__instance);
            Log.Info(stateNode.state);

            // var label = stateNode.state.motion == null ? "None" : stateNode.state.motion.name;

            // EditorGUI.LabelField(stateNode.position, label);

            // if (state.motion != null)
            //     Log.Info(state.motion.name);
            return true;
        }
    }
}