using System;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEditor;
using UnityEngine;
using Yueby.EditorWindowExtends.Core;
using YuebyAvatarTools.Packages.yueby.tools.core.Editor.Utils;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.HarmonyPatches
{
    internal static class PatchLoader
    {
        public const string BaseMenuPath = "Tools/YuebyTools/Editor Window Extends/";
        private static readonly Action<Harmony>[] Patches = { ProjectBrowserPatch.Patch };

        private static Harmony _harmony;

        [InitializeOnLoadMethod]
        internal static void PrepareApplyPatches()
        {
            EditorApplication.delayCall += async () =>
            {
                Log.Info("Preparing to apply Harmony patches", 1, 2, 3, 4);
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
                    Log.Warning($"{declaringTypeName} >>> Applied");
                    throw new Exception("Patching failed.");
                }
                catch (Exception e)
                {
                    Log.Info($"{declaringTypeName} >>> Failed to apply.");
                    Log.Error("1. Check if the patch is already applied.");
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

        [MenuItem(BaseMenuPath + "Reapply patches")]
        private static void ReapplyPatches()
        {
            UnpatchAll();
            ApplyPatches();
        }
    }
}
