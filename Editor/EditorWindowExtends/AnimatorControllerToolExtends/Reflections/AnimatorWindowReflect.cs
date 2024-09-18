using System;
using System.Reflection;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections
{
    public static class AnimatorWindowReflect
    {
        private static Type _animatorWindowType;

        public static Type AnimatorWindowType => _animatorWindowType ??= ReflectionHelper.GetEditorGraphsType("AnimatorControllerTool");

        public static FieldInfo LayerEditor => AnimatorWindowType.GetField("m_LayerEditor", ReflectionHelper.InstanceLookup);
        public static FieldInfo ParameterEditor => AnimatorWindowType.GetField("m_ParameterEditor", ReflectionHelper.InstanceLookup);
    }
}