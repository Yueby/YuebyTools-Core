using UnityEditor;
using UnityEngine;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.Drawer
{
    public class HoverDrawer : ProjectBrowserDrawer
    {
        public override string DrawerName => "Hover Line";

        public override void OnProjectBrowserGUI(AssetItem item)
        {
            if (item.IsHover && Selection.activeObject != item.Asset)
            {
                var rect = item.OriginRect;

                rect.y += rect.height;
                rect.height = 1;
                EditorGUI.DrawRect(rect, Color.gray);
            }
        }

    
    }
}