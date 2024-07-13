using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.UI;
using UnityEditorInternal;
using UnityEngine;
using Yueby.Utils;

namespace Yueby.PackageImporter
{
    public class PackageImporterEditorWindow : EditorWindow
    {
        private static PackageImporterEditorWindow _window;
        private const string _key = "PackageImporterData";

        private PackageImporterData _packageImporterData;
        private ReorderableListDroppable _reorderableList;

        [MenuItem("Tools/YuebyTools/Utils/Package Importer")]
        public static void ShowWindow()
        {
            _window = GetWindow<PackageImporterEditorWindow>();
            _window.titleContent = new GUIContent("Package Importer");
        }

        private void OnEnable()
        {
            // Debug.Log("OnEnable");
            var packageData = EditorPrefs.GetString(_key);
            _packageImporterData = new PackageImporterData();
            if (!string.IsNullOrEmpty(packageData))
                JsonUtility.FromJsonOverwrite(packageData, _packageImporterData);

            _reorderableList = new ReorderableListDroppable(_packageImporterData.Packages, typeof(PackageInfo), EditorGUIUtility.singleLineHeight, null, true, true, Repaint)
            {
                OnAdd = OnAdd,
                OnDraw = OnDraw,
                OnRemove = OnRemove,
            };
        }

        private void OnDisable()
        {
            // Save data
            SaveData();
        }

        private float OnDraw(Rect rect, int index, bool arg3, bool arg4)
        {
            var height = EditorGUIUtility.singleLineHeight;
            var labelRect = new Rect(rect.x, rect.y, rect.width, height);
            EditorGUI.LabelField(labelRect, new GUIContent(_packageImporterData.Packages[index].Name), EditorStyles.boldLabel);

            var style = new GUIStyle(EditorStyles.miniLabel);
            style.normal.textColor = Color.gray;
            var pathRect = new Rect(rect.x, labelRect.y + height, rect.width - 22, height);
            EditorGUI.LabelField(pathRect, new GUIContent(_packageImporterData.Packages[index].Path), style);

            var descriptionRect = new Rect(rect.x, pathRect.y + height, rect.width - 22, height);
            EditorGUI.LabelField(descriptionRect, new GUIContent(_packageImporterData.Packages[index].Description));

            var result = descriptionRect.y + descriptionRect.height - labelRect.y;

            if (GUI.Button(new Rect(rect.width, rect.y + result / 2 - 10, 20, 20), "●"))
            {
                ImportPackage(_packageImporterData.Packages[index].Path);
                // Debug.Log("Import" + _packageImporterData.Packages[index].Name);
            }

            return result;
        }

        private void OnAdd(ReorderableList list)
        {
            var packageConfigureDrawer = new PackageConfigureDrawer();
            ModularEditorWindow.ShowWindow(packageConfigureDrawer, () =>
            {
                var data = packageConfigureDrawer.Data;
                if (!string.IsNullOrEmpty(data.Path))
                {
                    // Debug.Log(data.Path);
                    if (string.IsNullOrEmpty(data.Description))
                        data.Description = "No description";

                    list.list.Add(data);

                    // _packageImporterData.Packages.Add(data);

                    SaveData();
                }
            });
        }

        private void OnRemove(ReorderableList list)
        {
            SaveData();
        }

        private void SaveData()
        {
            var json = JsonUtility.ToJson(_packageImporterData);
            // Debug.Log(json);
            EditorPrefs.SetString(_key, json);
        }

        private void OnGUI()
        {
            EditorUI.DrawEditorTitle("Package Importer");
            _reorderableList.DoLayoutList("", new Vector2(0, 0), false, false, false);

            _packageImporterData.IsInteractive = EditorUI.Radio(_packageImporterData.IsInteractive, "Interactive");
            if (GUILayout.Button("Import All", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                ImportAllAssets();
            }
        }

        private void ImportAllAssets()
        {
            var unityPackages = new string[_packageImporterData.Packages.Count];
            for (var i = 0; i < _packageImporterData.Packages.Count; i++)
                unityPackages[i] = _packageImporterData.Packages[i].Path;

            for (int i = 0; i < unityPackages.Length; i++)
            {
                if (unityPackages[i].EndsWith(".unitypackage"))
                {
                    AssetDatabase.ImportPackage(unityPackages[i], _packageImporterData.IsInteractive);
                }
            }

            Debug.Log("Assets have been imported");
        }

        private void ImportPackage(string path)
        {
            if (path.EndsWith(".unitypackage"))
            {
                AssetDatabase.ImportPackage(path, _packageImporterData.IsInteractive);
                Debug.Log("Package has been imported: " + path);
            }

        }

    }

    [Serializable]
    public class PackageInfo
    {
        public string Name;
        public string Path;
        public string Description;

        public PackageInfo() { }

        public PackageInfo(string name, string path, string description)
        {
            Name = name;
            Path = path;
            Description = description;
        }

    }

    [Serializable]
    public class PackageImporterData
    {
        public List<PackageInfo> Packages;
        public bool IsInteractive;

        public PackageImporterData()
        {
            Packages = new List<PackageInfo>();
        }

        public PackageImporterData(List<PackageInfo> packages)
        {
            Packages = packages;
        }
    }
}
