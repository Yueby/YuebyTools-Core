using System;
using UnityEditor;
using UnityEngine;

namespace Editor
{


}
namespace Yueby.ModalWindow
{
    public class ModalEditorWindow : EditorWindow
    {
        private Action _onOk;
        private Action<bool> _onReturnValue;
        private IModalEditorWindowDrawer _drawer;

        private string _ok;
        private string _cancel;

        private bool IsHideCancel => _onReturnValue == null && _onOk == null;

        private void OnEnable()
        {
            _drawer?.OnEnable();
        }

        private void OnDisable()
        {
            _drawer?.OnDisable();
        }

        public static void Create(
            IModalEditorWindowDrawer drawer,
            Action onOk = null,
            string ok = "Ok",
            string cancel = "Cancel",
            bool showFocusCenter = true
        )
        {
            var focusedWindowPosition =
                focusedWindow == null ? new Rect(0, 0, 0, 0) : focusedWindow.position;
            var window = CreateWindow<ModalEditorWindow>();
            if (showFocusCenter)
                window.Init(focusedWindowPosition, drawer, onOk, ok, cancel);
            else
                window.Init(drawer, onOk, ok, cancel);
            window.ShowModalUtility();
        }

        public static void Create(
            IModalEditorWindowDrawer drawer,
            Action<bool> result,
            string ok = "Ok",
            string cancel = "Cancel",
            bool showFocusCenter = true
        )
        {
            var focusedWindowPosition =
                focusedWindow == null ? new Rect(0, 0, 0, 0) : focusedWindow.position;
            var window = CreateWindow<ModalEditorWindow>();
            if (showFocusCenter)
                window.Init(focusedWindowPosition, drawer, result, ok, cancel);
            else
                window.Init(drawer, result, ok, cancel);
            window.ShowModalUtility();
        }

        private static void CreateUtility(
            IModalEditorWindowDrawer drawer,
            Action onOk,
            string ok,
            string cancel,
            bool showFocusCenter = true
        )
        {
            var focusedWindowPosition =
                focusedWindow == null ? new Rect(0, 0, 0, 0) : focusedWindow.position;
            var window = CreateWindow<ModalEditorWindow>();
            if (showFocusCenter)
                window.Init(focusedWindowPosition, drawer, onOk, ok, cancel);
            else
                window.Init(drawer, onOk, ok, cancel);
            window.ShowUtility();
        }

        private void Init(
            IModalEditorWindowDrawer drawer,
            Action onOk = null,
            string ok = "Ok",
            string cancel = "Cancel"
        )
        {
            _drawer = drawer;
            _onOk = onOk;
            _onReturnValue = null;
            _ok = ok;
            _cancel = cancel;

            titleContent = new GUIContent(text: _drawer.Title);

            OnChangeWindowSize(this);
        }

        public void Init(
            IModalEditorWindowDrawer drawer,
            Action<bool> result,
            string ok = "Ok",
            string cancel = "Cancel"
        )
        {
            _drawer = drawer;
            _onOk = null;
            _onReturnValue = result;
            _ok = ok;
            _cancel = cancel;

            titleContent = new GUIContent(text: _drawer.Title);

            OnChangeWindowSize(this);
        }

        private void Init(
            Rect focusedWindowPosition,
            IModalEditorWindowDrawer drawer,
            Action onOk = null,
            string ok = "Ok",
            string cancel = "Cancel"
        )
        {
            _drawer = drawer;
            _onOk = onOk;
            _onReturnValue = null;
            _ok = ok;
            _cancel = cancel;

            titleContent = new GUIContent(text: _drawer.Title);

            OnChangeWindowSize(this, focusedWindowPosition);
        }

        public void Init(
            Rect focusedWindowPosition,
            IModalEditorWindowDrawer drawer,
            Action<bool> result,
            string ok = "Ok",
            string cancel = "Cancel"
        )
        {
            _drawer = drawer;
            _onOk = null;
            _onReturnValue = result;
            _ok = ok;
            _cancel = cancel;

            titleContent = new GUIContent(text: _drawer.Title);

            OnChangeWindowSize(this, focusedWindowPosition);
        }

        public static void Show(
            IModalEditorWindowDrawer drawer,
            Action onOk = null,
            string ok = "Ok",
            string cancel = "Cancel",
            bool showFocusCenter = true
        )
        {
            Create(drawer, onOk, ok, cancel, showFocusCenter);
        }

        public static void Show(
            IModalEditorWindowDrawer drawer,
            Action<bool> result,
            string ok = "Ok",
            string cancel = "Cancel",
            bool showFocusCenter = true
        )
        {
            Create(drawer, result, ok, cancel, showFocusCenter);
        }

        public static void ShowUtility(
            IModalEditorWindowDrawer drawer,
            Action onOk = null,
            string ok = "Ok",
            string cancel = "Cancel",
            bool showFocusCenter = true
        )
        {
            CreateUtility(drawer, onOk, ok, cancel, showFocusCenter);
        }

        public static void ShowTip(
            string tip,
            string title = "Tips",
            Action onOk = null,
            string ok = "Ok",
            string cancel = "Cancel",
            MessageType messageType = MessageType.Info,
            bool showFocusCenter = true
        )
        {
            Show(new TipsWindowDrawer(tip, title, messageType), onOk, ok, cancel, showFocusCenter);
        }

        private void OnChangeWindowSize(EditorWindow window)
        {
            var size = new Vector2(
                _drawer.Position.width,
                _drawer.Position.height + EditorGUIUtility.singleLineHeight + 2f
            );
            window.minSize = size;
            window.maxSize = size;

            var rect = new Rect(
                Screen.currentResolution.width / 2f - size.x / 2f,
                Screen.currentResolution.height / 2f - size.y / 2f,
                size.x,
                size.y
            );

            // Debug.Log(rect);
            window.position = rect;
        }

        private void OnChangeWindowSize(EditorWindow window, Rect focusedWindowPosition)
        {
            var size = new Vector2(
                _drawer.Position.width,
                _drawer.Position.height + EditorGUIUtility.singleLineHeight + 2f
            );
            window.minSize = size;
            window.maxSize = size;

            var rect = new Rect(
                focusedWindowPosition.x + focusedWindowPosition.width / 2f - size.x / 2f,
                focusedWindowPosition.y + focusedWindowPosition.height / 2f - size.y / 2f,
                size.x,
                size.y
            );
            // Debug.Log(rect);

            window.position = rect;
        }

        private void OnGUI()
        {
            if (_drawer == null)
            {
                Close();
                return;
            }

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

        void OnEnable();
        void OnDisable();

        void OnDraw();
    }
}