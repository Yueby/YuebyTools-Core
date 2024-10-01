using HarmonyLib;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Yueby.EditorWindowExtends.ProjectBrowserExtends;

namespace Yueby.EditorWindowExtends.HarmonyPatches
{
    public static class ProjectBrowserPatch
    {
        internal static void Patch(Harmony harmony)
        {
            // Patch ProjectBrowserColumnOneTreeViewGUI.OnRowGUI
            var onDoItemGUIMethod = AccessTools.Method(typeof(ProjectBrowserPatch), nameof(DoItemGUIPrefix));
            harmony.Patch(ProjectBrowserReflect.AssetsTreeViewGUIType.Method("DoItemGUI", new[]
            {
                typeof(Rect),
                typeof(int),
                typeof(TreeViewItem),
                typeof(bool),
                typeof(bool),
                typeof(bool)
            }), new HarmonyMethod(onDoItemGUIMethod));


            // Patch ObjectListArea.LocalGroup.DrawItem
            var drawItemMethod = AccessTools.Method(typeof(ProjectBrowserPatch), nameof(DrawItemPrefix));
            harmony.Patch(ProjectBrowserReflect.LocalGroupType.Method("DrawItem", new[]
            {
                typeof(Rect),
                ProjectBrowserReflect.FilterResultType,
                ProjectBrowserReflect.BuiltinResourceType,
                typeof(bool)
            }), new HarmonyMethod(drawItemMethod));

            ProjectBrowserExtender.Instance?.Repaint();
        }


        private static bool DoItemGUIPrefix(Rect rect, int row, TreeViewItem item, bool selected, bool focused, bool useBoldFont)
        {
            if (item == null) return true;


            ProjectBrowserExtender.OnProjectBrowserTreeViewItemGUI(item.id, rect, item);
            return true;
        }

        private static bool DrawItemPrefix(Rect position, object filterItem, object builtinResource, bool isFolderBrowsing)
        {
            if (filterItem == null) return true;

            var instanceID = (int)ProjectBrowserReflect.FilterResultType.Field("instanceID").GetValue(filterItem);
            ProjectBrowserExtender.OnProjectBrowserObjectAreaItemGUI(instanceID, position);
            return true;
        }
    }
}