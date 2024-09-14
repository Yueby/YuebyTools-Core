using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yueby.ProjectBrowserExtends.Core;

namespace Yueby.ProjectBrowserExtends.Drawer
{
    [InitializeOnLoad]
    public class HoverDrawer
    {
        static HoverDrawer()
        {
            ProjectBrowserDrawer.Register(nameof(HoverDrawer), OnDrawHover);
        }

        private static void OnDrawHover(AssetItem item)
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
