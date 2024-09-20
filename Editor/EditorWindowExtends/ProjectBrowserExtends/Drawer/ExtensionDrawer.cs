using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

#if UDONSHARP
using UdonSharp;
#endif

#if UDON
using VRC.Udon;
using VRC.Udon.Editor.ProgramSources.UdonGraphProgram.UI.GraphView;
#endif

#if !UDON && !UDONSHARP && VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.ScriptableObjects;
#endif

using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.Drawer
{
    public class ExtensionDrawer : ProjectBrowserDrawer
    {
        public override string DrawerName => "Extension Label";

        private static GUIStyle _style;

        private static Dictionary<string, string> _convertDict;

        public ExtensionDrawer()
        {
            _convertDict = new Dictionary<string, string> { { nameof(BlendTree), ".blendtree" } };

#if UDON
            _convertDict.Add(nameof(UdonBehaviour), ".udon");
            _convertDict.Add(nameof(UdonGraph), ".ugraph");
#endif

#if UDONSHARP
            _convertDict.Add(nameof(UdonSharpBehaviour), ".us");
            _convertDict.Add(nameof(UdonSharpProgramAsset), ".usasset");
#endif


#if !UDON && !UDONSHARP && VRC_SDK_VRCSDK3
            _convertDict.Add(nameof(VRCExpressionsMenu), ".menu");
            _convertDict.Add(nameof(VRCExpressionParameters), ".parameters");
#endif
        }


        public override void OnProjectBrowserGUI(AssetItem item)
        {
            if (item.IsFolder || item.Asset == null) return;

            var extension = Path.GetExtension(item.Path);

            if (_convertDict.TryGetValue(item.Asset.GetType().Name, out var ext))
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