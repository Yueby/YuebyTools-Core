using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections;
using Yueby.EditorWindowExtends.HarmonyPatches.MapperObject;
using Yueby.Utils.Reflections;
using Object = UnityEngine.Object;

namespace Yueby.EditorWindowExtends.HarmonyPatches
{
    public static class AnimatorControllerToolPatch
    {
        private static Dictionary<int, StateNode> _stateNodeCaches = new();
        private static GraphGUI _graphGUI;

        private static Harmony _harmony;
        private static MethodInfo _animatorControllerToolOnGUIMethod = AnimatorControllerToolReflect.Type.Method("OnGUI");

        public static event Action<GraphGUI, StateNode> OnDrawStateNode;

        private static readonly List<string> NodeIgnore = new()
        {
            "Any State",
            "Entry",
            "Exit",
        };


        internal static void Patch(Harmony harmony)
        {
            _harmony = harmony;

            // GraphGUI - OnGraphGUI
            var onGraphGUIMethodPrefix = AccessTools.Method(typeof(AnimatorControllerToolPatch), nameof(OnGraphGUIPrefix));
            var onGraphGUIMethodPostfix = AccessTools.Method(typeof(AnimatorControllerToolPatch), nameof(OnGraphGUIPostfix));

            harmony.Patch(AnimatorControllerToolReflect.GraphGUIType.Method("OnGraphGUI"), new HarmonyMethod(onGraphGUIMethodPrefix), new HarmonyMethod(onGraphGUIMethodPostfix));
        }


        private static void OnGraphGUIPrefix(Object __instance)
        {
            if (__instance == null)
                return;

            _graphGUI = Mapper.Map<GraphGUI>(__instance);
            _graphGUI.Instance = __instance;
            // Log.Info(JsonUtility.ToJson(graphGUI), __instance.GetInstanceID(), "|", graphGUI.Name, graphGUI.Graph.nodes.Count);
        }

        private static void OnGraphGUIPostfix()
        {
            if (_graphGUI == null) return;
            foreach (var node in _graphGUI.Graph.nodes)
            {
                if (NodeIgnore.Contains(node.Title)) continue;

                GraphGUIExtender.OnDrawStateNode(_graphGUI, node);
            }
        }
    }
}