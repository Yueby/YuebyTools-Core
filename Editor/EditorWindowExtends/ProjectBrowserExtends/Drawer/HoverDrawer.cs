using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Yueby.EditorWindowExtends.Core;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;
using Yueby.EditorWindowExtends.Reflections;
using Yueby.EditorWindowExtends.Utils;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.Drawer
{
    public class HoverDrawer : ProjectBrowserDrawer
    {
        public override string DrawerName => "Hover Background";
        public override string Tooltip => "Draw a background when hovering over an item in the project browser.";
        private Rect _hoverRect;
        private bool _isDown;
        private AssetItem _lastHoverItem;
        private readonly DrawerColor bkgColor = new(new Color(0f, 0f, 0f, 0.03137255f), new Color(0f, 0f, 0f, 0.07450981f));

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

            var color = bkgColor.GetColor(); //GameObjectStylesReflect.GetHoveredBackgroundColor();
            // color.a = 0.5f;

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