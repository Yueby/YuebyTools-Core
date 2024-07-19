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

        private bool IsHideCancel => _onReturnValue == null && _onOk == null;

        private void OnEnable()
        {
            _drawer?.OnEnable();
        }

        private void OnDisable()
        {
            _drawer?.OnDisable();
        }

        public static void Create(IModalEditorWindowDrawer drawer, Action onOk = null, string ok = "Ok", string cancel = "Cancel")
        {
            var window = CreateWindow<ModalEditorWindow>();
            window.Init(drawer, onOk, ok, cancel);
            window.ShowModalUtility();
        }

        public static void Create(IModalEditorWindowDrawer drawer, Action<bool> result, string ok = "Ok", string cancel = "Cancel")
        {
            var window = CreateWindow<ModalEditorWindow>();
            window.Init(drawer, result, ok, cancel);
            window.ShowModalUtility();
        }

        private static void CreateUtility(IModalEditorWindowDrawer drawer, Action onOk, string ok, string cancel)
        {
            var window = CreateWindow<ModalEditorWindow>();
            window.Init(drawer, onOk, ok, cancel);
            window.ShowUtility();
        }

        private void Init(IModalEditorWindowDrawer drawer, Action onOk = null, string ok = "Ok", string cancel = "Cancel")
        {
            _drawer = drawer;
            _onOk = onOk;
            _onReturnValue = null;
            _ok = ok;
            _cancel = cancel;

            titleContent = new GUIContent(text: _drawer.Title);

            OnChangeWindowSize(this);
        }


        public void Init(IModalEditorWindowDrawer drawer, Action<bool> result, string ok = "Ok", string cancel = "Cancel")
        {
            _drawer = drawer;
            _onOk = null;
            _onReturnValue = result;
            _ok = ok;
            _cancel = cancel;

            titleContent = new GUIContent(text: _drawer.Title);

            OnChangeWindowSize(this);
        }

        public static void Show(IModalEditorWindowDrawer drawer, Action onOk = null, string ok = "Ok", string cancel = "Cancel")
        {
            Create(drawer, onOk, ok, cancel);
        }

        public static void Show(IModalEditorWindowDrawer drawer, Action<bool> result, string ok = "Ok", string cancel = "Cancel")
        {
            Create(drawer, result, ok, cancel);
        }

        public static void ShowUtility(IModalEditorWindowDrawer drawer, Action onOk = null, string ok = "Ok", string cancel = "Cancel")
        {
            CreateUtility(drawer, onOk, ok, cancel);
        }


        private void OnChangeWindowSize(EditorWindow window)
        {
            var size = new Vector2(_drawer.Position.width, _drawer.Position.height + EditorGUIUtility.singleLineHeight + 2f);
            window.minSize = size;
            window.maxSize = size;
            window.position = new Rect(Screen.currentResolution.width / 2f - size.x / 2f, Screen.currentResolution.height / 2f - size.y / 2f, size.x, size.y);
        }

        public static void ShowTip(string tip, string title = "Tips", Action onOk = null, string ok = "Ok", string cancel = "Cancel", MessageType messageType = MessageType.Info)
        {
            Show(new TipsWindowDrawer(tip, title, messageType), onOk, ok, cancel);
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

        void OnEnable();
        void OnDisable();

        void OnDraw();
    }
}