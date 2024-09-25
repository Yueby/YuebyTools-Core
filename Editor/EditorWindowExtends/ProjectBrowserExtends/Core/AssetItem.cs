using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.Core
{
    public class AssetItem
    {
        public Rect Rect;
        public Rect OriginRect;

        public string Guid { get; private set; }
        public string Path { get; }

        public bool IsFolder { get; private set; }
        public bool IsHover { get; private set; }
        public Object Asset { get; private set; }

        public ProjectBrowserAsset ProjectBrowserAsset { get; set; }

        public AssetItem(string guid, Rect rect)
        {
            OriginRect = rect;
            Guid = guid;
            Rect = rect;
            IsHover = rect.Contains(Event.current.mousePosition);
            Path = AssetDatabase.GUIDToAssetPath(Guid);
            Asset = AssetDatabase.LoadAssetAtPath(Path, typeof(Object));
            IsFolder = !string.IsNullOrEmpty(Path) && AssetDatabase.IsValidFolder(Path);
        }

        public void Refresh(string guid, Rect rect)
        {
            OriginRect = rect;
            Rect = rect;
            Guid = guid;
            IsHover = rect.Contains(Event.current.mousePosition);
        }
    }
}