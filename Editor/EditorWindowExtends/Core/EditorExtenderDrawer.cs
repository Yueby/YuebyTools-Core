using UnityEditor;
using Yueby.Core;
using YuebyTools.Core.Utils;

namespace Yueby.EditorWindowExtends.Core
{
    public class EditorExtenderDrawer<TExtender, TDrawer> : IEditorExtenderDrawer
        where TExtender : EditorExtender<TExtender, TDrawer>, new()
        where TDrawer : EditorExtenderDrawer<TExtender, TDrawer>, new()
    {
        protected readonly BindProperty<bool> IsVisibleProperty = new(true);
        protected readonly BindProperty<int> OrderProperty = new(0);

        // 将 Extender 属性修改为 TExtender 类型
        public virtual TExtender Extender { get; set; }

        public string SavePath => $"{GetType().FullName}";

        public bool IsVisible
        {
            get
            {
                var isVisible = EditorPrefs.GetBool($"{SavePath}.IsVisible", true);
                IsVisibleProperty.Value = isVisible;
                return isVisible;
            }
        }

        public int Order
        {
            get
            {
                var order = EditorPrefs.GetInt($"{SavePath}.Order", DefaultOrder);
                OrderProperty.Value = order;
                return order;
            }
        }

        public virtual int DefaultOrder { get; } = 0;
        public virtual string DrawerName { get; } = "";
        public virtual string Tooltip { get; } = "";

        public EditorExtenderDrawer()
        {
            DrawerName = GetType().Name;
        }

        // 修改 Init 方法
        public virtual void Init(TExtender extender)
        {
            Extender = extender;
            IsVisibleProperty.ValueChanged += OnVisiblePropertyChanged;
            OrderProperty.ValueChanged += OnOrderPropertyChanged;
        }


        public virtual void OnOrderPropertyChanged(int value)
        {
            EditorPrefs.SetInt($"{SavePath}.Order", value);

            Repaint();
        }

        public virtual void OnVisiblePropertyChanged(bool value)
        {
            EditorPrefs.SetBool($"{SavePath}.IsVisible", value);
            Repaint();
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