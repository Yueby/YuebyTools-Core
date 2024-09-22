using UnityEditor.IMGUI.Controls;
using Yueby.EditorWindowExtends.Core;

namespace Yueby.EditorWindowExtends.ProjectBrowserExtends.Core
{
    public class ProjectBrowserDrawer : EditorExtenderDrawer<ProjectBrowserExtender, ProjectBrowserDrawer>
    {
     

        public virtual void OnProjectBrowserGUI(AssetItem item)
        {
        }

        public virtual void OnProjectBrowserTreeViewItemGUI(AssetItem item, TreeViewItem treeViewItem)
        {
        }
        
        public virtual void OnProjectBrowserTreeViewItemBackgroundGUI(AssetItem item, TreeViewItem treeViewItem)
        {
        }

        public virtual void OnProjectBrowserObjectAreaItemGUI(AssetItem item)
        {
        }
        
        public virtual void OnProjectBrowserObjectAreaItemBackgroundGUI(AssetItem item)
        {
        }

        
        
    }
}