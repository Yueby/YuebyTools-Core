using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core;
using Yueby.EditorWindowExtends.Core;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.Core;
using Yueby.ModalWindow;
using Yueby.Utils;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.ModalWindow
{
    public class ExtenderOptionModalWindowDrawer<T> : ModalEditorWindowDrawer<List<T>> where T : EditorExtenderDrawer
    {
        public override string Title => "Options";
        private readonly ReorderableListDroppable _reorderableList;
        private readonly EditorExtender<T> _extender;

        public ExtenderOptionModalWindowDrawer(List<T> data, EditorExtender<T> extender)
        {
            Data = data;
            _extender = extender;
            position.width = 400;
            position.height = 250;
            _reorderableList = new ReorderableListDroppable(Data, typeof(T), EditorGUIUtility.singleLineHeight, null, false, false)
            {
                OnDraw = OnListDraw,
                OnChanged = OnListChanged,
            };
        }

        private void OnListChanged(int index)
        {
            Data[index].Order = index;
        }

        private float OnListDraw(Rect rect, int index, bool arg3, bool arg4)
        {
            var singleLineHeight = EditorGUIUtility.singleLineHeight;
            var toggleRect = new Rect(rect.x, rect.y, singleLineHeight, singleLineHeight);
            var labelRect = new Rect(rect.x + singleLineHeight, rect.y, rect.width - singleLineHeight, singleLineHeight);
            var item = Data[index];
            item.IsVisible = EditorGUI.Toggle(toggleRect, item.IsVisible);
            EditorGUI.LabelField(labelRect, item.DrawerName);
            return singleLineHeight;
        }


        public override void OnDraw()
        {
            EditorUI.DrawEditorTitle($"{_extender.GetType().Name}", 18, 5);
            _reorderableList.DoLayoutList("", new Vector2(0, position.height - EditorGUIUtility.singleLineHeight - 18 - 10 * 2), false, false, false);

            var enableLabel = !_extender.IsEnable ? "Disable" : "Enable";
            EditorGUI.BeginChangeCheck();
            var enable = EditorUI.Radio(_extender.IsEnable, "Project Browser Extender " + enableLabel);
            if (EditorGUI.EndChangeCheck())
            {
                _extender.ToggleEnable();
            }
        }
    }
}