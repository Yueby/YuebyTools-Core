using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections;
using Yueby.EditorWindowExtends.Core;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends
{
    [InitializeOnLoad]
    public class LayerControllerViewExtender : EditorExtender<LayerControllerViewDrawer>
    {
        private const string MenuPath = BaseMenuPath + "Animator/" + nameof(LayerControllerViewExtender);
        private static LayerControllerViewExtender Extender;

        private EditorWindow _animatorControllerToolWindow;
        private int _lastIndex = -1;
        public bool IsFocusedOnWindow => EditorWindow.focusedWindow == AnimatorWindow;

        private EditorWindow AnimatorWindow
        {
            get
            {
                if (_animatorControllerToolWindow != null) return _animatorControllerToolWindow;

                var window = Resources.FindObjectsOfTypeAll(AnimatorWindowReflect.AnimatorWindowType);
                if (window.Length > 0)
                {
                    _animatorControllerToolWindow = window[0] as EditorWindow;
                }

                return _animatorControllerToolWindow;
            }
        }

        static LayerControllerViewExtender()
        {
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            Extender = new LayerControllerViewExtender();
            EditorApplication.update -= OnUpdate;
        }

        public LayerControllerViewExtender()
        {
            if (AnimatorWindow == null) return;

            var layerList = LayerControllerViewReflect.GetLayerReorderableList(AnimatorWindow);

            layerList.onAddCallback += OnAdd;
            layerList.onChangedCallback += OnChanged;
            layerList.onSelectCallback += OnSelect;
            layerList.onRemoveCallback += OnRemove;
            layerList.onMouseDragCallback += OnMouseDrag;
            layerList.onMouseUpCallback += OnMouseUp;

            layerList.drawElementCallback += OnDrawElement;
            layerList.drawElementBackgroundCallback += OnDrawElementBackground;


            foreach (var drawer in Drawers)
            {
                drawer.Init(layerList, this);
            }
        }

        private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (!IsEnable) return;

            foreach (var drawer in Drawers.Where(drawer => drawer.IsVisible))
            {
                drawer.OnDrawElement(rect, index, isactive, isfocused);
            }

            if (_lastIndex == index) return;
            _lastIndex = index;
            AnimatorWindow.Repaint();
        }

        private void OnDrawElementBackground(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (!IsEnable) return;

            foreach (var drawer in Drawers.Where(drawer => drawer.IsVisible))
            {
                drawer.OnDrawElementBackground(rect, index, isactive, isfocused);
            }
        }

        private void OnMouseUp(ReorderableList list)
        {
            if (!IsEnable) return;

            foreach (var drawer in Drawers.Where(drawer => drawer.IsVisible))
            {
                drawer.OnMouseUp(list);
            }
        }

        private void OnMouseDrag(ReorderableList list)
        {
            if (!IsEnable) return;

            foreach (var drawer in Drawers.Where(drawer => drawer.IsVisible))
            {
                drawer.OnMouseDrag(list);
            }
        }

        private void OnRemove(ReorderableList list)
        {
            if (!IsEnable) return;

            foreach (var drawer in Drawers.Where(drawer => drawer.IsVisible))
            {
                drawer.OnRemove(list);
            }
        }

        private void OnSelect(ReorderableList list)
        {
            if (!IsEnable) return;

            foreach (var drawer in Drawers.Where(drawer => drawer.IsVisible))
            {
                drawer.OnSelect(list);
            }
        }

        private void OnChanged(ReorderableList list)
        {
            if (!IsEnable) return;

            foreach (var drawer in Drawers.Where(drawer => drawer.IsVisible))
            {
                drawer.OnChanged(list);
            }
        }

        private void OnAdd(ReorderableList list)
        {
            if (!IsEnable) return;

            foreach (var drawer in Drawers.Where(drawer => drawer.IsVisible))
            {
                drawer.OnAdd(list);
            }
        }

        public override void Repaint()
        {
            base.Repaint();
            AnimatorWindow?.Repaint();
        }


        [MenuItem(MenuPath, false)]
        private static void ShowOptionWindow()
        {
            Extender.ShowOptions();
        }
    }
}