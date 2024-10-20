using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Yueby.Core.Utils;
using Yueby.EditorWindowExtends.Core;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;
using Logger = Yueby.Core.Utils.Logger;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends
{
    [InitializeOnLoad]
    public sealed class ProjectBrowserExtender
        : EditorExtender<ProjectBrowserExtender, ProjectBrowserDrawer>
    {
        public override string Name => "ProjectWindow";

        public const float RightOffset = 2f;
        private Dictionary<string, AssetItem> _assetItems;

        private EditorWindow _mouseOverWindow;
        private string _lastHoveredGuid;
        private bool _isDirty;

        public static ProjectBrowserExtender Instance { get; private set; }

        static ProjectBrowserExtender()
        {
            Instance = new ProjectBrowserExtender();
        }

        public ProjectBrowserExtender()
        {
            SetEnable(IsEnabled);
        }

        public override void SetEnable(bool value)
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

            base.SetEnable(value);
        }

        private void OnUpdate()
        {
            _mouseOverWindow = EditorWindow.mouseOverWindow;
            // Debug.Log(_mouseOverWindow.GetType());

            // Logger.LogInfo(_mouseOverWindow.GetType().FullName);
        }

        public static void OnProjectBrowserObjectAreaItemGUI(int instanceID, Rect rect)
        {
            if (!Instance.IsEnabled)
                return;

            if (Instance is { ExtenderDrawers: null })
                return;

            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(instanceID));

            Instance.CheckRepaintAndDoGUI(
                guid,
                rect,
                (assetItem) =>
                {
                    foreach (
                        var drawer in Instance.ExtenderDrawers.Where(drawer =>
                            drawer is { IsVisible: true }
                        )
                    )
                    {
                        drawer.OnProjectBrowserObjectAreaItemBackgroundGUI(assetItem);
                        drawer.OnProjectBrowserObjectAreaItemGUI(assetItem);
                    }
                }
            );
        }

        public static void OnProjectBrowserTreeViewItemGUI(
            int instanceID,
            Rect rect,
            TreeViewItem item
        )
        {
            if (!Instance.IsEnabled)
                return;

            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(instanceID));
            if (Instance is { ExtenderDrawers: null })
                return;

            Instance.CheckRepaintAndDoGUI(
                guid,
                rect,
                (assetItem) =>
                {
                    foreach (
                        var drawer in Instance.ExtenderDrawers.Where(drawer =>
                            drawer is { IsVisible: true }
                        )
                    )
                    {
                        drawer.OnProjectBrowserTreeViewItemBackgroundGUI(assetItem, item);
                        drawer.OnProjectBrowserTreeViewItemGUI(assetItem, item);
                    }
                }
            );
        }

        private void OnProjectBrowserItemGUI(string guid, Rect rect)
        {
            CheckRepaintAndDoGUI(
                guid,
                rect,
                (assetItem) =>
                {
                    foreach (var drawer in ExtenderDrawers.Where(drawer => drawer is { IsVisible: true }))
                    {
                        drawer.OnProjectBrowserGUI(assetItem);
                    }
                }
            );
        }

        private void CheckRepaintAndDoGUI(string guid, Rect rect, Action<AssetItem> callback)
        {
            if (ExtenderDrawers == null)
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

        public void RemoveAssetItem(string guid)
        {
            if (_assetItems != null && _assetItems.ContainsKey(guid))
            {
                _assetItems.Remove(guid);
            }
        }

        public override void Repaint()
        {
            EditorApplication.RepaintProjectWindow();
        }
    }
}