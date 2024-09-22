using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.Drawer
{
    public class BackgroundDrawer : ProjectBrowserDrawer
    {
        public override string DrawerName => "Alternating Background";
        private readonly Color _backgroundColor = new(0.5f, 0.5f, 0.5f, 0.05f);

        public override void OnProjectBrowserObjectAreaItemBackgroundGUI(AssetItem item)
        {
            DrawBackground(item);
        }

        public override void OnProjectBrowserTreeViewItemBackgroundGUI(AssetItem item, TreeViewItem treeViewItem)
        {
            DrawBackground(item);
        }

        private void DrawBackground(AssetItem item)
        {
            var originRect = item.OriginRect;
            if (originRect.height > EditorGUIUtility.singleLineHeight) return;
            if ((int)originRect.y % (int)(originRect.height * 2) >= (int)originRect.height)
                EditorGUI.DrawRect(originRect, _backgroundColor);
        }
    }
}