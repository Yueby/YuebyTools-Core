using System;
using System.Reflection;
using HarmonyLib;
using UnityEditor.IMGUI.Controls;
using Object = UnityEngine.Object;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends
{
    public static class ProjectBrowserReflect
    {
        public static readonly Type TreeViewItemType = AccessTools.TypeByName("UnityEditor.IMGUI.Controls.TreeViewItem");
        public static readonly Type AssetsTreeViewGUIType = AccessTools.TypeByName("UnityEditor.AssetsTreeViewGUI");
        public static readonly Type LocalGroupType = AccessTools.TypeByName("UnityEditor.ObjectListArea").Inner("LocalGroup");
        public static readonly Type FilterResultType = AccessTools.TypeByName("UnityEditor.FilteredHierarchy").Inner("FilterResult");
        public static readonly Type BuiltinResourceType = AccessTools.TypeByName("UnityEditor.BuiltinResource");

        public static readonly FieldInfo TreeViewItemParentField = AccessTools.Field(TreeViewItemType, "m_Parent");
        public static readonly FieldInfo TreeViewItemChildrenField = AccessTools.Field(TreeViewItemType, "m_Children");
        public static readonly FieldInfo TreeViewItemDepthField = AccessTools.Field(TreeViewItemType, "m_Depth");


        private static FieldInfo _assetTreeStateField;
        private static FieldInfo _folderTreeStateField;
        private static MethodInfo _isTwoColumnsMethod;

        public static readonly Type Type = AccessTools.TypeByName("UnityEditor.ProjectBrowser");

        private static FieldInfo AssetTreeStateField
        {
            get
            {
                if (_assetTreeStateField == null) _assetTreeStateField = Type.GetField("m_AssetTreeState", ReflectionHelper.InstanceLookup);
                return _assetTreeStateField;
            }
        }

        private static FieldInfo FolderTreeStateField
        {
            get
            {
                if (_folderTreeStateField == null) _folderTreeStateField = Type.GetField("m_FolderTreeState", ReflectionHelper.InstanceLookup);
                return _folderTreeStateField;
            }
        }

        private static MethodInfo IsTwoColumnsMethod
        {
            get
            {
                if (_isTwoColumnsMethod == null) _isTwoColumnsMethod = Type.GetMethod("IsTwoColumns", ReflectionHelper.InstanceLookup);
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