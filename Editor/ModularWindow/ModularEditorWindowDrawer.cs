using UnityEngine;

namespace Yueby.PackageImporter
{
    public abstract class ModularEditorWindowDrawer<T> : IModularEditorWindowDrawer
    {
        public virtual string Title { get; } = "Modular Editor Window";

        public virtual Rect Position { get; }
        public T Data { get; protected set; }

        public virtual void OnDraw()
        {

        }

    }
}