using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yueby.EditorWindowExtends.Core;
using Yueby.ModalWindow;
using Yueby.Utils;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.ModalWindow
{
    public class ExtenderOptionModalWindowDrawer<TExtender, TDrawer> : ModalEditorWindowDrawer<List<TDrawer>> where TExtender : EditorExtender<TExtender, TDrawer>, new() where TDrawer : EditorExtenderDrawer<TExtender, TDrawer>, new()
    {
        public override string Title => "Options";
        private readonly ReorderableListDroppable _reorderableList;
        private readonly EditorExtender<TExtender, TDrawer> _extender;

        public ExtenderOptionModalWindowDrawer(List<TDrawer> data, EditorExtender<TExtender, TDrawer> extender)
        {
            Data = data;
            _extender = extender;
            position.width = 400;
            position.height = 250;
            _reorderableList = new ReorderableListDroppable(Data, typeof(TDrawer), EditorGUIUtility.singleLineHeight, null, false, false)
            {
                OnDraw = OnListDraw,
                OnChanged = OnListChanged,
            };
        }

        private void OnListChanged(int index)
        {
            Data[index].ChangeOrder(index);
        }

        private float OnListDraw(Rect rect, int index, bool arg3, bool arg4)
        {
            var singleLineHeight = EditorGUIUtility.singleLineHeight;
            var toggleRect = new Rect(rect.x, rect.y, singleLineHeight, singleLineHeight);
            var labelRect = new Rect(rect.x + singleLineHeight, rect.y, rect.width - singleLineHeight, singleLineHeight);
            var item = Data[index];

            item.ChangeVisible(EditorGUI.Toggle(toggleRect, item.IsVisible));

            EditorGUI.LabelField(labelRect, new GUIContent(item.DrawerName, item.ToolTip));
            return singleLineHeight;
        }


        public override void OnDraw()
        {
            EditorUI.DrawEditorTitle($"{_extender.GetType().Name}", 18, 5);
            _reorderableList.DoLayoutList("", new Vector2(0, position.height - EditorGUIUtility.singleLineHeight - 18 - 10 * 2), false, false, false);

            var enableLabel = !_extender.IsEnable ? "Disable" : "Enable";
            EditorGUI.BeginChangeCheck();
            var enable = EditorUI.Radio(_extender.IsEnable, _extender.GetType().Name + " " + enableLabel);
            if (EditorGUI.EndChangeCheck())
            {
                _extender.ToggleEnable();
            }
        }
    }
}