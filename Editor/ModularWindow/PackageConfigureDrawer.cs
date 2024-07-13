using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Yueby.Utils;

namespace Yueby.PackageImporter
{
    public class PackageConfigureDrawer : ModularEditorWindowDrawer<PackageInfo>
    {
        public override string Title => "Package Configure";
        public override Rect Position => new(0, 0, 300, (EditorGUIUtility.singleLineHeight + 2) * 3);

        private const string _packagePathKey = "PackageImporter_LastOpenPackagePath";

        public PackageConfigureDrawer()
        {
            Data = new PackageInfo();
        }

        public override void OnDraw()
        {
            const int labelWidth = 80;

            EditorUI.HorizontalEGL(() =>
            {
                Data.Path = EditorUI.TextField("Path", Data.Path, labelWidth);
                if (GUILayout.Button("Browse"))
                {
                    var lastOpenPackagePath = EditorPrefs.GetString(_packagePathKey, "Assets");

                    var result = EditorUtility.OpenFilePanel("Select Package", lastOpenPackagePath, "unitypackage");
                    if (!string.IsNullOrEmpty(result))
                    {
                        Data.Path = result;
                        var file = new FileInfo(result);
                        Data.Name = file.Name.Replace(file.Extension, "");

                        EditorPrefs.SetString(_packagePathKey, file.Directory.FullName);
                    }
                }
            });

            Data.Name = EditorUI.TextField("Name", Data.Name, labelWidth: labelWidth);
            Data.Description = EditorUI.TextField("Description", Data.Description, labelWidth);
        }
    }
}