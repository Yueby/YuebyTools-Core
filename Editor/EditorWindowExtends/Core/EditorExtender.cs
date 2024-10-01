using System;
using System.Collections.Generic;
using System.Reflection;
using Editor.EditorWindowExtends.Core;
using UnityEditor;
using YuebyTools.Core.Utils;

namespace Yueby.EditorWindowExtends.Core
{
    public class EditorExtender<TExtender, TDrawer> : IEditorExtender where TExtender : EditorExtender<TExtender, TDrawer>, new() where TDrawer : EditorExtenderDrawer<TExtender, TDrawer>, new()
    {
        public const string BaseMenuPath = "Tools/YuebyTools/Editor Window Extends/";
        public virtual string Name => GetType().FullName;
        protected List<TDrawer> ExtenderDrawers = new();

        public List<IEditorExtenderDrawer> Drawers
        {
            get => ExtenderDrawers.ConvertAll(drawer => (IEditorExtenderDrawer)drawer);
            set { ExtenderDrawers = value.ConvertAll(drawer => (TDrawer)drawer); }
        }

        // protected readonly ExtenderOptionModalWindowDrawer<TExtender, TDrawer> OptionModalDrawer;

        public bool IsEnabled
        {
            get => EditorPrefs.GetBool($"{Name}.IsEnabled", true);
            protected set
            {
                EditorPrefs.SetBool($"{Name}.IsEnabled", value);
                foreach (var drawer in ExtenderDrawers)
                {
                    drawer.ChangeVisible(value);
                }

                Repaint();
            }
        }

        protected EditorExtender() // 注意构造函数名称
        {
            
            foreach (var drawerType in GetAllDrawerTypes())
            {
                var drawer = (TDrawer)Activator.CreateInstance(drawerType);

                // 强制转换 this 为 TExtender
                drawer?.Init((TExtender)this);
                ExtenderDrawers.Add(drawer);
            }

            ExtenderDrawers.Sort((a, b) => a.Order.CompareTo(b.Order));
            // OptionModalDrawer = new ExtenderOptionModalWindowDrawer<TExtender, TDrawer>(ExtenderDrawers, this);
        }


        private IEnumerable<Type> GetAllDrawerTypes()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                if (type.BaseType == typeof(TDrawer))
                {
                    yield return type;
                }
            }
        }

        // public void ToggleEnable()
        // {
        //     SetEnable(!IsEnabled);
        // }

        public virtual void SetEnable(bool value)
        {
            IsEnabled = value;
        }


        public virtual void Repaint()
        {
        }

        // protected virtual void ShowOptions()
        // {
        //     ModalEditorWindow.ShowUtility(OptionModalDrawer, showFocusCenter: false);
        // }
    }
}