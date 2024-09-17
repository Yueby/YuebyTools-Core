using System;
using System.Reflection;
using UnityEditor;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends
{
    public static class ProjectWindowUtilHandler
    {
        private static MethodInfo _createFolderWithTemplatesMethod;

        private static MethodInfo CreateFolderWithTemplatesMethod
        {
            get
            {
                if (_createFolderWithTemplatesMethod == null) _createFolderWithTemplatesMethod = Type.GetMethod("CreateFolderWithTemplates", ReflectionHelper.StaticLookup, null, new[] { typeof(string), typeof(string[]) }, null);
                return _createFolderWithTemplatesMethod;
            }
        }

        public static Type Type
        {
            get
            {
                return typeof(ProjectWindowUtil);
            }
        }

        public static void CreateFolderWithTemplates(string defaultName, params string[] templates)
        {
            CreateFolderWithTemplatesMethod.Invoke(null, new object[] { defaultName, templates });
        }

    }
}