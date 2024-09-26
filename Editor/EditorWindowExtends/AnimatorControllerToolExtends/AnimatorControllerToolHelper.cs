using System;
using UnityEditor;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends
{
    [InitializeOnLoad]
    public static class AnimatorControllerToolHelper
    {
        // public delegate void DoGraphToolbarDelegate(Rect rect);

        // public static DoGraphToolbarDelegate DoGraphToolbar;

        public static Action<bool> OnAnimatorControllerToolState;
        private static EditorWindow _window;


        // public static EditorWindow Window
        // {
        //     get
        //     {
        //         if (_animatorControllerToolWindow != null) return _animatorControllerToolWindow;
        //
        //         var window = Resources.FindObjectsOfTypeAll(AnimatorWindowReflect.Type);
        //         if (window.Length > 0)
        //         {
        //             _animatorControllerToolWindow = window[0] as EditorWindow;
        //         }
        //
        //         return _animatorControllerToolWindow;
        //     }
        // }

        public static EditorWindow Window
        {
            get
            {
                if (_window == null)
                {
                    _window = (EditorWindow)AnimatorControllerToolReflect.ToolFieldInfo.GetValue(null);
                }

                return _window;
            }
        }


        static AnimatorControllerToolHelper()
        {
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            OnAnimatorControllerToolState?.Invoke(Window != null);
        }
    }
}