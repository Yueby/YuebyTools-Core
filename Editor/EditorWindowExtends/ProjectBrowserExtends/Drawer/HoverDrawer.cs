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
        private Rect _hoverRect;
        private bool _isDown;
        private AssetItem _lastHoverItem;

        public override void OnProjectBrowserObjectAreaItemGUI(AssetItem item)
        {
            DrawHover(item);
        }

        public override void OnProjectBrowserTreeViewItemGUI(
            AssetItem item,
            TreeViewItem treeViewItem
        )
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

            if (!item.IsHover)
                return;

            if (_lastHoverItem != item)
            {
                _lastHoverItem = item;
                _isDown = false;
            }

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    _isDown = true;
                    break;
                case EventType.MouseUp:
                    _isDown = false;
                    break;
            }

            rect = ExpandRect(
                rect,
                _isDown ? 4
                    : item.OriginRect.height > EditorGUIUtility.singleLineHeight ? 2
                    : 0
            );

            var color = GameObjectStylesReflect.GetHoveredBackgroundColor();

            using (new BackgroundColorScope(color))
            {
                var style = GameObjectStylesReflect.GetHoveredItemBackgroundStyle();
                GUI.Label(rect, GUIContent.none, style);
            }

            // EditorWindow.mouseOverWindow.Repaint();
            // Extender.Repaint();
        }

        private Rect ExpandRect(Rect rect, int expand)
        {
            return new Rect(
                rect.x - expand,
                rect.y - expand,
                rect.width + expand * 2,
                rect.height + expand * 2
            );
        }
    }
}
