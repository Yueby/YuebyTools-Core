using UnityEditor;
using UnityEngine;
using Yueby.EditorWindowExtends.HierarchyExtends.Core;
using Yueby.EditorWindowExtends.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Yueby.EditorWindowExtends.HierarchyExtends.Drawer
{
    public class TreeViewDrawer : HierarchyDrawer
    {
        public override int DefaultOrder => 4;
        private const float LineWidth = 1.2f;

        private SelectionItem _endItem;
        private List<SelectionItem> _selectionItems = new List<SelectionItem>();
        private bool _isCycleFinished = false;

        public override void OnHierarchyChanged()
        {
            base.OnHierarchyChanged();

            if (_selectionItems.Count == 0) return;
            _selectionItems.Clear();
            _isCycleFinished = false;

            _endItem = null;
            Extender.Repaint();
        }

        public override void OnHierarchyWindowItemGUI(SelectionItem selectionItem)
        {
            base.OnHierarchyWindowItemGUI(selectionItem);

            if (_selectionItems.Count > 0 && _selectionItems.Contains(selectionItem))
            {
                // 一次循环了所有item，绘制完毕
                _isCycleFinished = true;
                _endItem = _selectionItems[0];
                foreach (var currentItem in _selectionItems.Where(currentItem => currentItem.OriginRect.y > _endItem.OriginRect.y))
                {
                    _endItem = currentItem;
                }
            }

            _selectionItems.Add(selectionItem);

            var item = selectionItem;
            if (item.TargetObject == null) return;

            var transform = item.TargetObject.transform;
            var rect = item.OriginRect;

            // 横线
            var hLineRect = rect;
            hLineRect.height = LineWidth;
            hLineRect.x -= rect.height * 0.5f;
            hLineRect.x -= rect.height - 2;

            // 如果children为0，横线多往右画一点
            var hasChild = transform.childCount > 0;
            if (!hasChild)
                hLineRect.width = rect.height; // 增加宽度
            else
                hLineRect.width = rect.height * 0.3f;

            hLineRect.y += rect.height * 0.5f;
            EditorGUI.DrawRect(hLineRect, Styles.LineColor.GetColor());

            var isEndOfScene = _endItem != null && _endItem == item;

            // 纵线
            var isLastChild = IsLastChild(transform, item.InstanceID);
            var vLineRect = rect;
            vLineRect.width = LineWidth;
            vLineRect.x -= rect.height * 0.5f;

            vLineRect.x -= rect.height - 2;
            vLineRect.height = isLastChild ? rect.height * 0.5f : rect.height;
            if (isEndOfScene && transform.parent == null)
            {
                vLineRect.height = rect.height * 0.5f;
            }
            EditorGUI.DrawRect(vLineRect, Styles.LineColor.GetColor());

            transform = transform.parent;
            vLineRect.height = rect.height;

            while (transform != null)
            {
                vLineRect.x -= rect.height - 2;

                if (isEndOfScene)
                {
                    vLineRect.height = rect.height * 0.5f;

                    var hLienRectX = hLineRect.x;
                    hLineRect.width = rect.height - 2;

                    hLineRect.x -= hLineRect.width;

                    // 如果父对象是一个根节点
                    if (transform.parent == null)
                    {
                        EditorGUI.DrawRect(vLineRect, Styles.LineColor.GetColor());
                    }
                    EditorGUI.DrawRect(hLineRect, Styles.LineColor.GetColor());
                }
                else
                {
                    if (!IsLastChild(transform, item.InstanceID))
                        EditorGUI.DrawRect(vLineRect, Styles.LineColor.GetColor());
                }

                transform = transform.parent;
            }

            if (_isCycleFinished)
            {
                _isCycleFinished = false;
                _selectionItems.Clear();
            }
        }

        private static bool IsLastChild(Transform transform, int instanceID)
        {
            return transform.parent && transform.GetSiblingIndex() == transform.parent.childCount - 1;
        }

    }
}