using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core;
using Yueby.EditorWindowExtends.Core;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.ModalWindow;
using Yueby.ModalWindow;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends
{
    [InitializeOnLoad]
    public sealed class ProjectBrowserExtender : EditorExtender<ProjectBrowserDrawer>
    {
        private const string MenuPath = BaseMenuPath + nameof(ProjectBrowserDrawer);
        public const float RightOffset = 2f;
        private Dictionary<string, AssetItem> _assetItems;


        private EditorWindow _mouseOverWindow;
        private string _lastHoveredGuid;
        private bool _isDirty;

        private static readonly ProjectBrowserExtender Extender;


        static ProjectBrowserExtender()
        {
            Extender = new ProjectBrowserExtender();
        }

        public ProjectBrowserExtender()
        {
            SetExtenderEnable(IsEnable);
        }

        [MenuItem(MenuPath, false)]
        private static void ShowOptionWindow()
        {
            Extender.ShowOptions();
        }


        public override void SetExtenderEnable(bool value)
        {
            if (!value)
            {
                EditorApplication.projectWindowItemOnGUI -= OnProjectBrowserItemGUI;
                EditorApplication.update -= OnUpdate;
            }
            else
            {
                EditorApplication.projectWindowItemOnGUI -= OnProjectBrowserItemGUI;
                EditorApplication.projectWindowItemOnGUI += OnProjectBrowserItemGUI;

                EditorApplication.update -= OnUpdate;
                EditorApplication.update += OnUpdate;
            }

            base.SetExtenderEnable(value);
        }


        private void OnUpdate()
        {
            _mouseOverWindow = EditorWindow.mouseOverWindow;
            // Debug.Log(_mouseOverWindow.GetType());
        }

        private void OnProjectBrowserItemGUI(string guid, Rect rect)
        {
            if (Drawers == null)
                return;

            // SetDirty();

            if (_mouseOverWindow != null && _mouseOverWindow.GetType() == ProjectBrowserReflect.Type && _mouseOverWindow.wantsMouseMove == false)
                _mouseOverWindow.wantsMouseMove = true;

            var needRepaint = false;
            var assetItem = GetAssetItem(guid, rect);

            if (assetItem.IsHover && _lastHoveredGuid != guid)
            {
                _lastHoveredGuid = guid;
                needRepaint = true;
            }

            foreach (var drawer in Drawers.Where(drawer => drawer is { IsVisible: true }))
            {
                drawer.OnProjectBrowserGUI(assetItem);
            }

            if (needRepaint && _mouseOverWindow != null)
                _mouseOverWindow.Repaint();
        }

        private AssetItem GetAssetItem(string guid, Rect rect)
        {
            _assetItems ??= new Dictionary<string, AssetItem>();
            if (_assetItems.TryGetValue(guid, out var assetItem))
            {
                assetItem.Refresh(guid, rect);
                return assetItem;
            }

            assetItem = new AssetItem(guid, rect);
            _assetItems.Add(guid, assetItem);
            return assetItem;
        }

        public override void Repaint()
        {
            EditorApplication.RepaintProjectWindow();
        }
    }
}