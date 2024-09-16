using UnityEditorInternal;
using UnityEngine;
using Yueby.EditorWindowExtends.Core;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core
{
    public class LayerControllerViewDrawer : EditorExtenderDrawer
    {
        public ReorderableList ReorderableList { get; private set; }
        public LayerControllerViewExtender Extender { get; private set; }

        public virtual void Init(ReorderableList reorderableList, LayerControllerViewExtender editorWindowExtender)
        {
            ReorderableList = reorderableList;
            Extender = editorWindowExtender;
        }

        public virtual void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
        }

        public virtual void OnDrawElementBackground(Rect rect, int index, bool isactive, bool isfocused)
        {
        }

        public virtual void OnMouseUp(ReorderableList list)
        {
        }

        public virtual void OnMouseDrag(ReorderableList list)
        {
        }

        public virtual void OnRemove(ReorderableList list)
        {
        }

        public virtual void OnSelect(ReorderableList list)
        {
        }

        public virtual void OnChanged(ReorderableList list)
        {
        }

        public virtual void OnAdd(ReorderableList list)
        {
        }
    }
}