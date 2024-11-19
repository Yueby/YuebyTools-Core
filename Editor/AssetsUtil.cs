using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Logger = Yueby.Core.Utils.Logger;

public static class AssetsUtil
{
    private static bool ValidateSelection()
    {
        return Selection.activeObject != null && Selection.objects.Length <= 1;
    }

    [MenuItem("Assets/YuebyTools/Copy File Path (Absolute)", validate = true)]
    public static bool ValidateCopyRealPath() => ValidateSelection();

    [MenuItem("Assets/YuebyTools/Copy File Path (Absolute)", priority = 10000)]
    public static void CopyFileAbsolutePath()
    {
        var path =Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(Selection.activeObject);
        CopyToClipboard(path, "path");
    }

    [MenuItem("Assets/YuebyTools/Copy Directory Path (Absolute)", validate = true)]
    public static bool ValidateCopyDirectoryPhysicalPath() => ValidateSelection();

    [MenuItem("Assets/YuebyTools/Copy Directory Path (Absolute)", priority = 10001)]
    public static void CopyDirectoryPhysicalPath()
    {
        var path = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(Selection.activeObject);
        var directoryPath = Path.GetDirectoryName(path);
        CopyToClipboard(directoryPath, "directory path");
    }

    [MenuItem("Assets/YuebyTools/Show Folder in Browser", validate = true)]
    public static bool ValidateOpenFolderInBrowser() => Selection.activeObject is DefaultAsset;

    [MenuItem("Assets/YuebyTools/Show Folder in Browser", priority = 10002)]
    public static void OpenFolderInBrowser()
    {
        if (!ValidateOpenFolderInBrowser())
            return;

        var path = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(Selection.activeObject);
        System.Diagnostics.Process.Start(path);
        Logger.LogInfo($"Opening folder: {path}");
    }

    private static void CopyToClipboard(string content, string description)
    {
        EditorGUIUtility.systemCopyBuffer = content;
        Logger.LogInfo($"Copied {description}: {content}");
    }
}
