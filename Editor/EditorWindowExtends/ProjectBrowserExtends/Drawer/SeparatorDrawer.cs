using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Yueby.EditorWindowExtends.Core;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;
using Yueby.EditorWindowExtends.Utils;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.Drawer
{
    public class SeparatorDrawer : ProjectBrowserDrawer
    {
        public override string DrawerName => "Separator";

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
            if (item.OriginRect.height <= EditorGUIUtility.singleLineHeight)
            {
                var rect = new Rect(item.OriginRect.x, item.OriginRect.y + item.OriginRect.height, item.OriginRect.width + item.OriginRect.x, 1);
                EditorGUI.DrawRect(rect, Styles.SeparatorColor.GetColor());

                var selectionRect = item.OriginRect;

                selectionRect.width += selectionRect.x;
                selectionRect.x = 0;
                selectionRect.height -= 1;
                selectionRect.y += 1;
                EditorGUI.DrawRect(selectionRect, (Mathf.FloorToInt((selectionRect.y - 4) / 16 % 2) == 0) ? Styles.EvenShadingColor.GetColor() : Styles.OddShadingColor.GetColor());
            }

            // var originRect = item.OriginRect;
            // if (originRect.height > EditorGUIUtility.singleLineHeight) return;
            // if ((int)originRect.y % (int)(originRect.height * 2) >= (int)originRect.height)
            //     EditorGUI.DrawRect(originRect, _backgroundColor);
        }
    }
}