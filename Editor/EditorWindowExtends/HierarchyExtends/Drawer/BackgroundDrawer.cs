using UnityEditor;
using UnityEngine;
using Yueby.Core.Utils;
using Yueby.EditorWindowExtends.HierarchyExtends.Core;
using Yueby.EditorWindowExtends.Utils;
using Logger = Yueby.Core.Utils.Logger;

namespace Yueby.EditorWindowExtends.HierarchyExtends.Drawer
{
    public class BackgroundDrawer : HierarchyDrawer
    {
        public override string DrawerName => "Background";
        public override int DefaultOrder => 0;

        public override void OnHierarchyWindowItemGUI(SelectionItem selectionItem)
        {
            base.OnHierarchyWindowItemGUI(selectionItem);

            if (selectionItem.OriginRect.height <= EditorGUIUtility.singleLineHeight)
            {
                var selectionRect = selectionItem.OriginRect;

                selectionRect.width += selectionRect.x;
                selectionRect.x = 0;
                // selectionRect.height -= 1;
                // selectionRect.y += 1;
                EditorGUI.DrawRect(selectionRect, (Mathf.FloorToInt((selectionRect.y - 4) / 16 % 2) == 0) ? Styles.EvenShadingColor.GetColor() : Styles.OddShadingColor.GetColor());
            }
        }
    }
}