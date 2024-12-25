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
                    
                    // 初始化应用状态
                    if (config.materials.Count > 0 && 
                        config.appliedMaterialIndex >= 0 && 
                        config.appliedMaterialIndex < config.materials.Count)
                    {
                        data.IsApplied = true;
                        data.AppliedMaterial = config.materials[config.appliedMaterialIndex];
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
                    rendererPath = data.Renderer != null ? 
                        GetRelativePath(_targetComponent.transform, data.Renderer.transform) : "",
                    selectedMaterialIndex = data.SelectedMaterialIndex,
                    appliedMaterialIndex = data.IsApplied ? data.AppliedMaterialIndex : -1,
                    materials = new List<Material>(data.Materials),
                    isFoldout = data.IsFoldout
                };

                _targetComponent.rendererConfigs.Add(config);
            }

            EditorUtility.SetDirty(_targetComponent);
        }

        private void OnDisable()
        {
            // 先退出所有预览状态
            ResetAllPreviews();
            
            // 再保存数据
            SaveToComponent();
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
                    if (data.Renderer != null && data.Materials.Count > 0)
                    {
                        string materialName = null;
                        
                        // 在预览模式下，显示当前预览的材质
                        if (data.IsPreviewMode && data.PreviewMaterialIndex >= 0 && data.PreviewMaterialIndex < data.Materials.Count)
                        {
                            var previewMaterial = data.Materials[data.PreviewMaterialIndex];
                            if (previewMaterial != null)
                            {
                                materialName = $"{previewMaterial.name} (Preview)";
                            }
                        }
                        // 不在预览模式时，显示已应用的材质
                        else if (data.IsApplied && data.AppliedMaterial != null)
                        {
                            materialName = data.AppliedMaterial.name;
                        }

                        if (!string.IsNullOrEmpty(materialName))
                        {
                            titleText += $" - {materialName}";
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
                        EditorGUI.BeginDisabledGroup(!data.IsPreviewMode || data.PreviewMaterialIndex <= 0);
                        if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.PrevKey"), GUILayout.Width(24), GUILayout.Height(20)))
                        {
                            data.SwitchMaterial(-1);
                        }
                        EditorGUI.EndDisabledGroup();

                        // 下一个材质
                        EditorGUI.BeginDisabledGroup(!data.IsPreviewMode || data.Materials.Count == 0 || data.PreviewMaterialIndex >= data.Materials.Count - 1);
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

        private void ResetAllPreviews()
        {
            foreach (var data in _rendererDatas)
            {
                data.ResetToOriginal();
            }
            _globalPreviewMode = false;
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
            private bool _isPreviewMode;
            public bool IsPreviewMode
            {
                get => _isPreviewMode;
                set
                {
                    if (_isPreviewMode == value) return;
                    
                    _isPreviewMode = value;
                    
                    if (Renderer == null) return;

                    if (_isPreviewMode)
                    {
                        // 开启预览时
                        // 1. 保存当前材质作为恢复用
                        var currentMaterials = Renderer.sharedMaterials;
                        OriginalMaterial = currentMaterials[SelectedMaterialIndex];
                        
                        // 2. 如果之前没有预览过，初始化预览索引为已应用的索引或0
                        if (_previewMaterialIndex < 0)
                        {
                            _previewMaterialIndex = IsApplied ? AppliedMaterialIndex : 0;
                        }
                        
                        // 3. 应用预览材质
                        PreviewMaterial();
                    }
                    else
                    {
                        // 关闭预览时，恢复到预览前的材质状态
                        var materials = Renderer.sharedMaterials;
                        materials[SelectedMaterialIndex] = OriginalMaterial;
                        Renderer.sharedMaterials = materials;
                        
                        // 重置预览索引
                        _previewMaterialIndex = -1;
                    }
                }
            }
            public Material OriginalMaterial;
            public Material AppliedMaterial;
            public bool IsApplied;

            private int _previewMaterialIndex = -1;
            public int PreviewMaterialIndex 
            {
                get => _previewMaterialIndex;
                set
                {
                    if (_previewMaterialIndex == value) return;
                    _previewMaterialIndex = value;
                    if (IsPreviewMode)
                    {
                        PreviewMaterial();
                    }
                }
            }

            // 添加属性来访问应用的材质索引
            public int AppliedMaterialIndex
            {
                get => _config.appliedMaterialIndex;
                private set => _config.appliedMaterialIndex = value;
            }

            public RendererData(MaterialSwitcher.RendererConfig config, MaterialSwitcher component)
            {
                _config = config;
                _component = component;
                _window = EditorWindow.GetWindow<MaterialSwitcherEditorWindow>();
                
                // 初始化预览索引为-1（未预览状态）
                _previewMaterialIndex = -1;
                
                // 初始化应用状态
                IsApplied = config.materials.Count > 0 && 
                            config.appliedMaterialIndex >= 0 && 
                            config.appliedMaterialIndex < config.materials.Count;
                
                if (IsApplied)
                {
                    AppliedMaterial = config.materials[config.appliedMaterialIndex];
                }
                
                InitializeList();
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

                MaterialList.OnAdd += _ => 
                {
                    Materials.Add(null);
                    CheckMaterialIndexes(); // 添加时检查
                };

                MaterialList.OnRemove += list =>
                {
                    if (list.index >= 0 && list.index < Materials.Count)
                    {
                        Materials.RemoveAt(list.index);
                        CheckMaterialIndexes(); // 删除时检查
                    }
                };

                MaterialList.OnChanged += _ => 
                {
                    // 只在重排序时更新预览
                    if (IsPreviewMode && PreviewMaterialIndex >= 0)
                    {
                        PreviewMaterial();
                    }
                };

                MaterialList.RefreshElementHeights();
            }

            // 添加新方法来检查索引
            private void CheckMaterialIndexes()
            {
                // 如果删除了正在预览或应用的材质，重置状态
                if (PreviewMaterialIndex >= Materials.Count)
                {
                    PreviewMaterialIndex = Materials.Count - 1;
                }
                if (_config.appliedMaterialIndex >= Materials.Count)
                {
                    IsApplied = false;
                    _config.appliedMaterialIndex = -1;
                }
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
                if (!IsPreviewMode || Materials.Count == 0) return;
                
                // 如果当前预览索引无效，从0开始
                if (_previewMaterialIndex < 0)
                {
                    _previewMaterialIndex = 0;
                }
                
                PreviewMaterialIndex = Mathf.Clamp(
                    PreviewMaterialIndex + direction, 
                    0, 
                    Materials.Count - 1
                );
            }

            public void PreviewMaterial()
            {
                if (!IsPreviewMode || Renderer == null || Materials.Count == 0) return;

                var materials = Renderer.sharedMaterials;
                materials[SelectedMaterialIndex] = Materials[PreviewMaterialIndex];
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
                AppliedMaterial = Materials[PreviewMaterialIndex];
                AppliedMaterialIndex = PreviewMaterialIndex; // 使用属性
                
                // 更新当前材质
                var materials = Renderer.sharedMaterials;
                materials[SelectedMaterialIndex] = AppliedMaterial;
                Renderer.sharedMaterials = materials;
                
                // 更新原始材质为新应用的材质
                OriginalMaterial = AppliedMaterial;
                
                // 标记为已应用
                IsApplied = true;
                
                // 标记场景已改变
                EditorUtility.SetDirty(Renderer);
                if (_component != null)
                {
                    EditorUtility.SetDirty(_component);
                }
            }

            public void ResetToOriginal()
            {
                if (!IsPreviewMode) return;
                
                IsPreviewMode = false;
                PreviewMaterialIndex = -1;
                if (!IsApplied)
                {
                    var materials = Renderer.sharedMaterials;
                    materials[SelectedMaterialIndex] = OriginalMaterial;
                    Renderer.sharedMaterials = materials;
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