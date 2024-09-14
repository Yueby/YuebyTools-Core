using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Yueby.ProjectBrowserExtends
{
    public static class ProjectBrowserHandler
    {
        private static Type _type;
        private static FieldInfo _assetTreeStateField;
        private static FieldInfo _folderTreeStateField;
        private static MethodInfo _isTwoColumnsMethod;

        public static Type Type
        {
            get
            {
                if (_type == null) _type = ReflectionHandler.GetEditorType("ProjectBrowser");
                return _type;
            }
        }

        private static FieldInfo AssetTreeStateField
        {
            get
            {
                if (_assetTreeStateField == null) _assetTreeStateField = Type.GetField("m_AssetTreeState", ReflectionHandler.InstanceLookup);
                return _assetTreeStateField;
            }
        }

        private static FieldInfo FolderTreeStateField
        {
            get
            {
                if (_folderTreeStateField == null) _folderTreeStateField = Type.GetField("m_FolderTreeState", ReflectionHandler.InstanceLookup);
                return _folderTreeStateField;
            }
        }

        private static MethodInfo IsTwoColumnsMethod
        {
            get
            {
                if (_isTwoColumnsMethod == null) _isTwoColumnsMethod = Type.GetMethod("IsTwoColumns", ReflectionHandler.InstanceLookup);
                return _isTwoColumnsMethod;
            }
        }

        public static TreeViewState GetAssetTreeViewState(Object projectWindow)
        {
            return AssetTreeStateField.GetValue(projectWindow) as TreeViewState;
        }

        public static TreeViewState GetFolderTreeViewState(Object projectWindow)
        {
            return FolderTreeStateField.GetValue(projectWindow) as TreeViewState;
        }

        public static bool IsTwoColumns(Object projectWindow)
        {
            return (bool)IsTwoColumnsMethod.Invoke(projectWindow, null);
        }
    }
}