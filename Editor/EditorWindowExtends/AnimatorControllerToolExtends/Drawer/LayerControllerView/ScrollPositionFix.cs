using UnityEditorInternal;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core;
using Yueby.EditorWindowExtends.Core;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Drawer.LayerControllerView
{
    public class ScrollPositionFix : LayerControllerViewDrawer
    {
        public override string DrawerName => "Scroll Position Fix (When Delete Layer)";
        private Vector2 _scrollPosition;
        private int _lastCount = 0;

        public override void Init(LayerControllerViewExtender extender, ReorderableList reorderableList)
        {
            base.Init(extender, reorderableList);
            _scrollPosition = Vector2.zero;
        }

        public override void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            base.OnDrawElement(rect, index, isactive, isfocused);

            if (Extender.ScrollPosition != Vector2.zero)
                _scrollPosition = Extender.ScrollPosition;

            if (_lastCount != ReorderableList.list.Count)
            {
                _lastCount = ReorderableList.list.Count;
                LayerControllerViewReflect.SetLayerScrollPosition(Extender.AnimatorWindow, _scrollPosition);
            }
        }
    }
}