using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends
{
    [System.Serializable]
    public class ProjectBrowserAsset
    {
        public string Name;
        public string Path;
        public string Guid;
        public bool IsNewAsset;
        public bool HasNewAsset;

        public ProjectBrowserAsset Parent;
        public List<ProjectBrowserAsset> Children = new();
        private Dictionary<string, ProjectBrowserAsset> _childrenDict = new(); // 新增字典以提高查找效率

        public ProjectBrowserAsset(string path, string guid)
        {
            Path = path;
            Guid = guid;
            Name = System.IO.Path.GetFileName(path);
            IsNewAsset = false;
            HasNewAsset = false;
        }

        public ProjectBrowserAsset()
        {
        }

        public ProjectBrowserAsset FindByGuid(string guid)
        {
            if (Guid.Equals(guid, System.StringComparison.OrdinalIgnoreCase))
            {
                return this;
            }

            // 使用字典查找
            if (_childrenDict.TryGetValue(guid, out var childInDict))
            {
                return childInDict; // 找到匹配的子节点
            }

            // 如果字典中未找到，遍历子节点
            foreach (var child in Children)
            {
                var result = child.FindByGuid(guid);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public ProjectBrowserAsset FindByPath(string path)
        {
            if (Path.Equals(path, System.StringComparison.OrdinalIgnoreCase))
            {
                return this;
            }

            foreach (var child in Children)
            {
                var result = child.FindByPath(path);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public bool RemoveByGuid(string guid)
        {
            for (var i = 0; i < Children.Count; i++)
            {
                if (Children[i].Guid.Equals(guid, System.StringComparison.OrdinalIgnoreCase))
                {
                    // Log.Info($"删除资产: {Children[i].Path}");
                    _childrenDict.Remove(Guid); // 从字典中移除
                    Children.RemoveAt(i);
                    RefreshParent(this); // 从子节点移

                    return true;
                }

                if (Children[i].RemoveByGuid(guid))
                {
                    // Log.Info($"删除资产: {Children[i].Path}");
                    RefreshParent(this); // 更新状态
                    return true;
                }
            }

            return false;
        }

        public List<ProjectBrowserAsset> AddAssetToTree(string path)
        {
            var pathParts = path.Split('/');
            var currentAsset = this;
            var addedAssets = new List<ProjectBrowserAsset>(); // 用于存储被添加的资产

            for (var i = 1; i < pathParts.Length; i++)
            {
                var part = pathParts[i];
                var foundChild = currentAsset.Children.Find(child => child.Name.Equals(part, System.StringComparison.OrdinalIgnoreCase));

                if (foundChild == null)
                {
                    var currentPath = string.Join("/", pathParts, 0, i + 1);
                    var instanceId = AssetDatabase.AssetPathToGUID(currentPath);
                    foundChild = new ProjectBrowserAsset(currentPath, instanceId)
                    {
                        Parent = currentAsset // 设置 Parent 属性
                    };
                    currentAsset.Children.Add(foundChild);
                    currentAsset._childrenDict[foundChild.Guid] = foundChild; // 更新字典

                    addedAssets.Add(foundChild);
                }

                currentAsset = foundChild;
            }

            return addedAssets; // 返回所有被添加的资产
        }

        public void SetNewAsset(bool isNew)
        {
            if (Children.Count <= 0 && isNew)
            {
                IsNewAsset = true;
                // 将新资产加入到 AssetListener.NewAssets 字典
                if (!AssetListener.NewAssets.ContainsKey(Guid))
                {
                    AssetListener.NewAssets[Guid] = this; // 使用 Guid 作为键
                }
            }
            else
            {
                IsNewAsset = false;
                // 如果之前在字典中，则移除
                if (AssetListener.NewAssets.ContainsKey(Guid))
                {
                    AssetListener.NewAssets.Remove(Guid); // 根据 Guid 移除
                }
            }

            // 更新父节点状态
            RefreshParent(this);
            // AssetListener.OutputJson();
        }

        public void RefreshParent(ProjectBrowserAsset asset)
        {
            if (asset == null) return;
            asset.HasNewAsset = asset.IsNewAsset || asset.Children.Exists(child => child.HasNewAsset);
            asset.RefreshParent(asset.Parent);
        }
    }

    [InitializeOnLoad]
    public class AssetListener : AssetPostprocessor
    {
        public static ProjectBrowserAsset Root; // 树的根节点

        public static Dictionary<string, ProjectBrowserAsset> NewAssets { get; } = new();

        static AssetListener()
        {
            InitializeFileTree(); // 初始化文件树
        }

        private static void InitializeFileTree()
        {
            Root = new ProjectBrowserAsset("Root", "0");

            // 初始化 Assets 和 Packages 文件夹作为子节点
            var assetsFolder = new ProjectBrowserAsset("Assets", AssetDatabase.AssetPathToGUID("Assets"));
            var packagesFolder = new ProjectBrowserAsset("Packages", AssetDatabase.AssetPathToGUID("Packages"));

            Root.Children.Add(assetsFolder);
            Root.Children.Add(packagesFolder);

            // 获取所有资产路径
            var allAssetPaths = AssetDatabase.GetAllAssetPaths();

            // 遍历每个资产路径
            foreach (var assetPath in allAssetPaths)
            {
                if (assetPath.StartsWith("Assets/"))
                {
                    // 向 Assets 文件夹添加子项目
                    assetsFolder.AddAssetToTree(assetPath);
                }
                else if (assetPath.StartsWith("Packages/"))
                {
                    // 向 Packages 文件夹添加子项目
                    packagesFolder.AddAssetToTree(assetPath);
                }
            }
        }

        public static void OutputJson()
        {
            // 序列化为 JSON
            var json = JsonUtility.ToJson(Root, true);

            // 获取桌面的路径
            string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            // 定义输出的文件路径
            string filePath = Path.Combine(desktopPath, "ProjectBrowserAssets.json");

            // 如果文件已经存在，则删除它
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // 将 JSON 写入文件
            File.WriteAllText(filePath, json);
            Debug.Log($"JSON 数据已输出到: {filePath}"); // 在控制台打印输出文件路径
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var importedNewAssets = new List<ProjectBrowserAsset>();

            // 处理新导入的资产
            foreach (var asset in importedAssets)
            {
                if (asset.StartsWith("Assets/"))
                {
                    // 向 Assets 文件夹添加资产，并返回新资产
                    var newAssets = Root.FindByGuid(AssetDatabase.AssetPathToGUID("Assets"))?.AddAssetToTree(asset);
                    if (newAssets != null)
                    {
                        importedNewAssets.AddRange(newAssets);
                    }
                }
                else if (asset.StartsWith("Packages/"))
                {
                    // 向 Packages 文件夹添加资产，并返回新资产
                    var newAssets = Root.FindByGuid(AssetDatabase.AssetPathToGUID("Packages"))?.AddAssetToTree(asset);
                    if (newAssets != null)
                    {
                        importedNewAssets.AddRange(newAssets);
                    }
                }
            }

            // 标记新导入的资产
            foreach (var newAsset in importedNewAssets)
            {
                newAsset.SetNewAsset(true);
            }

            // 处理删除的资产
            foreach (var deletedAsset in deletedAssets)
            {
                if (deletedAsset.StartsWith("Assets/"))
                {
                    // 从 Assets 文件夹中删除资产
                    var removedAssetGuid = AssetDatabase.AssetPathToGUID(deletedAsset);
                    Root.FindByGuid(AssetDatabase.AssetPathToGUID("Assets"))?.RemoveByGuid(removedAssetGuid);
                    ProjectBrowserExtender.Instance.RemoveAssetItem(removedAssetGuid);
                }
                else if (deletedAsset.StartsWith("Packages/"))
                {
                    // 从 Packages 文件夹中删除资产
                    var removedAssetGuid = AssetDatabase.AssetPathToGUID(deletedAsset);
                    Root.FindByGuid(AssetDatabase.AssetPathToGUID("Packages"))?.RemoveByGuid(removedAssetGuid);
                    ProjectBrowserExtender.Instance.RemoveAssetItem(removedAssetGuid);
                }
            }

            // OutputJson(); // 每次更新后输出 JSON 数据
        }

        public static void ClearAsset(ProjectBrowserAsset asset)
        {
            if (asset == null) return;

            // 将当前资产的 IsNewAsset 设置为 false
            asset.SetNewAsset(false); // 会更新资产并调用 RefreshParent

            // 遍历子节点，递归调用 ClearAsset
            foreach (var child in asset.Children)
            {
                ClearAsset(child); // 递归清理子节点
            }
        }

    }
}