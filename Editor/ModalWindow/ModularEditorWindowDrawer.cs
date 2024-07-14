using System;
using UnityEditor;
using UnityEngine;

namespace Yueby.ModalWindow
{
    public abstract class ModalEditorWindowDrawer<T> : IModalEditorWindowDrawer
    {
        public virtual string Title { get; protected set; } = "Modal Editor Window";

        public virtual Rect Position => _position;
        public T Data { get; protected set; }

        protected Rect _position;

        public ModalEditorWindowDrawer()
        {
            _position = new(0, 0, 300, (EditorGUIUtility.singleLineHeight + 2) * 3);
        }

        public virtual void OnDraw()
        {

        }

    }
}