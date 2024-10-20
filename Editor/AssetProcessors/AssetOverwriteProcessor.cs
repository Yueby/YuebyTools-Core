using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// 处理资源覆盖的类
public class AssetOverwriteProcessor : AssetPostprocessor
{
    // 存储待删除文件的集合
    private static HashSet<string> _filesToRemove = new HashSet<string>();

    // 预处理资产的方法
    private void OnPreprocessAsset()
    {
        // 如果存在meta文件则返回
        if (!assetImporter.importSettingsMissing) return;

        // 在拖放前后文件名一致的情况下返回
        var name = Path.GetFileNameWithoutExtension(assetPath);
        if (DragAndDrop.paths.Length == 0 || DragAndDrop.paths.Any(p => name == Path.GetFileName(p))) return;

        // 如果文件名不是以" 数字"结尾则返回
        var count = name.Split(" ").Last();
        if (!uint.TryParse(count, out _)) return;

        // 如果目标文件不存在则返回
        var original = Path.GetDirectoryName(assetPath) + Path.DirectorySeparatorChar + name[..(name.Length - count.Length - 1)] + Path.GetExtension(assetPath);
        if (!File.Exists(original)) return;

        var result = EditorUtility.DisplayDialog("Warning", "The file already exists, do you want to overwrite it?", "Yes", "No");
        if (!result) return;

        // 复制文件并覆盖
        File.Copy(assetPath, original, true);
        _filesToRemove.Add(assetPath);


        // Logger.Info("Overwrite asset:", original);

        // 创建一个虚拟文件以加速
        // 删除文件会导致出错
        if (assetImporter is TextureImporter)
            WriteAndCopyTime(assetPath, new Texture2D(1, 1).EncodeToPNG()); // 扩展名应该与png固定

        if (assetPath.EndsWith(".obj", System.StringComparison.OrdinalIgnoreCase))
            WriteAndCopyTime(assetPath, new byte[] { });

        if (assetPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
            WriteAndCopyTime(assetPath, "; FBX 7.7.0 project file");
    }

    // 后处理所有资产的方法
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
    {
        // 如果没有文件待删除则返回
        if (_filesToRemove.Count == 0) return;

        // 删除待删除的文件
        foreach (var file in _filesToRemove)
            AssetDatabase.DeleteAsset(file);
        _filesToRemove.Clear();

        AssetDatabase.Refresh();
    }

    // 写入并复制时间（字符串版本）
    private static void WriteAndCopyTime(string path, string value)
    {
        var fi = new FileInfo(path);
        var timeC = fi.CreationTime; // 获取创建时间
        var timeW = fi.LastWriteTime; // 获取最后修改时间
        File.WriteAllText(path, value); // 写入文件内容
        // 复制时间
        new FileInfo(path) { CreationTime = timeC, LastWriteTime = timeW };
    }

    // 写入并复制时间（字节数组版本）
    private static void WriteAndCopyTime(string path, byte[] value)
    {
        var fi = new FileInfo(path);
        var timeC = fi.CreationTime; // 获取创建时间
        var timeW = fi.LastWriteTime; // 获取最后修改时间
        File.WriteAllBytes(path, value); // 写入文件内容
        // 复制时间
        new FileInfo(path) { CreationTime = timeC, LastWriteTime = timeW };
    }
}
