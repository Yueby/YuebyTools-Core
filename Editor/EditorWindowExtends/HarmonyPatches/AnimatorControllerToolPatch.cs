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
        private static Dictionary<int, StateNode> _stateNodeCaches = new();

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

        private static bool NodeUI(Object __instance, GraphGUI host)
        {
            if (_stateNodeCaches.TryGetValue(__instance.GetInstanceID(), out var stateNode))
            {
                Log.Info(JsonUtility.ToJson(stateNode));
            }
            else
            {
                stateNode = ReflectionUtil.Create<StateNode>(__instance);
                _stateNodeCaches.Add(__instance.GetInstanceID(), stateNode);
            }


            //Log.Info(JsonUtility.ToJson(stateNode));


            // var label = stateNode.state.motion == null ? "None" : stateNode.state.motion.name;

            // EditorGUI.LabelField(stateNode.position, label);

            // if (state.motion != null)
            //     Log.Info(state.motion.name);
            return true;
        }
    }
}