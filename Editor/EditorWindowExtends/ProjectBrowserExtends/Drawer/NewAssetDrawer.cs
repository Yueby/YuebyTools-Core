using UnityEditor;
using UnityEngine;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.Drawer
{
    public class NewAssetDrawer : ProjectBrowserDrawer
    {
        public override string DrawerName => "New Asset Dot";
        public override string Tooltip => "Create a new asset in the project browser";
        public override int DefaultOrder => 2;

        public override void OnProjectBrowserGUI(AssetItem item)
        {
            if (item.Asset == null)
                return;

            item.ProjectBrowserAsset ??= AssetListener.Root.FindByGuid(item.Guid);

            if (item.ProjectBrowserAsset is not { HasNewAsset: true })
                return;

            const float dotRadius = 8f;

            var dotRect = new Rect(
                item.OriginRect.x - dotRadius * 0.5f,
                item.OriginRect.y - dotRadius,
                dotRadius * 2f,
                dotRadius * 2f
            );
            var dotStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.green }
            };

            GUI.Label(dotRect, new GUIContent("●"), dotStyle);

            if (!item.IsHover)
                return;

            if (item.ProjectBrowserAsset.IsNewAsset && Event.current.type == EventType.MouseDown) item.ProjectBrowserAsset.SetNewAsset(false);

            if (!item.IsFolder) return;
            var content = EditorGUIUtility.IconContent(
                EditorGUIUtility.isProSkin ? "d_Package Manager" : "Package Manager"
            );
            content.tooltip = "Clear New Asset Dot";
            DrawIconButton(item, () => { ClearDot(item.ProjectBrowserAsset); }, content);
        }

        private void ClearDot(ProjectBrowserAsset asset)
        {
            AssetListener.ClearAsset(asset);
        }
    }
}