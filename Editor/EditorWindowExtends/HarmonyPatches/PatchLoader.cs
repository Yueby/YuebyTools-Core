using System;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEditor;
using Yueby.Core.Utils;

namespace Yueby.EditorWindowExtends.HarmonyPatches
{
    internal static class PatchLoader
    {
        public const string BaseMenuPath = "Tools/YuebyTools/Editor Window Extends/";

        private static readonly Action<Harmony>[] Patches =
        {
            ProjectBrowserPatch.Patch,
            AnimatorControllerToolPatch.Patch
        };

        private static Harmony _harmony;

        private static int _initializedCount;

        [InitializeOnLoadMethod]
        internal static void PrepareApplyPatches()
        {
            EditorApplication.update += OnUpdate;
        }

        private static async void OnUpdate()
        {
            EditorApplication.update -= OnUpdate;
            await Task.Delay(TimeSpan.FromSeconds(1f));
            ApplyPatches();
        }

        internal static void ApplyPatches()
        {
            // Debug.Log("Applying Harmony patches");
            _harmony = new Harmony("yueby.tools.core");
            foreach (var patch in Patches)
                try
                {
                    patch(_harmony);
                    _initializedCount++;
                }
                catch (Exception e)
                {
                    Logger.LogException(e);
                    _initializedCount--;
                }

            if (_initializedCount == Patches.Length) Logger.LogInfo("All patches applied.");

            AssemblyReloadEvents.beforeAssemblyReload -= UnpatchAll;
            AssemblyReloadEvents.beforeAssemblyReload += UnpatchAll;
        }

        internal static void UnpatchAll()
        {
            _harmony.UnpatchAll();
        }

        // [MenuItem(BaseMenuPath + "Reapply patches")]
        // private static void ReapplyPatches()
        // {
        //     UnpatchAll();
        //     ApplyPatches();
        // }
    }
}