using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using Yueby.EditorWindowExtends.ProjectBrowserExtends.ModalWindow;
using Yueby.ModalWindow;

namespace Yueby.EditorWindowExtends.Core
{
    public class EditorExtender<T> where T : EditorExtenderDrawer
    {
        public const string BaseMenuPath = "Tools/YuebyTools/Editor Window Extends/";
        protected string FullName => GetType().FullName;
        protected readonly List<T> Drawers = new();
        protected readonly ExtenderOptionModalWindowDrawer<T> OptionModalDrawer;

        public bool IsEnable
        {
            get => EditorPrefs.GetBool($"{FullName}.IsEnable", true);
            protected set => EditorPrefs.SetBool($"{FullName}.IsEnable", value);
        }

        protected EditorExtender()
        {
            foreach (var drawerType in GetAllDrawerTypes())
            {
                var drawer = (T)Activator.CreateInstance(drawerType);

                Drawers.Add(drawer);
            }


            Drawers.Sort((a, b) =>
            {
                if (a.Order == b.Order)
                    return 0;
                if (a.Order > b.Order)
                    return 1;
                return -1;
            });

            OptionModalDrawer = new ExtenderOptionModalWindowDrawer<T>(Drawers, this);
        }

        private IEnumerable<Type> GetAllDrawerTypes()
        {
            // Get all classes that inherit from ProjectViewDetailBase:
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                if (type.BaseType == typeof(T))
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