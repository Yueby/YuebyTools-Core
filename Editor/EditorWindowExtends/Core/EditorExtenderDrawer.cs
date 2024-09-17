using UnityEditor;

namespace Yueby.EditorWindowExtends.Core
{
    public class EditorExtenderDrawer
    {
        public EditorExtenderDrawer()
        {
            DrawerName = GetType().Name;
        }

        public string SavePath => $"{GetType().FullName}";

        public bool IsVisible
        {
            get => EditorPrefs.GetBool($"{SavePath}.IsVisible", true);

            set
            {
                EditorPrefs.SetBool($"{SavePath}.IsVisible", value);
                EditorApplication.RepaintProjectWindow();
            }
        }


        public int Order
        {
            get => EditorPrefs.GetInt($"{SavePath}.Order", 0);

            set
            {
                EditorPrefs.SetInt($"{SavePath}.Order", value);
                EditorApplication.RepaintProjectWindow();
            }
        }

        public virtual string DrawerName { get; }
    }
}