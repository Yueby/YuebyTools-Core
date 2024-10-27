using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core;
using Yueby.EditorWindowExtends.Core;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends
{
    [InitializeOnLoad]
    public class LayerControllerViewExtender : EditorExtender<LayerControllerViewExtender, LayerControllerViewDrawer>
    {
        private static ReorderableList _lastList;

        private int _lastIndex = -1;

        static LayerControllerViewExtender()
        {
            AnimatorControllerToolHelper.OnAnimatorControllerToolState += OnAnimatorControllerToolState;
        }

        public LayerControllerViewExtender()
        {
            if (AnimatorControllerToolHelper.Window == null) return;

            _lastList = LayerControllerViewReflect.GetLayerReorderableList(AnimatorControllerToolHelper.Window);
            if (_lastList == null)
            {
                // Debug.LogWarning("Can't find layer list, try recreate extender.");
                Instance = null;
                return;
            }

            _lastList.onAddCallback += OnAdd;
            _lastList.onChangedCallback += OnChanged;
            _lastList.onSelectCallback += OnSelect;
            _lastList.onRemoveCallback += OnRemove;
            _lastList.onMouseDragCallback += OnMouseDrag;
            _lastList.onMouseUpCallback += OnMouseUp;

            _lastList.drawElementCallback += OnDrawElement;
            _lastList.drawElementBackgroundCallback += OnDrawElementBackground;

            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;

            foreach (var drawer in ExtenderDrawers) drawer.Init(this, _lastList);
        }

        private void OnUpdate()
        {
            foreach (var drawer in ExtenderDrawers.Where(drawer => drawer.IsVisible))
                drawer?.OnUpdate();
        }

        public override string Name => "Layer View";
        public static LayerControllerViewExtender Instance { get; private set; }
        public Vector2 ScrollPosition { get; private set; }

        private static void OnAnimatorControllerToolState(bool state)
        {
            if (state)
                // if (_lastList != ParameterControllerViewReflect.GetParameterReorderableList(AnimatorControllerToolHelper.Window))
                //     _extender = null;
                Instance ??= new LayerControllerViewExtender();
            else
                Instance = null;
        }

        private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (!IsEnabled) return;

            ScrollPosition = LayerControllerViewReflect.GetLayerScrollPosition(AnimatorControllerToolHelper.Window);

            foreach (var drawer in ExtenderDrawers.Where(drawer => drawer.IsVisible)) drawer.OnDrawElement(rect, index, isactive, isfocused);

            if (_lastIndex == index) return;
            _lastIndex = index;
            AnimatorControllerToolHelper.Window.Repaint();
        }

        private void OnDrawElementBackground(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (!IsEnabled) return;

            foreach (var drawer in ExtenderDrawers.Where(drawer => drawer.IsVisible)) drawer.OnDrawElementBackground(rect, index, isactive, isfocused);
        }

        private void OnMouseUp(ReorderableList list)
        {
            if (!IsEnabled) return;

            foreach (var drawer in ExtenderDrawers.Where(drawer => drawer.IsVisible)) drawer.OnMouseUp(list);
        }

        private void OnMouseDrag(ReorderableList list)
        {
            if (!IsEnabled) return;

            foreach (var drawer in ExtenderDrawers.Where(drawer => drawer.IsVisible)) drawer.OnMouseDrag(list);
        }

        private void OnRemove(ReorderableList list)
        {
            if (!IsEnabled) return;

            foreach (var drawer in ExtenderDrawers.Where(drawer => drawer.IsVisible)) drawer.OnRemove(list);
        }

        private void OnSelect(ReorderableList list)
        {
            if (!IsEnabled) return;

            foreach (var drawer in ExtenderDrawers.Where(drawer => drawer.IsVisible)) drawer.OnSelect(list);
        }

        private void OnChanged(ReorderableList list)
        {
            if (!IsEnabled) return;

            foreach (var drawer in ExtenderDrawers.Where(drawer => drawer.IsVisible)) drawer.OnChanged(list);
        }

        private void OnAdd(ReorderableList list)
        {
            if (!IsEnabled) return;

            foreach (var drawer in ExtenderDrawers.Where(drawer => drawer.IsVisible)) drawer.OnAdd(list);
        }

        public override void Repaint()
        {
            base.Repaint();
            AnimatorControllerToolHelper.Window?.Repaint();
        }
    }
}