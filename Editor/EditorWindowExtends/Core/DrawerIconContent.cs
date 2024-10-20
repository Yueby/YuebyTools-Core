using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Yueby.EditorWindowExtends.Core
{
    public class DrawerIconContent
    {
        public GUIContent Light { get; }
        public GUIContent Dark { get; }

        public DrawerIconContent(GUIContent light, GUIContent dark)
        {
            Light = light;
            Dark = dark;
        }

        public GUIContent GetContent()
        {
            return EditorGUIUtility.isProSkin ? Dark : Light;
        }
    }
}