using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;
using Object = UnityEngine.Object;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.Drawer
{
    public class ExtensionDrawer : ProjectBrowserDrawer
    {
        public override string DrawerName { get; } = "Extension Label";

        private static GUIStyle _style;

        private static readonly Dictionary<string, string> ConvertDict = new()
        {
            { nameof(BlendTree), ".blendtree" },
            { nameof(VRCExpressionsMenu), ".menu" },
            { nameof(VRCExpressionParameters), ".parameters" },
        };

        public override void OnProjectBrowserGUI(AssetItem item)
        {
            if (item.IsFolder)
                return;

            if (item.Asset != null)
            {
                var extension = Path.GetExtension(item.Path);

                if (ConvertDict.TryGetValue(item.Asset.GetType().Name, out var ext))
                    extension = ext;

                _style ??= new GUIStyle(EditorStyles.label);
                var extensionContent = new GUIContent(extension);
                var size = _style.CalcSize(extensionContent);

                var rect = item.Rect;
                rect.xMin = rect.xMax - size.x - ProjectBrowserExtender.RightOffset;
                rect.xMax -= ProjectBrowserExtender.RightOffset;
                rect.height = EditorGUIUtility.singleLineHeight;

                item.Rect.xMax -= size.x - ProjectBrowserExtender.RightOffset;

                var badgeRect = new Rect(rect.x, rect.y, rect.width, rect.height - 2);

                GUI.Box(badgeRect, new GUIContent(""), "Badge");
                EditorGUI.LabelField(rect, extensionContent);
            }
        }

    }
}