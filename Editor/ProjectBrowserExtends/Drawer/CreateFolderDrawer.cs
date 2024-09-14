using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Yueby.ProjectBrowserExtends.Core;
using Object = UnityEngine.Object;

namespace Yueby.ProjectBrowserExtends.Drawer
{
    [InitializeOnLoad]
    public class CreateFolderDrawer
    {
        static CreateFolderDrawer()
        {
            ProjectBrowserDrawer.Register(nameof(CreateFolderDrawer), OnDrawCreateFolder);
        }

        public static void OnDrawCreateFolder(AssetItem item)
        {
            if (!item.IsFolder || !item.IsHover)
                return;

            float folderWidth = 24;
            Rect rect = item.Rect;
            rect.xMin = rect.xMax - folderWidth - ProjectBrowserDrawer.RIGHT_OFFSET;
            rect.xMax -= ProjectBrowserDrawer.RIGHT_OFFSET;
            rect.height = EditorGUIUtility.singleLineHeight - 2;
            item.Rect.xMax -= folderWidth;

            var content = EditorGUIUtility.IconContent(
                EditorGUIUtility.isProSkin ? "Folder On Icon" : "Folder Icon"
            );

            var iconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(14, 14));

            if (GUI.Button(rect, content))
                CreateFolder(item.Asset);

            EditorGUIUtility.SetIconSize(iconSize);
        }

        private static void CreateFolder(Object asset, string defaultFolderName = "New Folder")
        {
            Selection.activeObject = asset;
            ProjectWindowUtilHandler.CreateFolderWithTemplates(defaultFolderName, null);
        }
    }
}
