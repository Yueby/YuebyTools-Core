using UnityEditorInternal;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Drawer.ParameterControllerView
{
    public class ScrollPositionFix : ParameterControllerViewDrawer
    {
        private int _lastCount;
        private Vector2 _scrollPosition;
        public override string DrawerName => "Scroll Position Fix";
        public override string Tooltip => "Fix parameter delete and add scroll position";

        public override void Init(ParameterControllerViewExtender extender, ReorderableList reorderableList)
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

                ParameterControllerViewReflect.SetParameterScrollPosition(AnimatorControllerToolHelper.Window, _scrollPosition);

                _lastCount = ReorderableList.list.Count;
            }
        }
    }
}