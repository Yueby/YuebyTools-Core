using System.Collections.Generic;

namespace Yueby.EditorWindowExtends.Core
{
    public interface IEditorExtender
    {
        string Name { get; }
        bool IsEnabled { get; }
        List<IEditorExtenderDrawer> Drawers { get; set; }
        void SetEnable(bool value);
    }
}