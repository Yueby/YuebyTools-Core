using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Yueby.EditorWindowExtends.Core;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;
using Object = UnityEngine.Object;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends
{
    [InitializeOnLoad]
    public sealed class ProjectBrowserExtender
        : EditorExtender<ProjectBrowserExtender, ProjectBrowserDrawer>
    {
        private const string MenuPath = BaseMenuPath + nameof(ProjectBrowserExtender);
        public const float RightOffset = 2f;
        private Dictionary<string, AssetItem> _assetItems;

        private EditorWindow _mouseOverWindow;
        private string _lastHoveredGuid;
        private bool _isDirty;

        public static readonly ProjectBrowserExtender Extender;

        static ProjectBrowserExtender()
        {
            // var method= ProjectBrowserReflect.Type.GetMethod("OnGUIAssetCallback",ReflectionHelper.InstanceLookup);
            //
            // var harmony = new Harmony()
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

        public static void OnProjectBrowserObjectAreaItemGUI(int instanceID, Rect rect)
        {
            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(instanceID));
            if (Extender is { Drawers: null }) return;

            Extender.CheckRepaintAndDoGUI(guid, rect, (assetItem) =>
            {
                foreach (var drawer in Extender.Drawers.Where(drawer => drawer is { IsVisible: true }))
                {
                    drawer.OnProjectBrowserObjectAreaItemBackgroundGUI(assetItem);
                    drawer.OnProjectBrowserObjectAreaItemGUI(assetItem);
                }
            });
        }

        public static void OnProjectBrowserTreeViewItemGUI(int instanceID, Rect rect, TreeViewItem item)
        {
            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(instanceID));
            if (Extender is { Drawers: null }) return;

            Extender.CheckRepaintAndDoGUI(guid, rect, (assetItem) =>
            {
                foreach (var drawer in Extender.Drawers.Where(drawer => drawer is { IsVisible: true }))
                {
                    drawer.OnProjectBrowserTreeViewItemBackgroundGUI(assetItem, item);
                    drawer.OnProjectBrowserTreeViewItemGUI(assetItem, item);
                }
            });
        }


        private void OnProjectBrowserItemGUI(string guid, Rect rect)
        {
            CheckRepaintAndDoGUI(guid, rect, (assetItem) =>
            {
                foreach (var drawer in Drawers.Where(drawer => drawer is { IsVisible: true }))
                {
                    drawer.OnProjectBrowserGUI(assetItem);
                }
            });
        }

        private void CheckRepaintAndDoGUI(string guid, Rect rect, Action<AssetItem> callback)
        {
            if (Drawers == null)
                return;

            // SetDirty();

            if (
                _mouseOverWindow != null
                && _mouseOverWindow.GetType() == ProjectBrowserReflect.Type
                && _mouseOverWindow.wantsMouseMove == false
            )
                _mouseOverWindow.wantsMouseMove = true;

            var needRepaint = false;
            var assetItem = GetAssetItem(guid, rect);

            if (assetItem.IsHover && _lastHoveredGuid != guid)
            {
                _lastHoveredGuid = guid;
                needRepaint = true;
            }

            callback?.Invoke(assetItem);

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