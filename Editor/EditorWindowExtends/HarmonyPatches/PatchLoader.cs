using System;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEditor;
using UnityEngine;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.HarmonyPatches
{
    internal static class PatchLoader
    {
        private static readonly Action<Harmony>[] Patches =
        {
            ProjectBrowserPatch.Patch
        };

        [InitializeOnLoadMethod]
        internal static void ApplyPatches()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            EditorApplication.update -= OnUpdate;
            _ = DelayCall();
        }

        private static async Task DelayCall()
        {
            await Task.Delay(TimeSpan.FromSeconds(1f));

            // Debug.Log("Applying Harmony patches");
            var harmony = new Harmony("yueby.tools.core");
            foreach (var patch in Patches)
            {
                try
                {
                    patch(harmony);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            AssemblyReloadEvents.beforeAssemblyReload += () => { harmony.UnpatchAll(); };
        }
    }
}