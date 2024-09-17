using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.ModalWindow;
using Yueby.ModalWindow;

namespace Yueby.EditorWindowExtends.Core
{
    public class EditorExtender<TExtender, TDrawer> where TExtender : EditorExtender<TExtender, TDrawer>, new() where TDrawer : EditorExtenderDrawer<TExtender, TDrawer>, new()
    {
        public const string BaseMenuPath = "Tools/YuebyTools/Editor Window Extends/";
        protected string FullName => GetType().FullName;
        protected readonly List<TDrawer> Drawers = new();
        protected readonly ExtenderOptionModalWindowDrawer<TExtender, TDrawer> OptionModalDrawer;

        public bool IsEnable
        {
            get => EditorPrefs.GetBool($"{FullName}.IsEnable", true);
            protected set => EditorPrefs.SetBool($"{FullName}.IsEnable", value);
        }

        protected EditorExtender() // 注意构造函数名称
        {
            foreach (var drawerType in GetAllDrawerTypes())
            {
                var drawer = (TDrawer)Activator.CreateInstance(drawerType);

                // 强制转换 this 为 TExtender
                drawer?.Init((TExtender)this);

                Drawers.Add(drawer);
            }

            Drawers.Sort((a, b) => a.Order.CompareTo(b.Order));
            OptionModalDrawer = new ExtenderOptionModalWindowDrawer<TExtender, TDrawer>(Drawers, this);
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

        public void ToggleEnable()
        {
            SetExtenderEnable(!IsEnable);
        }

        public virtual void SetExtenderEnable(bool value)
        {
            IsEnable = value;
            Repaint();
        }

        public virtual void Repaint()
        {
        }

        protected virtual void ShowOptions()
        {
            ModalEditorWindow.ShowUtility(OptionModalDrawer, showFocusCenter: false);
        }
    }
}