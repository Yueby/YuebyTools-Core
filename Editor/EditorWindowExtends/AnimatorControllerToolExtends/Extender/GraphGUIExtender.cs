using System.Linq;
using Yueby.EditorWindowExtends.AnimatorControllerToolExtends.Core;
using Yueby.EditorWindowExtends.Core;
using Yueby.EditorWindowExtends.HarmonyPatches;
using Yueby.EditorWindowExtends.HarmonyPatches.MapperObject;
using YuebyTools.Core.Utils;

namespace Yueby.EditorWindowExtends.AnimatorControllerToolExtends
{
    public class GraphGUIExtender : EditorExtender<GraphGUIExtender, GraphGUIDrawer>
    {
        public override string Name => "Graph View";

        public static GraphGUIExtender Instance { get; private set; }

        static GraphGUIExtender()
        {
            Instance = new GraphGUIExtender();
        }

        public static void OnDrawStateNode(GraphGUI graphGUI, StateNode stateNode)
        {
            foreach (var drawer in Instance.ExtenderDrawers.Where(drawer => drawer.IsVisible))
            {
                drawer.OnDrawGraphGUI(graphGUI, stateNode);
            }
        }
    }
}