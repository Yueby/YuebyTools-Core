using UnityEditor;
using System;
using UnityEngine;
using Yueby.Utils;
using UnityEditor.TerrainTools;

namespace Yueby.ModalWindow
{
    public class ModalEditorWindow : EditorWindow
    {
        private Action _onOk;
        private Action<bool> _onReturnValue;
        private IModalEditorWindowDrawer _drawer;

        private string _ok;
        private string _cancel;

        public bool IsHideCancel => _onReturnValue == null && _onOk == null;

        public static ModalEditorWindow Create(IModalEditorWindowDrawer drawer, Action onOk = null, string ok = "Ok", string cancel = "Cancel", bool useInEditorWindow = true)
        {
            var window = CreateWindow<ModalEditorWindow>();
            window.Init(drawer, onOk, ok, cancel, useInEditorWindow);
            return window;
        }

        public static ModalEditorWindow CreateReturnValue(IModalEditorWindowDrawer drawer, Action<bool> result = null, string ok = "Ok", string cancel = "Cancel", bool useInEditorWindow = true)
        {
            var window = CreateWindow<ModalEditorWindow>();
            window.InitReturnValue(drawer, result, ok, cancel, useInEditorWindow);
            return window;
        }

        public void Init(IModalEditorWindowDrawer drawer, Action onOk = null, string ok = "Ok", string cancel = "Cancel", bool useInEditorWindow = true)
        {
            _drawer = drawer;
            _onOk = onOk;
            _onReturnValue = null;
            _ok = ok;
            _cancel = cancel;

            titleContent = new GUIContent(text: _drawer.Title);

            OnChangeWindowSize(this);

            ShowModalUtility();
        }

        public void InitReturnValue(IModalEditorWindowDrawer drawer, Action<bool> result = null, string ok = "Ok", string cancel = "Cancel", bool useInEditorWindow = true)
        {
            _drawer = drawer;
            _onOk = null;
            _onReturnValue = result;
            _ok = ok;
            _cancel = cancel;

            titleContent = new GUIContent(text: _drawer.Title);

            OnChangeWindowSize(this);

            ShowModalUtility();
        }

        public static void Show(IModalEditorWindowDrawer drawer, Action onOk = null, string ok = "Ok", string cancel = "Cancel", bool useInEditorWindow = true)
        {
            var window = Create(drawer, onOk, ok, cancel, useInEditorWindow);
        }

        public static void ShowReturnValue(IModalEditorWindowDrawer drawer, Action<bool> result = null, string ok = "Ok", string cancel = "Cancel", bool useInEditorWindow = true)
        {
            CreateReturnValue(drawer, result, ok, cancel, useInEditorWindow);
        }

        private void OnChangeWindowSize(EditorWindow window)
        {
            var minSize = new Vector2(_drawer.Position.width, _drawer.Position.height + EditorGUIUtility.singleLineHeight + 2);
            window.minSize = minSize;
            window.maxSize = minSize;
            window.position = new Rect(Screen.currentResolution.width / 2 - minSize.x / 2, Screen.currentResolution.height / 2 - minSize.y / 2, minSize.x, minSize.y);
        }

        public static void ShowTip(string tip, string title = "Tips", Action onOk = null, string ok = "Ok", string cancel = "Cancel", MessageType messageType = MessageType.Info, bool useInEditorWindow = true)
        {

            Show(new TipsWindowDrawer(tip, title, messageType), onOk, ok, cancel, useInEditorWindow);
        }

        public static void ShowTipAndResult(string tip, string title = "Tips", Action<bool> onReturnValue = null, string ok = "Ok", string cancel = "Cancel", MessageType messageType = MessageType.Info, bool useInEditorWindow = true)
        {
            ShowReturnValue(new TipsWindowDrawer(tip, title, messageType), onReturnValue, ok, cancel, useInEditorWindow);
        }

        private void OnGUI()
        {
            _drawer.OnDraw();

            var bottomRect = new Rect(0, position.height - EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            var okRect = new Rect(bottomRect.x, bottomRect.y, IsHideCancel ? bottomRect.width : bottomRect.width / 2, bottomRect.height);
            var cancelRect = new Rect(okRect.x + okRect.width, bottomRect.y, bottomRect.width / 2, bottomRect.height);

            if (GUI.Button(okRect, _ok))
            {

                Close();

                _onOk?.Invoke();
                _onReturnValue?.Invoke(true);
                // EditorUtils.WaitToDo(1, "Wait to Exit GUI", GUIUtility.ExitGUI);
            }

            if (!IsHideCancel && GUI.Button(cancelRect, _cancel))
            {
                Close();
                _onReturnValue?.Invoke(false);

                // EditorUtils.WaitToDo(1, "Wait to Exit GUI", GUIUtility.ExitGUI);
            }
        }
    }

    // public class ModularWindow
    // {
    //     public static List<ModalEditorWindow> _windows = new List<ModalEditorWindow>();

    // }

    public interface IModalEditorWindowDrawer
    {
        string Title { get; }
        Rect Position { get; }

        void OnDraw();
    }
}