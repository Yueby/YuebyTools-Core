using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yueby.ProjectBrowserExtends.Core;

namespace Yueby.ProjectBrowserExtends
{
    [InitializeOnLoad]
    public class ProjectBrowserDrawer
    {
        private const string MENU_PATH = "Tools/YuebyTools/Project Browser Style Extends";
        private const string TAG = "EXTEND_PROJECT_WINDOW";
        public const float RIGHT_OFFSET = 2f;

        private static Dictionary<string, AssetItem> _assetItems;
        private static List<ProjectBrowserDrawerListener> _drawerListeners;

        private static EditorWindow _mouseOverWindow;
        private static string _lastHoveredGuid;
        private static bool _isDirty;

        static ProjectBrowserDrawer()
        {
            if (IsEnable())
            {
                EditorApplication.projectWindowItemOnGUI -= OnProjectBrowserItemGUI;
                EditorApplication.projectWindowItemOnGUI += OnProjectBrowserItemGUI;

                EditorApplication.update -= OnUpdate;
                EditorApplication.update += OnUpdate;
            }
        }

        [MenuItem(MENU_PATH)]
        private static void Execute()
        {
            if (IsEnable())
            {
                EditorPrefs.SetString(nameof(ProjectBrowserDrawer), TAG);
                EditorApplication.projectWindowItemOnGUI -= OnProjectBrowserItemGUI;
                EditorApplication.update -= OnUpdate;
            }
            else
            {
                EditorPrefs.SetString(nameof(ProjectBrowserDrawer), "");
                EditorApplication.projectWindowItemOnGUI -= OnProjectBrowserItemGUI;
                EditorApplication.projectWindowItemOnGUI += OnProjectBrowserItemGUI;

                EditorApplication.update -= OnUpdate;
                EditorApplication.update += OnUpdate;
            }

            EditorApplication.RepaintProjectWindow();
        }

        [MenuItem(MENU_PATH, true)]
        public static bool SettingValidate()
        {
            Menu.SetChecked(MENU_PATH, IsEnable());
            return true;
        }

        private static bool IsEnable()
        {
            return EditorPrefs.GetString(nameof(ProjectBrowserDrawer)) != TAG;
        }

        private static void OnUpdate()
        {
            _mouseOverWindow = EditorWindow.mouseOverWindow;
        }

        private static void OnProjectBrowserItemGUI(string guid, Rect rect)
        {
            if (_drawerListeners == null)
                return;

            SetDirty();

            if (
                _mouseOverWindow != null
                && _mouseOverWindow.GetType() == ProjectBrowserHandler.Type
                && _mouseOverWindow.wantsMouseMove == false
            )
                _mouseOverWindow.wantsMouseMove = true;

            var assetItem = GetAssetItem(guid, rect);

            bool needRepaint = false;

            if (assetItem.IsHover && _lastHoveredGuid != guid)
            {
                _lastHoveredGuid = guid;
                needRepaint = true;
            }

            foreach (var listener in _drawerListeners)
            {
                listener?.OnDraw?.Invoke(assetItem);
            }
            

            if (needRepaint && _mouseOverWindow != null)
            {
                // Debug.Log("Repaint Project Window");
                _mouseOverWindow.Repaint();
            }
        }

        private static AssetItem GetAssetItem(string guid, Rect rect)
        {
            _assetItems ??= new Dictionary<string, AssetItem>();
            if (_assetItems.TryGetValue(guid, out AssetItem assetItem))
            {
                assetItem.Refresh(guid, rect);
                return assetItem;
            }

            assetItem = new AssetItem(guid, rect);
            _assetItems.Add(guid, assetItem);
            return assetItem;
        }

        public static void Register(string id, Action<AssetItem> onDraw, int order = 0)
        {
            if (string.IsNullOrEmpty(id))
                throw new Exception("ID cannot be empty.");

            _drawerListeners ??= new List<ProjectBrowserDrawerListener>();

            var hash = id.GetHashCode();
            foreach (var listener in _drawerListeners)
            {
                if (listener.Id == id && listener.Hash == hash)
                {
                    listener.Refresh(onDraw, order);
                    // Debug.Log($"Already have Drawer: ${id}, refresh it.");
                    return;
                }
            }

            var drawerListener = new ProjectBrowserDrawerListener(id, hash, order, onDraw);
            _drawerListeners.Add(drawerListener);
            // Debug.Log($"Register Drawer: ${id}.");

            _isDirty = true;
        }

        private static void SetDirty()
        {
            if (_isDirty)
            {
                _drawerListeners.Sort(
                    (a, b) =>
                    {
                        if (a.Order == b.Order)
                            return 0;
                        if (a.Order > b.Order)
                            return 1;
                        return -1;
                    }
                );
                _isDirty = false;
            }
        }
    }

    public class ProjectBrowserDrawerListener
    {
        public int Hash { get; private set; }
        public string Id { get; private set; }
        public int Order { get; private set; }
        public Action<AssetItem> OnDraw;

        public ProjectBrowserDrawerListener(
            string id,
            int hash,
            int order,
            Action<AssetItem> onDraw
        )
        {
            Id = id;
            Hash = hash;
            Order = order;
            OnDraw = onDraw;
        }

        public void Refresh(Action<AssetItem> onDraw, int order)
        {
            OnDraw = onDraw;
            Order = order;
        }
    }
}
