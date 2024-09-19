using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections;
using Yueby.EditorWindowExtends.Core;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends
{
    [InitializeOnLoad]
    public class ParameterControllerViewExtender : EditorExtender<ParameterControllerViewExtender, ParameterControllerViewDrawer>
    {
        private const string MenuPath = BaseMenuPath + "Animator/" + nameof(ParameterControllerViewExtender);
        private static ParameterControllerViewExtender _extender;
        private static ReorderableList _lastList;

        private int _lastIndex = -1;
        public Vector2 ScrollPosition { get; private set; }


        static ParameterControllerViewExtender()
        {
            AnimatorControllerToolHelper.OnAnimatorControllerToolState += OnAnimatorControllerToolState;
        }

        private static void OnAnimatorControllerToolState(bool state)
        {
            if (state)
            {
                if (_lastList != ParameterControllerViewReflect.GetParameterReorderableList(AnimatorControllerToolHelper.Window))
                    _extender = null;
                _extender ??= new ParameterControllerViewExtender();
            }
            else
            {
                _extender = null;
            }
        }

        public ParameterControllerViewExtender()
        {
            if (AnimatorControllerToolHelper.Window == null) return;

            _lastList = ParameterControllerViewReflect.GetParameterReorderableList(AnimatorControllerToolHelper.Window);
            if (_lastList == null)
            {
                Debug.LogWarning("Can't find parameter list, try recreate extender.");
                _extender = null;
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


            foreach (var drawer in Drawers)
            {
                drawer.Init(this, _lastList);
            }
        }

        private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (!IsEnable) return;

            ScrollPosition = ParameterControllerViewReflect.GetParameterScrollPosition(AnimatorControllerToolHelper.Window);

            foreach (var drawer in Drawers.Where(drawer => drawer.IsVisible))
            {
                drawer.OnDrawElement(rect, index, isactive, isfocused);
            }

            if (_lastIndex == index) return;
            _lastIndex = index;

            Repaint();
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
            AnimatorControllerToolHelper.Window?.Repaint();
        }


        [MenuItem(MenuPath, false)]
        private static void ShowOptionWindow()
        {
            _extender.ShowOptions();
        }
    }
}