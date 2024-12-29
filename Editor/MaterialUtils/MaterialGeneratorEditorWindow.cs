using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yueby.Core.Utils;
using Yueby.Utils;
namespace Yueby
{
    public class MaterialGeneratorEditorWindow : EditorWindow
    {
        private Material _templateMaterial;
        private List<Texture2D> _textures = new List<Texture2D>();
        private string _outputPath = "Assets/GeneratedMaterials";
        private ReorderableListDroppable _textureList;
        private string _materialNamePrefix = "";
        private string _materialNameSuffix = "_Material";
        private bool _overwriteExisting;
        private bool _showNaming;
        private Dictionary<int, string> _customNames = new Dictionary<int, string>();

        [MenuItem("Tools/YuebyTools/Utils/MaterialGenerator", false, 10)]
        private static void ShowWindow()
        {
            var window = GetWindow<MaterialGeneratorEditorWindow>();
            window.titleContent = new GUIContent("Material Generator");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }

        private void OnEnable()
        {
            InitializeList();
            LoadPrefs();
        }

        private void LoadPrefs()
        {
            _outputPath = EditorPrefs.GetString("MaterialGenerator_OutputPath", "Assets/GeneratedMaterials");
            _materialNamePrefix = EditorPrefs.GetString("MaterialGenerator_Prefix", "");
            _materialNameSuffix = EditorPrefs.GetString("MaterialGenerator_Suffix", "_Material");
            _overwriteExisting = EditorPrefs.GetBool("MaterialGenerator_Overwrite", false);
            _showNaming = EditorPrefs.GetBool("MaterialGenerator_ShowNaming", false);
            var customNamesJson = EditorPrefs.GetString("MaterialGenerator_CustomNames", "{}");
            _customNames = JsonUtility.FromJson<Dictionary<int, string>>(customNamesJson);
        }

        private void SavePrefs()
        {
            EditorPrefs.SetString("MaterialGenerator_OutputPath", _outputPath);
            EditorPrefs.SetString("MaterialGenerator_Prefix", _materialNamePrefix);
            EditorPrefs.SetString("MaterialGenerator_Suffix", _materialNameSuffix);
            EditorPrefs.SetBool("MaterialGenerator_Overwrite", _overwriteExisting);
            EditorPrefs.SetBool("MaterialGenerator_ShowNaming", _showNaming);
            var customNamesJson = JsonUtility.ToJson(_customNames);
            EditorPrefs.SetString("MaterialGenerator_CustomNames", customNamesJson);
        }

        private void InitializeList()
        {
            _textureList = new ReorderableListDroppable(
                _textures,
                typeof(Texture2D),
                60f,
                Repaint,
                true,
                true,
                Repaint
            );

            _textureList.OnDraw = (rect, index, active, focused) =>
            {
                rect.y += 2;
                rect.height = 56f;

                if (active)
                    EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 0.2f));

                var previewRect = rect;
                previewRect.width = 56f;
                previewRect.height = 56f;

                var contentRect = rect;
                contentRect.x = previewRect.xMax + 5;
                contentRect.width = rect.width - previewRect.width - 5;

                var nameRect = contentRect;
                nameRect.height = EditorGUIUtility.singleLineHeight;

                var customNameRect = contentRect;
                customNameRect.y = nameRect.yMax + 2;
                customNameRect.height = EditorGUIUtility.singleLineHeight;

                var objectFieldRect = contentRect;
                objectFieldRect.y = customNameRect.yMax + 2;
                objectFieldRect.height = EditorGUIUtility.singleLineHeight;

                var texture = _textures[index];
                EditorGUI.BeginChangeCheck();
                texture = EditorGUI.ObjectField(
                    objectFieldRect,
                    texture,
                    typeof(Texture2D),
                    false
                ) as Texture2D;

                if (EditorGUI.EndChangeCheck() && texture != null && !_textures.Contains(texture))
                {
                    _textures[index] = texture;
                }

                if (texture != null)
                {
                    var previewTexture = AssetPreview.GetAssetPreview(texture) ?? texture;
                    GUI.DrawTexture(previewRect, previewTexture, ScaleMode.ScaleToFit);

                    if (!_customNames.ContainsKey(index))
                        _customNames[index] = "";

                    EditorGUI.BeginChangeCheck();
                    _customNames[index] = EditorGUI.TextField(customNameRect, "Custom Name", _customNames[index]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        SavePrefs();
                    }

                    var baseName = string.IsNullOrEmpty(_customNames[index]) ? texture.name : _customNames[index];
                    var materialName = $"{_materialNamePrefix}{baseName}{_materialNameSuffix}.mat";
                    EditorGUI.LabelField(nameRect, materialName, EditorStyles.boldLabel);
                }
                else
                {
                    EditorGUI.LabelField(previewRect, "No Texture", EditorStyles.centeredGreyMiniLabel);
                    EditorGUI.LabelField(nameRect, "Please select a texture", EditorStyles.boldLabel);
                }

                return 60f;
            };

            _textureList.OnAdd += _ => { _textures.Add(null); };
            _textureList.OnRemove += list =>
            {
                if (_customNames.ContainsKey(list.index))
                {
                    _customNames.Remove(list.index);
                    SavePrefs();
                }
                Repaint();
            };
        }

        private void OnGUI()
        {
            EditorUI.VerticalEGL(() =>
            {
                // Template Material
                EditorUI.VerticalEGL("Badge", () =>
                {
                    EditorGUILayout.LabelField("Template Material");
                    EditorUI.HorizontalEGL(() =>
                    {
                        _templateMaterial = EditorGUILayout.ObjectField(
                            _templateMaterial,
                            typeof(Material),
                            false
                        ) as Material;

                        if (_templateMaterial != null && GUILayout.Button("Locate", GUILayout.Width(50)))
                        {
                            EditorGUIUtility.PingObject(_templateMaterial);
                        }
                    });

                    if (_templateMaterial == null)
                    {
                        EditorGUILayout.HelpBox("Please select a template material first", MessageType.Info);
                    }
                });

                EditorGUILayout.Space(10);

                // Texture List
                var bottomHeight = 150f;
                var listHeight = position.height - bottomHeight;
                _textureList.DoLayout(
                    "Texture List",
                    new Vector2(0, listHeight),
                    false,
                    false,
                    true,
                    HandleDroppedObjects,
                    Repaint
                );

                GUILayout.FlexibleSpace();

                // Bottom Settings
                EditorGUILayout.Space(10);
                using (new GUILayout.VerticalScope())
                {
                    // Output Settings
                    EditorUI.VerticalEGL("Badge", () =>
                    {
                        _showNaming = EditorGUILayout.Foldout(_showNaming, "Naming Settings");
                        if (_showNaming)
                        {
                            EditorGUI.indentLevel++;
                            _materialNamePrefix = EditorGUILayout.TextField("Prefix", _materialNamePrefix);
                            _materialNameSuffix = EditorGUILayout.TextField("Suffix", _materialNameSuffix);
                            EditorGUI.indentLevel--;
                        }

                        _overwriteExisting = EditorGUILayout.ToggleLeft("Overwrite Existing Materials", _overwriteExisting);

                        EditorUI.HorizontalEGL(() =>
                        {
                            EditorGUILayout.PrefixLabel("Output Path");
                            _outputPath = EditorGUILayout.TextField(_outputPath);
                            if (GUILayout.Button("Select", GUILayout.Width(50)))
                            {
                                var path = EditorUtility.OpenFolderPanel("Select Output Path", "Assets", "");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    _outputPath = FileUtil.GetProjectRelativePath(path);
                                }
                            }
                        });
                    });

                    EditorGUILayout.Space(10);

                    // Generate Button
                    using (new EditorGUI.DisabledGroupScope(_templateMaterial == null || _textures.Count == 0))
                    {
                        if (GUILayout.Button("Generate Materials", GUILayout.Height(30)))
                        {
                            GenerateMaterials();
                        }
                    }
                }
            });
        }

        private void HandleDroppedObjects(Object[] objects)
        {
            if (objects == null) return;

            foreach (var obj in objects)
            {
                // 验证对象类型
                if (!(obj is Texture2D || obj is DefaultAsset)) continue;

                if (obj is Texture2D texture)
                {
                    if (!_textures.Contains(texture))
                    {
                        _textures.Add(texture);
                    }
                }
                else if (obj is DefaultAsset)
                {
                    var path = AssetDatabase.GetAssetPath(obj);
                    if (AssetDatabase.IsValidFolder(path))
                    {
                        var textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { path });
                        foreach (var guid in textureGuids)
                        {
                            var texturePath = AssetDatabase.GUIDToAssetPath(guid);
                            var folderTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                            if (!_textures.Contains(folderTexture))
                            {
                                _textures.Add(folderTexture);
                            }
                        }
                    }
                }
            }
        }

        private void GenerateMaterials()
        {
            if (!AssetDatabase.IsValidFolder(_outputPath))
            {
                AssetDatabase.CreateFolder(
                    System.IO.Path.GetDirectoryName(_outputPath),
                    System.IO.Path.GetFileName(_outputPath)
                );
            }

            var successCount = 0;
            var skipCount = 0;

            foreach (var texture in _textures)
            {
                if (texture == null) continue;

                var index = _textures.IndexOf(texture);
                var baseName = _customNames.ContainsKey(index) && !string.IsNullOrEmpty(_customNames[index])
                    ? _customNames[index]
                    : texture.name;

                var materialName = $"{_materialNamePrefix}{baseName}{_materialNameSuffix}";
                var materialPath = $"{_outputPath}/{materialName}.mat";

                if (AssetDatabase.LoadAssetAtPath<Material>(materialPath) != null)
                {
                    if (!_overwriteExisting)
                    {
                        skipCount++;
                        continue;
                    }
                    AssetDatabase.DeleteAsset(materialPath);
                }

                var newMaterial = new Material(_templateMaterial);
                newMaterial.mainTexture = texture;
                AssetDatabase.CreateAsset(newMaterial, materialPath);
                successCount++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Complete", $"Generation completed!\nSuccess: {successCount}\nSkipped: {skipCount}", "OK");

            if (successCount > 0)
            {
                EditorUtility.FocusProjectWindow();
                var folderObject = AssetDatabase.LoadAssetAtPath<Object>(_outputPath);
                EditorGUIUtility.PingObject(folderObject);
            }
        }
    }
}