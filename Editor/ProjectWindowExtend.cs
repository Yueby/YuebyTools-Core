using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.WSA;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.ScriptableObjects;

[InitializeOnLoad]
public class ProjectWindowExtend
{
    private const string MENU_PATH = "Tools/YuebyTools/Extend Project Window";
    private const string TAG = "EXTEND_PROJECT_WINDOW";
    private const float OFFSET_X = 10;
    private static readonly Dictionary<string, string> _convertDict = new Dictionary<string, string>()
    {
        {nameof(BlendTree), ".blendtree"},
        {nameof(VRCExpressionsMenu), ".menu"},
        {nameof(VRCExpressionParameters), ".parameters"}
    };

    private static GUIStyle _style;

    static ProjectWindowExtend()
    {
        if (IsEnable())
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }
    }

    [MenuItem(MENU_PATH)]
    private static void Execute()
    {
        if (IsEnable())
        {
            EditorPrefs.SetString(nameof(ProjectWindowExtend), TAG);
            EditorApplication.projectWindowItemOnGUI -= OnProjectWindowItemGUI;
        }
        else
        {
            EditorPrefs.SetString(nameof(ProjectWindowExtend), "");
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        EditorApplication.RepaintProjectWindow();
    }

    [MenuItem(MENU_PATH, true)]
    public static bool SettingValidate()
    {
        Menu.SetChecked(MENU_PATH, IsEnable());
        return true;
    }

    private static bool IsEnable()
    {
        return EditorPrefs.GetString(nameof(ProjectWindowExtend)) != TAG;
    }

    private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
    {
        var path = AssetDatabase.GUIDToAssetPath(guid);
        OnDrawGUI(selectionRect, path);
        if (IsTreeView(selectionRect)) return;

        if (AssetDatabase.IsValidFolder(path))
            OnDrawFolder(selectionRect, path);
        else
            OnDrawFile(selectionRect, path);

        // if (selectionRect.Contains(Event.current.mousePosition))
        EditorApplication.RepaintProjectWindow();
    }

    private static void OnDrawGUI(Rect rect, string path)
    {
        var currentObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

        if (rect.Contains(Event.current.mousePosition) && Selection.activeObject != currentObject)
            EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height, rect.width, 1), Color.gray);

    }

    private static void OnDrawFile(Rect rect, string path)
    {
        var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        if (asset != null)
        {
            var extension = Path.GetExtension(path);
            if (_convertDict.TryGetValue(asset.GetType().Name, out var ext))
                extension = ext;

            _style ??= new GUIStyle(EditorStyles.label);
            var extensionContent = new GUIContent(extension);
            var size = _style.CalcSize(extensionContent);

            var labelRect = new Rect(OFFSET_X + rect.width - size.x, rect.y, size.x, size.y);
            EditorGUI.LabelField(labelRect, extensionContent);
        }

    }

    private static void OnDrawFolder(Rect rect, string folderPath)
    {

        // // 检查当前事件类型是否为Repaint
        // if (Event.current.type != EventType.Repaint) return;

        // 检测鼠标位置是否在当前rect内
        if (!rect.Contains(Event.current.mousePosition)) return;

        var content = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "Folder On Icon" : "Folder Icon");
        EditorGUIUtility.SetIconSize(new Vector2(16, 16));

        float folderWidth = 24;
        var buttonRect = new Rect(OFFSET_X + rect.width - folderWidth, rect.y, width: folderWidth, rect.height);

        if (GUI.Button(buttonRect, content))
        {
            var guid = AssetDatabase.CreateFolder(folderPath, "New Folder");
            AssetDatabase.Refresh();

            if (!string.IsNullOrEmpty(guid))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var defaultAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path);
                // EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<DefaultAsset>(path));
                ProjectWindowUtil.ShowCreatedAsset(defaultAsset);
            }

            // Debug.Log("Create Sub Folder: " + folderPath);
        }

    }

    private static bool IsTreeView(Rect rect)
    {
        return (rect.x - 16) % 14 == 0;
    }

}
