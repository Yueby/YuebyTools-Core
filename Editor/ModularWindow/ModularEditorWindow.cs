using UnityEditor;
using System;
using UnityEngine;
using Yueby.Utils;

namespace Yueby.PackageImporter
{
    public class ModularEditorWindow : EditorWindow
    {
        private static Action _onOk;
        private static IModularEditorWindowDrawer _drawer;
        private static ModularEditorWindow _window;

        public static void ShowWindow(IModularEditorWindowDrawer drawer, Action onOk = null)
        {
            _drawer = drawer;
            _onOk = onOk;

            _window = GetWindow<ModularEditorWindow>();
            _window.titleContent = new GUIContent(text: _drawer.Title);

            var minSize = new Vector2(_drawer.Position.width, _drawer.Position.height + EditorGUIUtility.singleLineHeight + 2);
            _window.minSize = minSize;
            _window.maxSize = minSize;
            // position is screen center
            _window.position = new Rect(Screen.currentResolution.width / 2 - minSize.x / 2, Screen.currentResolution.height / 2 - minSize.y / 2, minSize.x, minSize.y);
            // Debug.Log(_window.position);
            _window.ShowModal();

        }

        private void OnGUI()
        {
            _drawer.OnDraw();

            var bottomRect = new Rect(0, _window.position.height - EditorGUIUtility.singleLineHeight, _window.position.width, EditorGUIUtility.singleLineHeight);
            var okRect = new Rect(bottomRect.x, bottomRect.y, bottomRect.width / 2, bottomRect.height);
            var cancelRect = new Rect(okRect.x + okRect.width, bottomRect.y, bottomRect.width / 2, bottomRect.height);

            if (GUI.Button(okRect, "Ok"))
            {
                _onOk?.Invoke();
                Close();
            }

            if (_onOk != null && GUI.Button(cancelRect, "Cancel"))
            {
                Close();
            }
        }

    }

    public interface IModularEditorWindowDrawer
    {
        string Title { get; }
        Rect Position { get; }
        
        void OnDraw();
    }
}