using System;
using System.Collections.Generic;
using Editor.EditorWindowExtends.Core;
using UnityEditor;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends;
using Yueby.EditorWindowExtends.Core;
using Yueby.EditorWindowExtends.ProjectBrowserExtends;
using Yueby.Utils;
using YuebyTools.Core.Utils;

namespace Yueby.EditorWindowExtends
{
    public class ExtenderOptionWindow : EditorWindow
    {
        private static readonly List<ExtenderOptionHandler> ExtenderOptionHandlers = new();
        private static ExtenderOptionWindow _window;

        private static Vector2 _scrollPos;

        [MenuItem("Tools/YuebyTools/Editor Extender")]
        private static void OpenWindow()
        {
            _window = GetWindow<ExtenderOptionWindow>();
            _window.titleContent = new GUIContent("EditorExtenderOptions");
            _window.minSize = new Vector2(300, 400);
        }

        private void OnEnable()
        {
            ExtenderOptionHandlers.Clear();
            ExtenderOptionHandlers.Add(new ExtenderOptionHandler(ProjectBrowserExtender.Instance));
            ExtenderOptionHandlers.Add(new ExtenderOptionHandler(LayerControllerViewExtender.Instance));
            ExtenderOptionHandlers.Add(new ExtenderOptionHandler(ParameterControllerViewExtender.Instance));
            ExtenderOptionHandlers.Add(new ExtenderOptionHandler(GraphGUIExtender.Instance));
        }

        private void OnGUI()
        {
            _scrollPos = EditorUI.ScrollViewEGL(() =>
            {
                for (var i = 0; i < ExtenderOptionHandlers.Count; i++)
                {
                    var handler = ExtenderOptionHandlers[i];
                    handler.OnDraw();

                    if (i != ExtenderOptionHandlers.Count - 1)
                    {
                        EditorUI.Line();
                    }
                }
            }, _scrollPos);
        }
    }

    public class ExtenderOptionHandler
    {
        public ReorderableListDroppable List;
        public IEditorExtender Extender;

        public ExtenderOptionHandler(IEditorExtender extender)
        {
            List = SetupList(extender.Drawers);
            Extender = extender;
        }


        private ReorderableListDroppable SetupList(List<IEditorExtenderDrawer> data)
        {
            Log.Info(data.Count);
            return new ReorderableListDroppable(data, typeof(IEditorExtenderDrawer), EditorGUIUtility.singleLineHeight, null, false, false)
            {
                OnDraw = OnListDraw,
                OnChanged = OnListChanged,
            };
        }

        private void OnListChanged(int index)
        {
            Extender.Drawers[index].ChangeOrder(index);
        }

        private float OnListDraw(Rect rect, int index, bool arg3, bool arg4)
        {
            var singleLineHeight = EditorGUIUtility.singleLineHeight;
            var toggleRect = new Rect(rect.x, rect.y, singleLineHeight, singleLineHeight);
            var labelRect = new Rect(rect.x + singleLineHeight, rect.y, rect.width - singleLineHeight, singleLineHeight);
            var item = Extender.Drawers[index];

            item.ChangeVisible(EditorGUI.Toggle(toggleRect, item.IsVisible));

            EditorGUI.LabelField(labelRect, new GUIContent(item.DrawerName, item.Tooltip));
            return singleLineHeight;
        }


        public void OnDraw()
        {
            EditorUI.TitleLabelField($"{Extender.Name}");
            var enableLabel = !Extender.IsEnabled ? "Disable" : "Enable";
            EditorGUI.BeginChangeCheck();
            var enabled = EditorUI.Toggle(Extender.IsEnabled, Extender.Name + " " + enableLabel);
            if (EditorGUI.EndChangeCheck())
            {
                Extender.SetEnable(enabled);
            }

            List.DoLayout("", true, false, false, false);
        }
    }
}