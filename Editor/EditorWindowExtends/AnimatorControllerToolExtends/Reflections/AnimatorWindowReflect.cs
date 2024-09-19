using System;
using System.Reflection;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections
{
    public static class AnimatorWindowReflect
    {
        private static Type _type;

        public static Type Type => _type ??= ReflectionHelper.GetEditorGraphsType("AnimatorControllerTool");

        public static FieldInfo LayerEditor => Type.GetField("m_LayerEditor", ReflectionHelper.InstanceLookup);
        public static FieldInfo ParameterEditor => Type.GetField("m_ParameterEditor", ReflectionHelper.InstanceLookup);

        public static FieldInfo ToolFieldInfo => Type.GetField("tool", ReflectionHelper.StaticLookup);

        public static MethodInfo DoGraphToolbarMethodInfo => Type.GetMethod("DoGraphToolbar", ReflectionHelper.InstanceLookup);
    }
}