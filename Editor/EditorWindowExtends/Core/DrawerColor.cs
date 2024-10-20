using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Yueby.EditorWindowExtends.Core
{
    public class DrawerColor
    {
        public Color LightColor { get; }
        public Color DarkColor { get; }

        public DrawerColor(Color light, Color dark)
        {
            LightColor = light;
            DarkColor = dark;
        }

        public Color GetColor()
        {
            return EditorGUIUtility.isProSkin ? DarkColor : LightColor;
        }
    }
}