using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;
using YuebyTools.Core.Utils;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.Drawer
{
    public class TreeViewLineDrawer : ProjectBrowserDrawer
    {
        private static readonly Color LineColor = new(0.5f, 0.5f, 0.5f, 0.33f);

        public override string DrawerName => "TreeView Line";

        private AssetItem _currentItem;
        private TreeViewItem _currentTreeViewItem;

        public override void OnProjectBrowserTreeViewItemGUI(AssetItem item, TreeViewItem treeViewItem)
        {
            if (treeViewItem is not { parent: not null }) return;

            _currentItem = item;
            _currentTreeViewItem = treeViewItem;
        }

        public override void OnProjectBrowserGUI(AssetItem item)
        {
            if (item != _currentItem || item.OriginRect.height > EditorGUIUtility.singleLineHeight) return;

            var currentTreeViewItem = _currentTreeViewItem;

            var rect = item.OriginRect;

            // 横线
            var hLineRect = rect;
            hLineRect.height = 1;
            hLineRect.x -= rect.height * 0.5f;
            hLineRect.x -= rect.height - 2;

            // 如果children为0，横线多往右画一点
            if (currentTreeViewItem.children == null || currentTreeViewItem.children.Count == 0)
            {
                hLineRect.width = rect.height * 1.2f; // 增加宽度
            }
            else
            {
                hLineRect.width = rect.height * 0.5f;
            }

            hLineRect.y += rect.height * 0.5f;
            EditorGUI.DrawRect(hLineRect, LineColor);

            // 纵线
            var vLineRect = rect;
            vLineRect.width = 1;
            vLineRect.x -= rect.height * 0.5f;

            vLineRect.x -= rect.height - 2;
            vLineRect.height = IsLastChild(currentTreeViewItem) ? rect.height * 0.5f : rect.height;
            EditorGUI.DrawRect(vLineRect, LineColor);
            currentTreeViewItem = currentTreeViewItem.parent;
            vLineRect.height = rect.height;

            while (currentTreeViewItem != null)
            {
                vLineRect.x -= rect.height - 2;
                if (currentTreeViewItem.parent != null && !IsLastChild(currentTreeViewItem)) EditorGUI.DrawRect(vLineRect, LineColor);
                currentTreeViewItem = currentTreeViewItem.parent;
            }
        }

        private static bool IsLastChild(TreeViewItem t) => t.parent != null && t.parent.children.IndexOf(t) == t.parent.children.Count - 1;
    }
}