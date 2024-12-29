using UnityEngine;
using UnityEditor;
using Yueby.Utils;
using Yueby.ModalWindow;

namespace Yueby
{
    public class MirrorToolSettingsDrawer : ModalEditorWindowDrawer<object>
    {
        public MirrorToolSettingsDrawer()
        {
            this.Title = "Mirror Tool Settings";

            // 设置窗口高度
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = 5f;
            float helpBoxHeight = 40f;
            float buttonHeight = 25f;

            // 计算总高度
            float totalHeight = lineHeight * 2 +     // 标题
                              helpBoxHeight +        // HelpBox
                              buttonHeight +         // 删除按钮
                              EditorGUIUtility.singleLineHeight * 3;           // 间距

            position = new Rect(0, 0, 300, totalHeight);
        }

        public override void OnDraw()
        {
            EditorUI.VerticalEGL(() =>
            {
                // 可视化设置
                EditorUI.VerticalEGL(new GUIStyle("Badge"), () =>
                {
                    EditorUI.TitleLabelField("Visualization");
                    MirrorTool.ShowMirrorAxis = EditorUI.Toggle(MirrorTool.ShowMirrorAxis, "Show Mirror Axis");
                    EditorGUILayout.Space(2);
                    EditorGUILayout.HelpBox("Show or hide mirror axis in scene view", MessageType.Info);
                });

                EditorGUILayout.Space(5);

                // 数据管理
                EditorUI.VerticalEGL(new GUIStyle("Badge"), () =>
                {
                    EditorUI.TitleLabelField("Data Management");
                    if (GUILayout.Button("Clear All Mirror Data", GUILayout.Height(25)))
                    {
                        if (EditorUtility.DisplayDialog("Clear Mirror Data",
                            "Are you sure you want to clear all mirror data? This action cannot be undone.",
                            "Yes", "No"))
                        {
                            MirrorTool.ClearAllData();
                            Close();
                        }
                    }
                });
            });
        }
    }
}