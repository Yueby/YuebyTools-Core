using UnityEditor;
using Yueby.Core;

namespace Yueby.EditorWindowExtends.Core
{
    public class EditorExtenderDrawer<TExtender, TDrawer>
        where TExtender : EditorExtender<TExtender, TDrawer>, new()
        where TDrawer : EditorExtenderDrawer<TExtender, TDrawer>, new()
    {
        protected readonly BindProperty<bool> IsVisibleProperty = new(true);
        protected readonly BindProperty<int> OrderProperty = new(0);

        // 将 Extender 属性修改为 TExtender 类型
        public virtual TExtender Extender { get; set; }

        public string SavePath => $"{GetType().FullName}";
        public bool IsVisible => EditorPrefs.GetBool($"{SavePath}.IsVisible", true);
        public int Order => EditorPrefs.GetInt($"{SavePath}.Order", 0);
        public virtual string DrawerName { get; } = "";
        public virtual string ToolTip { get; } = "";

        public EditorExtenderDrawer()
        {
            DrawerName = GetType().Name;
            IsVisibleProperty.ValueChanged += OnVisiblePropertyChanged;
            OrderProperty.ValueChanged += OnOrderPropertyChanged;
        }

        // 修改 Init 方法
        public virtual void Init(TExtender extender)
        {
            Extender = extender;
        }

        protected virtual void OnOrderPropertyChanged(int value)
        {
            EditorPrefs.SetInt($"{SavePath}.Order", value);
            EditorApplication.RepaintProjectWindow();
        }

        protected virtual void OnVisiblePropertyChanged(bool value)
        {
            EditorPrefs.SetBool($"{SavePath}.IsVisible", value);
            EditorApplication.RepaintProjectWindow();
        }

        public void ChangeVisible(bool value)
        {
            IsVisibleProperty.Value = value;
        }

        public void ChangeOrder(int value)
        {
            OrderProperty.Value = value;
        }

        public void Repaint()
        {
            Extender.Repaint();
        }
    }
}