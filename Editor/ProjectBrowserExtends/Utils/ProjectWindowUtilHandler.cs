using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace Yueby.ProjectBrowserExtends
{
    public static class ProjectWindowUtilHandler
    {
        private static MethodInfo _createFolderWithTemplatesMethod;

        private static MethodInfo CreateFolderWithTemplatesMethod
        {
            get
            {
                if (_createFolderWithTemplatesMethod == null) _createFolderWithTemplatesMethod = Type.GetMethod("CreateFolderWithTemplates", ReflectionHandler.StaticLookup, null, new[] { typeof(string), typeof(string[]) }, null);
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