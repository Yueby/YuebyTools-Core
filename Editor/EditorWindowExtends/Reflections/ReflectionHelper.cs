using System;
using System.Reflection;

namespace Yueby.EditorWindowExtends
{
    public static class ReflectionHelper
    {
        public const BindingFlags InstanceLookup = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        public const BindingFlags StaticLookup = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        public const BindingFlags AllLookup = InstanceLookup | StaticLookup;
        private static Assembly _editorAssembly;

        private static Assembly _editorGraphsAssembly;
        private static Assembly _editorCoreModuleAssembly;

        public static Assembly EditorAssembly
        {
            get
            {
                if (_editorAssembly == null) _editorAssembly = Assembly.Load("UnityEditor");
                return _editorAssembly;
            }
        }

        public static Assembly EditorGraphsAssembly
        {
            get
            {
                if (_editorGraphsAssembly == null) _editorGraphsAssembly = Assembly.Load("UnityEditor.Graphs");
                return _editorGraphsAssembly;
            }
        }

        public static Assembly EditorCoreModuleAssembly
        {
            get
            {
                if (_editorCoreModuleAssembly == null) _editorCoreModuleAssembly = Assembly.Load("UnityEditor.CoreModule");
                return _editorCoreModuleAssembly;
            }
        }

        public static Type GetEditorType(string name, string @namespace = "UnityEditor")
        {
            var typeName = @namespace + "." + name;
            return EditorAssembly.GetType(typeName);
        }

        public static Type GetEditorGraphsType(string name, string @namespace = "UnityEditor.Graphs")
        {
            var typeName = @namespace + "." + name;
            return EditorGraphsAssembly.GetType(typeName);
        }

        public static Type GetEditorCoreModuleType(string name, string @namespace = "UnityEditor")
        {
            var typeName = @namespace + "." + name;
            return EditorCoreModuleAssembly.GetType(typeName);
        }
    }
}