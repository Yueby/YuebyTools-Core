using System.Threading.Tasks;
using HarmonyLib;
using UnityEditorInternal;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core;
using Yueby.Utils.Reflections;
using YuebyTools.Core.Utils;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;


namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Drawer.LayerControllerView
{
    public class LayerDefaultWeight : LayerControllerViewDrawer
    {
        public override string DrawerName => "Layer Default Weight To 1.0";
        private Vector2 _scrollPosition;
        private int _lastCount;

        public override async void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            base.OnDrawElement(rect, index, isactive, isfocused);
            
            if (_lastCount == ReorderableList.list.Count) return;

            // 增加层
            if (_lastCount < ReorderableList.list.Count)
            {
                var endIndex = ReorderableList.list.Count - 1;
                var window = AnimatorControllerToolHelper.Window;
                var controller = (AnimatorController)window.GetType().GetFieldValue("m_AnimatorController", window);


                if (ReorderableList.list[endIndex] is AnimatorControllerLayer layer)
                {
                    layer.defaultWeight = 1.0f;
                    var layers = controller.layers;
                    layers[endIndex] = layer;
                    controller.layers = layers;
                    layer = controller.layers[endIndex];
                }
            }

            _lastCount = ReorderableList.list.Count;
        }
    }
}