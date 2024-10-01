using System.Collections.Generic;

namespace Editor.EditorWindowExtends.Core
{
    public interface IEditorExtender
    {
        string Name { get; }
        bool IsEnabled { get; }
        void SetEnable(bool value);
        List<IEditorExtenderDrawer> Drawers { get; }
    }
}