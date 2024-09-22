using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;
using Yueby.EditorWindowExtends.Reflections;
using Yueby.EditorWindowExtends.Utils;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.Drawer
{
    public class HoverDrawer : ProjectBrowserDrawer
    {
        public override string DrawerName => "Hover Background";

        public override void OnProjectBrowserObjectAreaItemGUI(AssetItem item)
        {
            DrawHover(item);
        }

        public override void OnProjectBrowserTreeViewItemGUI(AssetItem item, TreeViewItem treeViewItem)
        {
            DrawHover(item);
        }

        private void DrawHover(AssetItem item)
        {
            // if (item.IsHover && Selection.activeObject != item.Asset)
            // {
            //     var rect = item.OriginRect;
            //
            //     rect.y += rect.height;
            //     rect.height = 1;
            //     EditorGUI.DrawRect(rect, Color.gray);
            // }

            var rect = item.Rect;

            if (item.OriginRect.height > EditorGUIUtility.singleLineHeight)
            {
                const float expand = 2;
                rect.x -= expand;
                rect.width += expand * 2;
                rect.y -= expand;
                rect.height += expand * 2;
            }

            // Debug.Log("rect: " + rect);
            if (!item.IsHover /*|| Selection.activeObject == item.Asset*/) return;

            var color = GameObjectStylesReflect.GetHoveredBackgroundColor();

            using (new BackgroundColorScope(color))
            {
                var style = GameObjectStylesReflect.GetHoveredItemBackgroundStyle();
                // if (item.OriginRect.height > EditorGUIUtility.singleLineHeight)
                // {
                //     style.normal.background = EditorGUIUtility.whiteTexture;
                // }
                GUI.Label(
                    rect,
                    GUIContent.none,
                    style
                );
            }
        }
    }
}