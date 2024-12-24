using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yueby.Utils;

namespace Yueby
{
    public class MaterialSwitcherEditorWindow : EditorWindow
    {
        private MaterialSwitcher _targetComponent;
        private List<RendererData> _rendererDatas = new List<RendererData>();
        private Vector2 _scrollPosition;
        private bool _globalPreviewMode;

        [MenuItem("Tools/YuebyTools/Utils/MaterialSwitcher", false, 11)]
        private static void ShowWindow()
        {
            var window = GetWindow<MaterialSwitcherEditorWindow>();
            window.titleContent = new GUIContent("Material Switcher");
            window.Show();
        }

        private void OnEnable()
        {
            if (_targetComponent == null)
            {
                EditorGUILayout.HelpBox("Currently in temporary preview mode. To save the configuration, please add MaterialSwitcher component to a scene object.", MessageType.Info);
            }
        }

        public void InitializeWithComponent(MaterialSwitcher component)
        {
            _targetComponent = component;
            LoadFromComponent();
        }

        private void LoadFromComponent()
        {
            _rendererDatas.Clear();

            foreach (var config in _targetComponent.rendererConfigs)
            {
                var data = new RendererData(config, _targetComponent);
                
                if (data.Renderer != null)
                {
                    UpdateMaterialSlots(data);
                    data.SelectedMaterialIndex = config.selectedMaterialIndex;
                    data.CurrentMaterialIndex = config.currentMaterialIndex;
                    
                    if (config.materials.Count > 0 && 
                        config.currentMaterialIndex >= 0 && 
                        config.currentMaterialIndex < config.materials.Count)
                    {
                        data.IsApplied = true;
                        data.AppliedMaterial = config.materials[config.currentMaterialIndex];
                    }
                }
                
                _rendererDatas.Add(data);
            }
        }

        private void SaveToComponent()
        {
            if (_targetComponent == null) return;

            _targetComponent.rendererConfigs.Clear();
            foreach (var data in _rendererDatas)
            {
                var config = new MaterialSwitcher.RendererConfig
                {
                    name = data.Name,
                    rendererPath = data.Renderer != null ? GetRelativePath(_targetComponent.transform, data.Renderer.transform) : "",
                    selectedMaterialIndex = data.SelectedMaterialIndex,
                    currentMaterialIndex = data.IsApplied ? data.CurrentMaterialIndex : -1, // 只保存已应用的材质索引
                    materials = new List<Material>(data.Materials),
                    isFoldout = data.IsFoldout
                };

                _targetComponent.rendererConfigs.Add(config);
            }

            EditorUtility.SetDirty(_targetComponent);
        }

        private void OnDisable()
        {
            // 先保存数据
            SaveToComponent();
            
            // 再恢复未应用的预览
            foreach (var data in _rendererDatas)
            {
                if (data.Renderer == null) continue;
                
                if (data.IsPreviewMode)
                {
                    var materials = data.Renderer.sharedMaterials;
                    // 如果已经应用了，保持应用的材质
                    if (data.IsApplied)
                    {
                        materials[data.SelectedMaterialIndex] = data.AppliedMaterial;
                    }
                    // 如果未应用，恢复原始材质
                    else if (data.OriginalMaterial != null)
                    {
                        materials[data.SelectedMaterialIndex] = data.OriginalMaterial;
                    }
                    data.Renderer.sharedMaterials = materials;
                    data.IsPreviewMode = false;
                }
            }
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            var newConfigName = EditorGUILayout.TextField("Config Name", _targetComponent.configName);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_targetComponent, "Change Config Name");
                _targetComponent.configName = newConfigName;
                EditorUtility.SetDirty(_targetComponent);
            }
            // 添加提示信息
            if (_targetComponent == null)
            {
                EditorGUILayout.HelpBox("Currently in temporary preview mode. To save the configuration, please add MaterialSwitcher component to a scene object.", MessageType.Info);
            }

            EditorUI.VerticalEGL(() =>
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                // 渲染器列表
                for (int i = 0; i < _rendererDatas.Count; i++)
                {
                    var data = _rendererDatas[i];
                    DrawRendererSection(data, i);
                }

                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space(5);

                // 底部控制栏
                EditorUI.HorizontalEGL(() =>
                {
                    // 全局预览控制
                    var globalPreviewContent = EditorGUIUtility.IconContent("d_scenevis_visible_hover");
                    globalPreviewContent.tooltip = "Toggle global preview mode";
                    var newGlobalPreview = GUILayout.Toggle(_globalPreviewMode, globalPreviewContent, "Button", GUILayout.Width(24), GUILayout.Height(20));
                    if (newGlobalPreview != _globalPreviewMode)
                    {
                        _globalPreviewMode = newGlobalPreview;
                        ToggleAllPreviews(_globalPreviewMode);
                    }

                    EditorGUI.BeginDisabledGroup(!_globalPreviewMode);
                    if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.PrevKey"), GUILayout.Width(24), GUILayout.Height(20)))
                    {
                        SwitchAllMaterials(-1);
                    }

                    if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.NextKey"), GUILayout.Width(24), GUILayout.Height(20)))
                    {
                        SwitchAllMaterials(1);
                    }

                    // 全局 Apply 按钮
                    var applyAllContent = EditorGUIUtility.IconContent("d_Valid");
                    applyAllContent.tooltip = "Apply all previewed materials";
                    var applyAllStyle = new GUIStyle("Button")
                    {
                        padding = new RectOffset(4, 4, 4, 4),
                        imagePosition = ImagePosition.ImageOnly
                    };
                    if (GUILayout.Button(applyAllContent, applyAllStyle, GUILayout.Width(24), GUILayout.Height(20)))
                    {
                        ApplyAllMaterials();
                    }
                    EditorGUI.EndDisabledGroup();

                    GUILayout.FlexibleSpace();

                    // 添加新渲染器按钮
                    if (GUILayout.Button("Add Renderer", GUILayout.Width(100)))
                    {
                        var newConfig = new MaterialSwitcher.RendererConfig
                        {
                            name = $"Renderer {_rendererDatas.Count + 1}",
                            isFoldout = true
                        };
                        
                        if (_targetComponent != null)
                        {
                            _targetComponent.rendererConfigs.Add(newConfig);
                        }
                        _rendererDatas.Add(new RendererData(newConfig, _targetComponent));
                    }
                });
            });
        }

        private void DrawRendererSection(RendererData data, int index)
        {
            EditorUI.VerticalEGL("Badge", () =>
            {
                // Header
                EditorUI.HorizontalEGL(() =>
                {
                    // 折叠箭头和标题
                    var titleText = string.IsNullOrEmpty(data.Name) ? $"Renderer {index + 1}" : data.Name;
                    if (data.Renderer != null && data.Materials.Count > 0 && data.CurrentMaterialIndex >= 0)
                    {
                        var currentMaterial = data.Materials[data.CurrentMaterialIndex];
                        if (currentMaterial != null)
                        {
                            titleText += $" - {currentMaterial.name}";
                        }
                    }
                    data.IsFoldout = EditorGUILayout.Foldout(data.IsFoldout, titleText, true);

                    if (data.Renderer != null)
                    {
                        // 预览按钮
                        var previewContent = EditorGUIUtility.IconContent("d_scenevis_visible_hover");
                        previewContent.tooltip = "Click to toggle preview state";
                        bool newPreviewMode = GUILayout.Toggle(data.IsPreviewMode, previewContent, "Button", GUILayout.Width(24), GUILayout.Height(20));
                        if (newPreviewMode != data.IsPreviewMode)
                        {
                            data.IsPreviewMode = newPreviewMode;
                            if (!newPreviewMode)
                            {
                                _globalPreviewMode = false;
                            }
                        }

                        // 上一个材质
                        EditorGUI.BeginDisabledGroup(data.CurrentMaterialIndex <= 0);
                        if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.PrevKey"), GUILayout.Width(24), GUILayout.Height(20)))
                        {
                            data.SwitchMaterial(-1);
                        }
                        EditorGUI.EndDisabledGroup();

                        // 下一个材质
                        EditorGUI.BeginDisabledGroup(data.Materials.Count == 0 || data.CurrentMaterialIndex >= data.Materials.Count - 1);
                        if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.NextKey"), GUILayout.Width(24), GUILayout.Height(20)))
                        {
                            data.SwitchMaterial(1);
                        }
                        EditorGUI.EndDisabledGroup();

                        // 应用按钮
                        EditorGUI.BeginDisabledGroup(!data.IsPreviewMode);
                        var applyContent = EditorGUIUtility.IconContent("d_Valid");
                        applyContent.tooltip = "Apply current material";
                        var applyStyle = new GUIStyle("Button")
                        {
                            padding = new RectOffset(4, 4, 4, 4),
                            imagePosition = ImagePosition.ImageOnly
                        };
                        if (GUILayout.Button(applyContent, applyStyle, GUILayout.Width(24), GUILayout.Height(20)))
                        {
                            data.ApplyMaterial();
                        }
                        EditorGUI.EndDisabledGroup();
                    }

                    // 删除按钮
                    if (GUILayout.Button("x", GUILayout.Width(24), GUILayout.Height(20)))
                    {
                        _rendererDatas.RemoveAt(index);
                        GUIUtility.ExitGUI();
                    }
                });

                if (data.IsFoldout)
                {
                    EditorGUILayout.Space(5);

                    // 名称设置
                    EditorGUI.BeginChangeCheck();
                    var newName = EditorGUILayout.TextField("Name", data.Name);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_targetComponent, "Change Config Name");
                        data.Name = newName;
                        EditorUtility.SetDirty(_targetComponent);
                    }

                    // Renderer引用
                    EditorGUI.BeginChangeCheck();
                    data.Renderer = EditorGUILayout.ObjectField("Renderer", data.Renderer, typeof(Renderer), true) as Renderer;
                    if (EditorGUI.EndChangeCheck() && data.Renderer != null)
                    {
                        UpdateMaterialSlots(data);
                    }

                    if (data.Renderer != null)
                    {
                        // 材质索引选择
                        data.SelectedMaterialIndex = EditorGUILayout.Popup("Material Index", data.SelectedMaterialIndex, data.MaterialSlotNames);

                        EditorGUILayout.Space(5);

                        // 材质列表
                        data.MaterialList.DoLayout(
                            "Material List",
                            Vector2.zero,
                            false,
                            false,
                            true,
                            data.HandleMaterialDrop,
                            Repaint
                        );
                    }
                }
            });

            EditorGUILayout.Space(5);
        }

        private void UpdateMaterialSlots(RendererData data)
        {
            var materials = data.Renderer.sharedMaterials;
            data.MaterialSlotNames = new string[materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                data.MaterialSlotNames[i] = $"Slot {i} ({(materials[i] != null ? materials[i].name : "None")})";
            }
            data.SelectedMaterialIndex = 0;
        }

        private void ToggleAllPreviews(bool enabled)
        {
            foreach (var data in _rendererDatas)
            {
                if (data.Renderer == null) continue;
                
                // 只切换预览状态
                if (data.IsPreviewMode != enabled)
                {
                    data.IsPreviewMode = enabled;
                }
            }
        }

        private void SwitchAllMaterials(int direction)
        {
            foreach (var data in _rendererDatas)
            {
                if (data.IsPreviewMode && data.Renderer != null && data.Materials.Count > 0)
                {
                    data.SwitchMaterial(direction);
                }
            }
        }

        private void ApplyAllMaterials()
        {
            foreach (var data in _rendererDatas)
            {
                if (data.IsPreviewMode)
                {
                    data.ApplyMaterial();
                }
            }
        }

        private class RendererData
        {
            private MaterialSwitcher _component;
            private Renderer _cachedRenderer;
            private MaterialSwitcher.RendererConfig _config;
            private MaterialSwitcherEditorWindow _window;

            // 序列化数据的属性包装
            public Renderer Renderer
            {
                get
                {
                    if (_cachedRenderer == null && !string.IsNullOrEmpty(_config.rendererPath) && _component != null)
                    {
                        // 从组件所在物体开始查找相对路径
                        Transform target = _component.transform.Find(_config.rendererPath);
                        if (target != null)
                        {
                            _cachedRenderer = target.GetComponent<Renderer>();
                        }
                    }
                    return _cachedRenderer;
                }
                set
                {
                    _cachedRenderer = value;
                    if (value != null && _component != null)
                    {
                        // 使用保存的窗口用
                        _config.rendererPath = _window.GetRelativePath(_component.transform, value.transform);
                    }
                    else
                    {
                        _config.rendererPath = "";
                    }
                }
            }
            public string Name
            {
                get => _config.name;
                set => _config.name = value;
            }
            public int SelectedMaterialIndex
            {
                get => _config.selectedMaterialIndex;
                set => _config.selectedMaterialIndex = value;
            }
            public List<Material> Materials => _config.materials;
            public bool IsFoldout
            {
                get => _config.isFoldout;
                set => _config.isFoldout = value;
            }

            // 编辑器运行时数
            public string[] MaterialSlotNames = new string[0];
            public ReorderableListDroppable MaterialList;
            public int CurrentMaterialIndex
            {
                get => _config.currentMaterialIndex;
                set => _config.currentMaterialIndex = value;
            }
            private bool _isPreviewMode;
            public bool IsPreviewMode
            {
                get => _isPreviewMode;
                set
                {
                    if (_isPreviewMode == value) return;
                    
                    _isPreviewMode = value;
                    
                    if (Renderer == null || Materials.Count == 0) return;

                    if (_isPreviewMode)
                    {
                        // 开启预览时，先保存原始材质（如果未应用过）
                        if (!IsApplied)
                        {
                            OriginalMaterial = Renderer.sharedMaterials[SelectedMaterialIndex];
                        }
                        else
                        {
                            // 如果之前已经应用过，确保使用保存的索引
                            CurrentMaterialIndex = _config.currentMaterialIndex;
                        }
                        
                        // 应用预览材质
                        var materials = Renderer.sharedMaterials;
                        materials[SelectedMaterialIndex] = Materials[CurrentMaterialIndex];
                        Renderer.sharedMaterials = materials;
                    }
                    else
                    {
                        // 关闭预览时，恢复到已应用的材质或原始材质
                        var materials = Renderer.sharedMaterials;
                        materials[SelectedMaterialIndex] = IsApplied ? AppliedMaterial : OriginalMaterial;
                        Renderer.sharedMaterials = materials;
                    }
                }
            }
            public Material OriginalMaterial;
            public Material AppliedMaterial;
            public bool IsApplied;

            public RendererData(MaterialSwitcher.RendererConfig config, MaterialSwitcher component)
            {
                _config = config;
                _component = component;
                _window = EditorWindow.GetWindow<MaterialSwitcherEditorWindow>();
                CurrentMaterialIndex = config.currentMaterialIndex; // 恢复保存的材质索引
                
                // 如果有保存的材质，则标记为已应用
                IsApplied = config.materials.Count > 0 && 
                            config.currentMaterialIndex >= 0 && 
                            config.currentMaterialIndex < config.materials.Count;
                if (IsApplied)
                {
                    AppliedMaterial = config.materials[config.currentMaterialIndex];
                }
                
                InitializeList();

                // 初始化材质槽名称数组
                MaterialSlotNames = new string[0];
                if (Renderer != null)
                {
                    var materials = Renderer.sharedMaterials;
                    MaterialSlotNames = new string[materials.Length];
                    for (int i = 0; i < materials.Length; i++)
                    {
                        MaterialSlotNames[i] = $"Slot {i} ({(materials[i] != null ? materials[i].name : "None")})";
                    }
                }
            }

            private void InitializeList()
            {
                MaterialList = new ReorderableListDroppable(
                    Materials,
                    typeof(Material),
                    EditorGUIUtility.singleLineHeight,
                    EditorWindow.focusedWindow.Repaint,
                    true,
                    true
                );

                MaterialList.OnDraw = (rect, index, active, focused) =>
                {
                    var material = Materials[index];
                    EditorGUI.ObjectField(rect, material, typeof(Material), false);
                    return EditorGUIUtility.singleLineHeight;
                };

                MaterialList.OnAdd += _ => Materials.Add(null);
                MaterialList.OnRemove += list =>
                {
                    if (list.index >= 0 && list.index < Materials.Count)
                    {
                        Materials.RemoveAt(list.index);
                    }
                };

                MaterialList.RefreshElementHeights();
            }

            public void HandleMaterialDrop(Object[] objects)
            {
                foreach (var obj in objects)
                {
                    if (obj is Material material && !Materials.Contains(material))
                    {
                        Materials.Add(material);
                    }
                }
            }

            public void SwitchMaterial(int direction)
            {
                CurrentMaterialIndex = Mathf.Clamp(CurrentMaterialIndex + direction, 0, Materials.Count - 1);
                PreviewMaterial();
            }

            public void PreviewMaterial()
            {
                if (!IsPreviewMode || Renderer == null || Materials.Count == 0) return;

                var materials = Renderer.sharedMaterials;
                materials[SelectedMaterialIndex] = Materials[CurrentMaterialIndex];
                Renderer.sharedMaterials = materials;
            }

            public void ApplyMaterial()
            {
                if (!IsPreviewMode || Renderer == null || Materials.Count == 0) return;

                // 记录 Undo
                Undo.RecordObject(Renderer, "Apply Material Change");
                if (_component != null)
                {
                    Undo.RecordObject(_component, "Save Material Index");
                }
                
                // 保存当前应用的材质和索引
                AppliedMaterial = Materials[CurrentMaterialIndex];
                _config.currentMaterialIndex = CurrentMaterialIndex;
                
                // 应用当前预览的材质
                var materials = Renderer.sharedMaterials;
                materials[SelectedMaterialIndex] = AppliedMaterial;
                Renderer.sharedMaterials = materials;
                
                // 标记为已应用
                IsApplied = true;
                
                // 标记场景已改变
                EditorUtility.SetDirty(Renderer);
                if (_component != null)
                {
                    EditorUtility.SetDirty(_component);
                }
            }
        }

        private string GetRelativePath(Transform root, Transform target)
        {
            if (root == target) return "";
            
            string path = target.name;
            Transform parent = target.parent;
            
            while (parent != null && parent != root)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            
            return path;
        }
    }
}