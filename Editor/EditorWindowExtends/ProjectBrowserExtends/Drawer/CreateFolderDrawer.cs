using UnityEditor;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;
using Object = UnityEngine.Object;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.Drawer
{
    public class CreateFolderDrawer : ProjectBrowserDrawer
    {
        public override string DrawerName => "Create Folder Button";
        public override int DefaultOrder => 1;

        public override void OnProjectBrowserGUI(AssetItem item)
        {
            if (!item.IsFolder || !item.IsHover)
                return;

            var content = EditorGUIUtility.IconContent(
                EditorGUIUtility.isProSkin ? "Folder On Icon" : "Folder Icon"
            );
            content.tooltip = "Create Folder";
            DrawIconButton(item, () => { CreateFolder(item.Asset); }, content);
        }


        private static void CreateFolder(Object asset, string defaultFolderName = "New Folder")
        {
            Selection.activeObject = asset;
            ProjectWindowUtilHandler.CreateFolderWithTemplates(defaultFolderName, null);
        }
    }
}