using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using Yueby.ProjectBrowserExtends.Core;
using Object = UnityEngine.Object;

namespace Yueby.ProjectBrowserExtends.Drawer
{
    [InitializeOnLoad]
    public class ExtensionDrawer
    {
        private static GUIStyle _style;

        private static readonly Dictionary<string, string> _convertDict = new Dictionary<
            string,
            string
        >
        {
            { nameof(BlendTree), ".blendtree" },
            { nameof(VRCExpressionsMenu), ".menu" },
            { nameof(VRCExpressionParameters), ".parameters" },
        };

        static ExtensionDrawer()
        {
            ProjectBrowserDrawer.Register(nameof(ExtensionDrawer), OnDrawExtension);
        }

        private static void OnDrawExtension(AssetItem item)
        {
            if (item.IsFolder)
                return;

            if (item.Asset != null)
            {
                var extension = Path.GetExtension(item.Path);

                if (_convertDict.TryGetValue(item.Asset.GetType().Name, out var ext))
                    extension = ext;

                _style ??= new GUIStyle(EditorStyles.label);
                var extensionContent = new GUIContent(extension);
                var size = _style.CalcSize(extensionContent);

                var labelRect = item.Rect;
                labelRect.xMin = labelRect.xMax - size.x;
                labelRect.height = EditorGUIUtility.singleLineHeight;
                item.Rect.xMax -= size.x;

                var badgeRect = new Rect(
                    labelRect.x,
                    labelRect.y,
                    labelRect.width,
                    labelRect.height - 2
                );

                GUI.Box(badgeRect, new GUIContent(""), "Badge");
                EditorGUI.LabelField(labelRect, extensionContent);
            }
        }
    }
}
