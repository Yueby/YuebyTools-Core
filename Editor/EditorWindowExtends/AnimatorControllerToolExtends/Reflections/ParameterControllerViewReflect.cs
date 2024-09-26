using System;
using System.Reflection;
using UnityEditorInternal;
using Object = UnityEngine.Object;
using Vector2 = UnityEngine.Vector2;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections
{
    public static class ParameterControllerViewReflect
    {
        private static Type _type;
        public static Type Type => _type ??= ReflectionHelper.GetEditorGraphsType("ParameterControllerView");

        public static FieldInfo ParameterList => Type.GetField("m_ParameterList", ReflectionHelper.InstanceLookup);
        public static FieldInfo ScrollPosition => Type.GetField("m_ScrollPosition", ReflectionHelper.InstanceLookup);

        public static ReorderableList GetParameterReorderableList(Object animatorWindow)
        {
            var parameterView = AnimatorControllerToolReflect.ParameterEditor.GetValue(animatorWindow);
            return ParameterList.GetValue(parameterView) as ReorderableList;
        }

        public static Vector2 GetParameterScrollPosition(Object animatorWindow)
        {
            var parameterView = AnimatorControllerToolReflect.ParameterEditor.GetValue(animatorWindow);
            return (Vector2)ScrollPosition.GetValue(parameterView);
        }

        public static void SetParameterScrollPosition(Object animatorWindow, Vector2 position)
        {
            var parameterView = AnimatorControllerToolReflect.ParameterEditor.GetValue(animatorWindow);
            ScrollPosition.SetValue(parameterView, position);
        }
    }
}