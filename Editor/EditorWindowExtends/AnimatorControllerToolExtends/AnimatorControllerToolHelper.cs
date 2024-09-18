using System;
using UnityEditor;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends
{
    [InitializeOnLoad]
    public static class AnimatorControllerToolHelper
    {
        public static Action<bool> OnAnimatorControllerToolState;
        private static EditorWindow _animatorControllerToolWindow;


        public static EditorWindow Window
        {
            get
            {
                if (_animatorControllerToolWindow != null) return _animatorControllerToolWindow;

                var window = Resources.FindObjectsOfTypeAll(AnimatorWindowReflect.AnimatorWindowType);
                if (window.Length > 0)
                {
                    _animatorControllerToolWindow = window[0] as EditorWindow;
                }

                return _animatorControllerToolWindow;
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