namespace Editor.EditorWindowExtends.Core
{
    public interface IEditorExtenderDrawer
    {
        bool IsVisible { get; }
        int Order { get; }
        string DrawerName { get; }
        string Tooltip { get; }

        void OnOrderPropertyChanged(int value);
        void OnVisiblePropertyChanged(bool value);

        void ChangeVisible(bool value);
        void ChangeOrder(int value);
        void Repaint();
    }
}