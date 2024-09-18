using System;

namespace Yueby.EditorWindowExtends.Reflections
{
    public static class EditorResourcesReflect
    {
        private static Type _type;
        public static Type Type => _type ??= ReflectionHelper.GetEditorCoreModuleType("UnityEditor.Experimental.EditorResources");
    }
}