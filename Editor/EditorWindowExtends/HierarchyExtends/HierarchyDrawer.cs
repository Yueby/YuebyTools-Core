using System;
using UnityEditor;
using UnityEngine;
using Yueby.EditorWindowExtends.Core;
using Yueby.EditorWindowExtends.HierarchyExtends.Core;
using Yueby.EditorWindowExtends.Utils;

namespace Yueby.EditorWindowExtends.HierarchyExtends
{
    public class HierarchyDrawer : EditorExtenderDrawer<HierarchyExtender, HierarchyDrawer>
    {
        public virtual void OnHierarchyWindowItemGUI(SelectionItem selectionItem)
        {

        }

        public virtual void OnHierarchyChanged()
        {

        }

        protected void DrawControl(SelectionItem item, float width, float height, Action<Rect> onClick)
        {
            var rect = item.Rect;

            rect.y -= 1;

            rect.xMin = rect.xMax - width;
            item.Rect.xMax -= width;
            // var iconSize = EditorGUIUtility.GetIconSize();
            // EditorGUIUtility.SetIconSize(new Vector2(12, 12));

            // EditorGUI.DrawRect(rect, Styles.HierarchyBackgroundColor.GetColor());
            // if (GUI.Button(rect, content))
            onClick?.Invoke(rect);

            // EditorGUIUtility.SetIconSize(iconSize);
        }
    }
}