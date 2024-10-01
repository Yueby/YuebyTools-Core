#if VRC_SDK_VRCSDK3
using UnityEditor;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Contact.Components;
using Yueby.ModalWindow;

namespace Yueby
{
    public class RemoveAvatarComponent : UnityEditor.Editor
    {
        [MenuItem("Tools/YuebyTools/VRChat/Remove PhysBones In Scene")]
        public static void StartRemove()
        {
            ModalEditorWindow.ShowTip("Are you sure to remove PhysBones in scene?", onOk: () =>
            {
                var receivers = FindObjectsByType<VRCContactReceiver>(FindObjectsSortMode.None);
                Debug.Log("Removing " + receivers.Length + " VRCContactReceiver components");
                // Register Undo
                Undo.RegisterCompleteObjectUndo(receivers, "Remove PhysBones Components");

                foreach (var contact in receivers)
                {
                    DestroyImmediate(contact, true);
                }

                var senders = FindObjectsByType<VRCContactSender>(FindObjectsSortMode.None);
                Debug.Log("Removing " + senders.Length + " VRCContactSender components");
                foreach (var sender in senders)
                {
                    DestroyImmediate(sender, true);
                }

                var physBones = FindObjectsByType<VRCPhysBoneBase>(FindObjectsSortMode.None);
                Debug.Log("Removing " + physBones.Length + " VRCPhysBoneBase components");
                foreach (var bone in physBones)
                {
                    DestroyImmediate(bone, true);
                }

                var colliders = FindObjectsByType<VRCPhysBoneColliderBase>(FindObjectsSortMode.None);
                Debug.Log("Removing " + colliders.Length + " VRCPhysBoneColliderBase components");
                foreach (var collider in colliders)
                {
                    DestroyImmediate(collider, true);
                }
            }, showFocusCenter: false);
        }
    }
}
#endif