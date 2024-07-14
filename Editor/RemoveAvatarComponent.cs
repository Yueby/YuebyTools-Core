#if VRC_SDK_VRCSDK3
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.Dynamics;
using Yueby.ModalWindow;

namespace Yueby
{
    public class RemoveAvatarComponent : Editor
    {
        [MenuItem("Tools/YuebyTools/VRChat/Remove PhysBones In Scene")]
        public static void StartRemove()
        {
            ModalEditorWindow.Show(new TipsWindowDrawer("Are you sure to remove PhysBones in scene?"), () =>
            {
                var physBones = FindObjectsByType<VRCPhysBoneBase>(FindObjectsSortMode.None);
                Debug.Log("Removing " + physBones.Length + " VRCPhysBoneBase components");
                // Register Undo

                Undo.RegisterCompleteObjectUndo(physBones, "Remove VRCPhysBoneBase Components");
                foreach (var bone in physBones)
                {
                    DestroyImmediate(bone, true);
                }

                var colliders = FindObjectsByType<VRCPhysBoneColliderBase>(FindObjectsSortMode.None);
                Debug.Log("Removing " + colliders.Length + " VRCPhysBoneColliderBase components");
                Undo.RegisterCompleteObjectUndo(colliders, "Remove VRCPhysBoneColliderBase Components");
                foreach (var collider in colliders)
                {
                    DestroyImmediate(collider, true);
                }
            });
        }
    }
}
#endif