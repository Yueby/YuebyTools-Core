using UnityEditor;
using UnityEngine;

namespace Yueby
{
    [CustomEditor(typeof(MaterialSwitcher))]
    public class MaterialSwitcherInspector : Editor
    {
        private Vector2 _scrollPosition;
        private bool _showConfigs = true;

        public override void OnInspectorGUI()
        {
            var component = (MaterialSwitcher)target;

            // 显示配置名称
            EditorGUILayout.LabelField("Config Name", component.configName,"Badge");
        
            EditorGUILayout.Space(5);

            if (GUILayout.Button("Open Material Switcher"))
            {
                var window = EditorWindow.GetWindow<MaterialSwitcherEditorWindow>();
                window.titleContent = new GUIContent("Material Switcher");
                window.InitializeWithComponent(component);
                window.Show();
            }

            EditorGUILayout.Space(5);

            // 显示配置预览
            _showConfigs = EditorGUILayout.Foldout(_showConfigs, "Renderer Configs", true);
            if (_showConfigs && component.rendererConfigs != null)
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.MaxHeight(300));
                EditorGUI.indentLevel++;

                for (int i = 0; i < component.rendererConfigs.Count; i++)
                {
                    var config = component.rendererConfigs[i];
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    
                    // 配置标题和材质信息
                    var materialInfo = "";
                    if (config.materials.Count > 0 && config.appliedMaterialIndex >= 0 && config.appliedMaterialIndex < config.materials.Count)
                    {
                        var currentMaterial = config.materials[config.appliedMaterialIndex];
                        materialInfo = currentMaterial != null ? currentMaterial.name : "None";
                    }
                    
                    EditorGUILayout.LabelField(
                        $"{config.name} ({materialInfo})", 
                        EditorStyles.boldLabel
                    );
                    
                    // 路径
                    if (!string.IsNullOrEmpty(config.rendererPath))
                    {
                        EditorGUILayout.LabelField("Path:", config.rendererPath);
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(2);
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.EndScrollView();
            }
        }
    }
}