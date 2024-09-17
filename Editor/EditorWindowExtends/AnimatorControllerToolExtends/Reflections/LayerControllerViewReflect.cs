using System;
using System.Reflection;
using UnityEditorInternal;
using Yueby.EditorWindowExtends;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections;
using Object = System.Object;

public static class LayerControllerViewReflect
{
    private static Type _layerControllerViewType;
    public static Type LayerControllerViewType => _layerControllerViewType ??= ReflectionHelper.GetEditorGraphsType("LayerControllerView");

    public static FieldInfo LayerList => LayerControllerViewType.GetField("m_LayerList", ReflectionHelper.InstanceLookup);


    public static ReorderableList GetLayerReorderableList(Object animatorWindow)
    {
        var layerView = AnimatorWindowReflect.GetLayerViewObject(animatorWindow);
        return LayerList.GetValue(layerView) as ReorderableList;
    }
}