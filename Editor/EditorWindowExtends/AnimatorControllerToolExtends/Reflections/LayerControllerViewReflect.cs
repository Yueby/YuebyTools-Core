using System;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;
using Yueby.EditorWindowExtends;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections;
using Object = System.Object;

public static class LayerControllerViewReflect
{
    private static Type _type;
    public static Type Type => _type ??= ReflectionHelper.GetEditorGraphsType("LayerControllerView");

    public static FieldInfo LayerList => Type.GetField("m_LayerList", ReflectionHelper.InstanceLookup);
    public static FieldInfo LayerScroll => Type.GetField("m_LayerScroll", ReflectionHelper.InstanceLookup);


    public static ReorderableList GetLayerReorderableList(Object animatorWindow)
    {
        var layerView = AnimatorControllerToolReflect.LayerEditor.GetValue(animatorWindow);
        return LayerList.GetValue(layerView) as ReorderableList;
    }

    public static Vector2 GetLayerScrollPosition(Object animatorWindow)
    {
        var layerView = AnimatorControllerToolReflect.LayerEditor.GetValue(animatorWindow);
        return (Vector2)LayerScroll.GetValue(layerView);
    }

    public static void SetLayerScrollPosition(Object animatorWindow, Vector2 position)
    {
        var layerView = AnimatorControllerToolReflect.LayerEditor.GetValue(animatorWindow);
        LayerScroll.SetValue(layerView, position);
    }
}