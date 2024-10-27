using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Yueby.EditorWindowExtends.HierarchyExtends;
using Yueby.EditorWindowExtends.HierarchyExtends.Core;
using Yueby.Utils;
using Yueby.Core.Utils;
using Logger = Yueby.Core.Utils.Logger;
using Yueby.EditorWindowExtends.Utils;
using Yueby.EditorWindowExtends.Core;

namespace Yueby.EditorWindowExtends.HierarchyExtends.Drawer
{
    public class ActiveDrawer : HierarchyDrawer
    {
        public override int DefaultOrder => 1;
        private DrawerIconContent _visibleIconContent = new(EditorGUIUtility.IconContent("animationvisibilitytoggleon@2x"), EditorGUIUtility.IconContent("animationvisibilitytoggleoff@2x"));
        // private DrawerIconContent _visibleHoverIconContent = new(EditorGUIUtility.IconContent("animationvisibilitytoggleon@2x"), EditorGUIUtility.IconContent("animationvisibilitytoggleoff@2x"));

        public override void OnHierarchyWindowItemGUI(SelectionItem selectionItem)
        {
            base.OnHierarchyWindowItemGUI(selectionItem);
            if (selectionItem.TargetObject == null) return;
            // if (!selectionItem.IsHover) return;
            // if (selectionItem.TargetObject.activeSelf)
            // {
            //     if (!selectionItem.IsHover) return;
            // }

            var height = EditorGUIUtility.singleLineHeight;
            DrawControl(selectionItem, height + 2, height, (rect) =>
            {
                rect.y += 1;

                rect.x += 2;
                rect.y -= 0.5f;

                var iconSize = EditorGUIUtility.GetIconSize();

                EditorGUI.BeginDisabledGroup(!selectionItem.TargetObject.activeSelf || !selectionItem.TargetObject.activeInHierarchy);
                EditorGUIUtility.SetIconSize(new Vector2(height - 2, height - 2));
                GUI.Label(rect, selectionItem.TargetObject.activeSelf ? _visibleIconContent.Light : _visibleIconContent.Dark);
                EditorGUIUtility.SetIconSize(iconSize);
                EditorGUI.EndDisabledGroup();

                if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                {
                    selectionItem.TargetObject.SetActive(!selectionItem.TargetObject.activeSelf);
                    Event.current.Use();
                }
            });
        }
    }
}