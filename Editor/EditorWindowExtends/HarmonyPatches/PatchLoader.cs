using System;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEditor;
using UnityEngine;
using Yueby.EditorWindowExtends.Core;
using YuebyTools.Core.Utils;

namespace Yueby.EditorWindowExtends.HarmonyPatches
{
    internal static class PatchLoader
    {
        public const string BaseMenuPath = "Tools/YuebyTools/Editor Window Extends/";
        private static readonly Action<Harmony>[] Patches =
        {
            ProjectBrowserPatch.Patch ,
             AnimatorControllerToolPatch.Patch
         };

        private static Harmony _harmony;

        [InitializeOnLoadMethod]
        internal static void PrepareApplyPatches()
        {
            EditorApplication.delayCall += async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1f));
                ApplyPatches();
            };
        }

        internal static void ApplyPatches()
        {
            // Debug.Log("Applying Harmony patches");
            _harmony = new Harmony("yueby.tools.core");
            foreach (var patch in Patches)
            {
                var declaringTypeName = patch.Method.DeclaringType.Name;
                try
                {
                    patch(_harmony);
                    Log.Info($"{declaringTypeName} --> Applied.");
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }

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
