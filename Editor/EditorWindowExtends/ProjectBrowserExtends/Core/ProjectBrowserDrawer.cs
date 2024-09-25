using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Yueby.EditorWindowExtends.Core;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.Core
{
    public class ProjectBrowserDrawer : EditorExtenderDrawer<ProjectBrowserExtender, ProjectBrowserDrawer>
    {
        public virtual void OnProjectBrowserGUI(AssetItem item)
        {
        }

        public virtual void OnProjectBrowserTreeViewItemGUI(AssetItem item, TreeViewItem treeViewItem)
        {
        }

        public virtual void OnProjectBrowserTreeViewItemBackgroundGUI(AssetItem item, TreeViewItem treeViewItem)
        {
        }

        public virtual void OnProjectBrowserObjectAreaItemGUI(AssetItem item)
        {
        }

        public virtual void OnProjectBrowserObjectAreaItemBackgroundGUI(AssetItem item)
        {
        }

        protected void DrawIconButton(AssetItem item, Action onClick, GUIContent content)
        {
            const int folderWidth = 24;
            var rect = item.Rect;

            rect.height = EditorGUIUtility.singleLineHeight - 2;
            rect.xMin = rect.xMax - folderWidth - ProjectBrowserExtender.RightOffset;
            if (item.OriginRect.height < EditorGUIUtility.singleLineHeight)
            {
                rect.xMax -= ProjectBrowserExtender.RightOffset;
                item.Rect.xMax -= folderWidth;
            }
            else
            {
                item.Rect.yMin += rect.height;
            }


            var iconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(14, 14));

            if (GUI.Button(rect, content))
                onClick?.Invoke();

            EditorGUIUtility.SetIconSize(iconSize);
        }
    }
}