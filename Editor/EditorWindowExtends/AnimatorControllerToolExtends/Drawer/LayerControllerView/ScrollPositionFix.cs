using UnityEditorInternal;
using UnityEngine;
using Yueby.Core.Utils;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core;
using Logger = Yueby.Core.Utils.Logger;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Drawer.LayerControllerView
{
    public class ScrollPositionFix : LayerControllerViewDrawer
    {
        private int _lastCount;
        private Vector2 _scrollPosition;

        public override string DrawerName => "Scroll Position Fix";
        public override string Tooltip => "Fix layer delete and add scroll position";

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
                // 增加层
                if (_lastCount < ReorderableList.list.Count)
                {
                    var y = rect.height * ReorderableList.list.Count;
                    _scrollPosition.y = y;
                }

                _lastCount = ReorderableList.list.Count;
            }

            LayerControllerViewReflect.SetLayerScrollPosition(AnimatorControllerToolHelper.Window, _scrollPosition);
        }
    }
}