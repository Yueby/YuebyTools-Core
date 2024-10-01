using UnityEditor;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Drawer.LayerControllerView
{
    public class IndexDrawer : LayerControllerViewDrawer
    {
        private GUIStyle _indexLabelStyleGray;

        private GUIStyle _indexLabelStyleWhite;
        public override string DrawerName => "Index Label";
        public override string Tooltip => "Draw the index of the layer in the list.";

        public override void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (ReorderableList.list[index] is not AnimatorControllerLayer layer) return;

            const float handleWidth = 18f;
            var indexContent = new GUIContent($"{index}");


            _indexLabelStyleGray ??= new GUIStyle(GUI.skin.label)
            {
                richText = true,
                alignment = TextAnchor.MiddleLeft,
                normal =
                {
                    textColor = Color.gray
                }
            };

            _indexLabelStyleWhite ??= new GUIStyle(GUI.skin.label)
            {
                richText = true,
                alignment = TextAnchor.MiddleLeft
            };


            var size = _indexLabelStyleWhite.CalcSize(indexContent);
            var style = _indexLabelStyleGray;


            if ( /*Extender.IsFocusedOnWindow &&*/ rect.Contains(Event.current.mousePosition))
                style = _indexLabelStyleWhite;

            var indexLabelRect = new Rect(rect.x - handleWidth, rect.y + rect.height - EditorGUIUtility.singleLineHeight - 6, size.x, size.y);
            EditorGUI.LabelField(indexLabelRect, indexContent, style);
        }
    }
}