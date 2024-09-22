using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;

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


            // 横線
            var rectHorizLine = rect;
            rectHorizLine.height = 1;
            rectHorizLine.x -= rect.height * 0.5f;
            rectHorizLine.x -= rect.height - 2;

            rectHorizLine.width = currentTreeViewItem.children == null || currentTreeViewItem.children.Count > 0 ? rect.height * 0.5f : rect.height * 1.2f;
            rectHorizLine.y += rect.height * 0.5f;
            EditorGUI.DrawRect(rectHorizLine, LineColor);

            // 縦線
            var rectLine = rect;
            rectLine.width = 1;
            rectLine.x -= rect.height * 0.5f;

            rectLine.x -= rect.height - 2;
            rectLine.height = IsLastChild(currentTreeViewItem) ? rect.height * 0.5f : rect.height;
            EditorGUI.DrawRect(rectLine, LineColor);
            currentTreeViewItem = currentTreeViewItem.parent;
            rectLine.height = rect.height;

            while (currentTreeViewItem != null)
            {
                rectLine.x -= rect.height - 2;
                if (currentTreeViewItem.parent != null && !IsLastChild(currentTreeViewItem)) EditorGUI.DrawRect(rectLine, LineColor);
                currentTreeViewItem = currentTreeViewItem.parent;
            }
        }

        private static bool IsLastChild(TreeViewItem t) => t.parent != null && t.parent.children.IndexOf(t) == t.parent.children.Count - 1;
    }
}