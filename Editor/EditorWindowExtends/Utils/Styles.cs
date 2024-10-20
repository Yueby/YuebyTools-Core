using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yueby.EditorWindowExtends.Core;
namespace Yueby.EditorWindowExtends.Utils
{
    public static class Styles
    {

        public static readonly DrawerColor SeparatorColor = new(new Color(0.69f, 0.69f, 0.69f, 1f), new Color(0.149f, 0.149f, 0.149f, 1f));//= new(new Color(0.4f, 0.4f, 0.4f, 0.282353f), new Color(0.1882353f, 0.1882353f, 0.1882353f, 1f));
        public static readonly DrawerColor EvenShadingColor = new(new Color(0f, 0f, 0f, 0.03137255f), new Color(0f, 0f, 0f, 0.07450981f));
        public static readonly DrawerColor OddShadingColor = new(new Color(1f, 1f, 1f, 0f), new Color(0f, 0f, 0f, 0f));

        public static readonly DrawerColor LineColor = new(new Color(0.3647059f, 0.3647059f, 0.3647059f, 0.5647059f), new Color(1f, 1f, 1f, 0.2235294f));

        public static readonly DrawerColor HeaderColor = new(new Color(0.9098f, 0.9098f, 0.9098f, 1f), new Color(0.16078431f, 0.16078431f, 0.16078431f, 1f));

        public static readonly DrawerColor HierarchyBackgroundColor = new(new Color(0.9098f, 0.9098f, 0.9098f, 1f), new Color(0.3098f, 0.3098f, 0.3098f, 1));

        public static readonly GUIStyle HeaderStyle = new(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

    }
}