using System;
using System.Reflection;
using UnityEngine;

namespace Yueby.EditorWindowExtends.Reflections
{
    public static class GameObjectTreeViewGUIReflect
    {
        private static Type _type;
        public static Type Type => _type ??= ReflectionHelper.GetEditorCoreModuleType("GameObjectTreeViewGUI");
    }

    public static class GameObjectStylesReflect
    {
        private static Type _type;
        public static Type Type => _type ?? GameObjectTreeViewGUIReflect.Type.GetNestedType("GameObjectStyles", ReflectionHelper.StaticLookup);

        public static FieldInfo HoveredItemBackgroundStyle => Type.GetField("hoveredItemBackgroundStyle", ReflectionHelper.StaticLookup);
        public static FieldInfo HoveredBackgroundColor => Type.GetField("hoveredBackgroundColor", ReflectionHelper.StaticLookup);

        public static GUIStyle GetHoveredItemBackgroundStyle()
        {
            return (GUIStyle)HoveredItemBackgroundStyle.GetValue(null);
        }

        public static Color GetHoveredBackgroundColor()
        {
            return (Color)HoveredBackgroundColor.GetValue(null);
        }
    }
}