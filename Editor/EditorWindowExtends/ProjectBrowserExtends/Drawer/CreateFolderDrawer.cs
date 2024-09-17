using UnityEditor;
using UnityEngine;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;
using Object = UnityEngine.Object;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.Drawer
{
    public class CreateFolderDrawer : ProjectBrowserDrawer
    {
        public override string DrawerName => "Create Folder Button";

        public override void OnProjectBrowserGUI(AssetItem item)
        {
            if (!item.IsFolder || !item.IsHover)
                return;

            const int folderWidth = 24;
            var rect = item.Rect;
            rect.xMin = rect.xMax - folderWidth - ProjectBrowserExtender.RightOffset;
            rect.xMax -= ProjectBrowserExtender.RightOffset;
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