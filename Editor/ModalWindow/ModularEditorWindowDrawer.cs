using UnityEditor;
using UnityEngine;

namespace Yueby.ModalWindow
{
    public abstract class ModalEditorWindowDrawer<T> : IModalEditorWindowDrawer
    {
        public virtual string Title { get; protected set; } = "Modal Editor Window";

        public virtual Rect Position => position;

        protected Rect position = new(0, 0, 300, (EditorGUIUtility.singleLineHeight + 2) * 3);

        public T Data { get; protected set; }

        public virtual void OnEnable()
        {
        }

        public void OnDisable()
        {
        }

        public virtual void OnDraw()
        {
        }
    }
}