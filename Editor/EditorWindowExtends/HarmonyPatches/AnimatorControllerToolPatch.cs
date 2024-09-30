using System.Collections.Generic;
using HarmonyLib;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections;
using Yueby.EditorWindowExtends.HarmonyPatches.Mapper;
using Yueby.Utils.Reflections;
using YuebyTools.Core.Utils;
using GraphGUI = Yueby.EditorWindowExtends.HarmonyPatches.Mapper.GraphGUI;
using Object = UnityEngine.Object;

namespace Yueby.EditorWindowExtends.HarmonyPatches
{
    public static class AnimatorControllerToolPatch
    {
        private static Dictionary<int, StateNode> _stateNodeCaches = new();
        private static GraphGUI _graphGUI;

        private static readonly List<string> NodeIgnore = new()
        {
            "Any State",
            "Entry",
            "Exit",
        };


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


        private static void OnGraphGUIPrefix(Object __instance)
        {
            if (__instance == null)
                return;

            _graphGUI = ReflectionUtil.Map<GraphGUI>(__instance);

            // Log.Info(JsonUtility.ToJson(graphGUI), __instance.GetInstanceID(), "|", graphGUI.Name, graphGUI.Graph.nodes.Count);
        }

        private static void OnGraphGUIPostfix()
        {
            if (_graphGUI == null) return;
            foreach (var node in _graphGUI.Graph.nodes)
            {
                if (NodeIgnore.Contains(node.Title)) continue;

                var label = node.State.motion == null ? "None" : node.State.motion.name;
                var rect = node.Position;
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
}