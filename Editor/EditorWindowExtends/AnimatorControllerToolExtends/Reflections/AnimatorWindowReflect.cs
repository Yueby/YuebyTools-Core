﻿using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEditorInternal;
using UnityEngine;
using Object = System.Object;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Reflections
{
    public static class AnimatorWindowReflect
    {
        private static Type _animatorWindowType;

        public static Type AnimatorWindowType => _animatorWindowType ??= ReflectionHelper.GetEditorGraphsType("AnimatorControllerTool");

        public static FieldInfo LayerEditor => AnimatorWindowType.GetField("m_LayerEditor", ReflectionHelper.InstanceLookup);

        public static Object GetLayerViewObject(Object animatorWindow)
        {
            return LayerEditor.GetValue(animatorWindow);
        }
    }
}