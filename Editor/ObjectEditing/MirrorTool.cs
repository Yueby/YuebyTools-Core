using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Yueby.Core.Utils;
using Yueby.Utils;
using Yueby.ModalWindow;

namespace Yueby
{
    [InitializeOnLoad]
    public class MirrorTool
    {
        // 镜像对象配置字典
        private static Dictionary<string, MirrorConfig> mirrorConfigs = new Dictionary<string, MirrorConfig>();
        private static GameObject currentSelectedObject;
        private static bool isWindowVisible = false;
        private static Vector2 windowPosition;
        private static readonly Vector2 windowSize = new Vector2(150, 240);
        private static Rect windowRect;
        private static readonly string configPath;
        private static readonly string enableKey = "MirrorTool_Enabled";
        private static bool isEnabled;
        private static bool showMirrorAxis = true;
        private static readonly Color themeColor = new Color(1f, 0.92f, 0.016f, 0.5f);
        private static PivotRotation lastPivotRotation;

        // 公共访问器
        public static bool ShowMirrorAxis
        {
            get => showMirrorAxis;
            set => showMirrorAxis = value;
        }

        // 配置数据类
        [System.Serializable]
        public class MirrorConfig
        {
            public List<string> targetObjectPaths = new List<string>();
            public Vector3 mirrorAxis = Vector3.right;

            public MirrorConfig(string targetPath, Vector3 axis)
            {
                targetObjectPaths = new List<string> { targetPath };
                mirrorAxis = axis;
            }

            public MirrorConfig()
            {
                targetObjectPaths = new List<string>();
                mirrorAxis = Vector3.right;
            }
        }

        static MirrorTool()
        {
            string projectPath = Application.dataPath;
            projectPath = projectPath.Substring(0, projectPath.Length - 6);
            configPath = System.IO.Path.Combine(projectPath, "ProjectSettings", "MirrorTool.json");

            isEnabled = EditorPrefs.GetBool(enableKey, false);
            if (isEnabled)
            {
                SceneView.duringSceneGui += OnSceneGUI;
            }
            LoadConfigs();
        }

        [MenuItem("Tools/YuebyTools/Utils/Toggle Mirror Tool %#m", false, 1)]
        private static void ToggleTool()
        {
            isEnabled = !isEnabled;
            EditorPrefs.SetBool(enableKey, isEnabled);

            if (isEnabled)
            {
                SceneView.duringSceneGui += OnSceneGUI;
                YuebyLogger.LogInfo($"Mirror Tool Enabled (Shortcut: Ctrl+Shift+M)");
            }
            else
            {
                SceneView.duringSceneGui -= OnSceneGUI;
                isWindowVisible = false;
                YuebyLogger.LogInfo($"Mirror Tool Disabled (Shortcut: Ctrl+Shift+M)");
            }

            SceneView.RepaintAll();
        }

        [MenuItem("Tools/YuebyTools/Utils/Toggle Mirror Tool %#m", true)]
        private static bool ValidateToggleTool()
        {
            Menu.SetChecked("Tools/YuebyTools/Utils/Toggle Mirror Tool %#m", isEnabled);
            return true;
        }

        private static void BreakMirrorConnection(GameObject source, GameObject target)
        {
            if (source == null || target == null) return;
            
            string sourcePath = GetObjectPath(source);
            string targetPath = GetObjectPath(target);

            // 从源对象配置中移除目标路径
            if (mirrorConfigs.TryGetValue(sourcePath, out var sourceConfig))
            {
                sourceConfig.targetObjectPaths.Remove(targetPath);
                if (sourceConfig.targetObjectPaths.Count == 0)
                {
                    mirrorConfigs.Remove(sourcePath);
                }
            }

            // 从目标对象配置中移除源路径
            if (mirrorConfigs.TryGetValue(targetPath, out var targetConfig))
            {
                targetConfig.targetObjectPaths.Remove(sourcePath);
                if (targetConfig.targetObjectPaths.Count == 0)
                {
                    mirrorConfigs.Remove(targetPath);
                }
            }

            SaveConfigs();
            SceneView.RepaintAll();
        }

        public static void EstablishMirrorConnection(GameObject source, GameObject target, Vector3 axis)
        {
            if (source == null || target == null) return;

            string sourcePath = GetObjectPath(source);
            string targetPath = GetObjectPath(target);

            // 为源对象建立连接
            if (!mirrorConfigs.ContainsKey(sourcePath))
            {
                mirrorConfigs[sourcePath] = new MirrorConfig();
            }
            if (!mirrorConfigs[sourcePath].targetObjectPaths.Contains(targetPath))
            {
                mirrorConfigs[sourcePath].targetObjectPaths.Add(targetPath);
            }
            mirrorConfigs[sourcePath].mirrorAxis = axis;

            // 为目标对象建立反向连接
            if (!mirrorConfigs.ContainsKey(targetPath))
            {
                mirrorConfigs[targetPath] = new MirrorConfig();
            }
            if (!mirrorConfigs[targetPath].targetObjectPaths.Contains(sourcePath))
            {
                mirrorConfigs[targetPath].targetObjectPaths.Add(sourcePath);
            }
            mirrorConfigs[targetPath].mirrorAxis = axis;

            SaveConfigs();
            SceneView.RepaintAll();
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (lastPivotRotation != Tools.pivotRotation)
            {
                lastPivotRotation = Tools.pivotRotation;
                SceneView.RepaintAll();
            }

            if (Selection.gameObjects.Length != 1)
            {
                isWindowVisible = false;
                currentSelectedObject = null;
                targetList = null;
                return;
            }

            GameObject selectedObject = Selection.gameObjects[0];

            if (currentSelectedObject != selectedObject)
            {
                currentSelectedObject = selectedObject;
                windowPosition = new Vector2(10, 10);
                windowRect = new Rect(windowPosition, windowSize);
                isWindowVisible = true;

                // 初始化或更新目标列表
                string sourcePath = GetObjectPath(currentSelectedObject);
                if (!mirrorConfigs.ContainsKey(sourcePath))
                {
                    mirrorConfigs[sourcePath] = new MirrorConfig();
                }

                InitializeTargetList(mirrorConfigs[sourcePath]);
            }

            if (!isWindowVisible) return;

            Handles.BeginGUI();

            windowRect = GUILayout.Window(0, windowRect, DrawMirrorWindow, "Mirror Tool");
            windowPosition = windowRect.position;

            Handles.EndGUI();

            if (currentSelectedObject != null && mirrorConfigs.TryGetValue(GetObjectPath(currentSelectedObject), out MirrorConfig config))
            {
                foreach (var targetPath in config.targetObjectPaths)
                {
                    GameObject targetObj = FindObjectByPath(targetPath);
                    if (targetObj != null)
                    {
                        ApplyMirror(currentSelectedObject, targetObj, config.mirrorAxis);

                        if (showMirrorAxis)
                        {
                            Vector3 targetPos = targetObj.transform.position;
                            float handleSize = HandleUtility.GetHandleSize(targetPos) * 0.15f;

                            Vector3 rightDir = Tools.pivotRotation == PivotRotation.Local ? targetObj.transform.right : Vector3.right;
                            Vector3 upDir = Tools.pivotRotation == PivotRotation.Local ? targetObj.transform.up : Vector3.up;
                            Vector3 forwardDir = Tools.pivotRotation == PivotRotation.Local ? targetObj.transform.forward : Vector3.forward;

                            Handles.color = Color.red;
                            Handles.ArrowHandleCap(0, targetPos, Quaternion.LookRotation(rightDir), handleSize * 2f, EventType.Repaint);

                            Handles.color = Color.green;
                            Handles.ArrowHandleCap(0, targetPos, Quaternion.LookRotation(upDir), handleSize * 2f, EventType.Repaint);

                            Handles.color = Color.blue;
                            Handles.ArrowHandleCap(0, targetPos, Quaternion.LookRotation(forwardDir), handleSize * 2f, EventType.Repaint);

                            Handles.color = new Color(1f, 1f, 1f, 0.5f);
                            Handles.DrawDottedLine(currentSelectedObject.transform.position, targetPos, 2f);
                        }
                    }
                }
            }
        }

        private static void InitializeTargetList(MirrorConfig config)
        {
            if (config.targetObjectPaths == null)
            {
                config.targetObjectPaths = new List<string>();
            }

            targetList = new ReorderableListDroppable(config.targetObjectPaths, typeof(string), EditorGUIUtility.singleLineHeight, () => SceneView.RepaintAll());
            targetList.OnDraw = (rect, index, active, focused) =>
            {
                if (index < 0 || index >= config.targetObjectPaths.Count) return EditorGUIUtility.singleLineHeight;

                var targetPath = config.targetObjectPaths[index];
                var targetObj = FindObjectByPath(targetPath);
                var newTargetObj = EditorGUI.ObjectField(rect, targetObj, typeof(GameObject), true) as GameObject;
                
                if (newTargetObj != targetObj)
                {
                    if (newTargetObj != null)
                    {
                        // 如果是替换，先断开旧的连接
                        if (targetObj != null)
                        {
                            BreakMirrorConnection(currentSelectedObject, targetObj);
                        }
                        
                        string newTargetPath = GetObjectPath(newTargetObj);
                        config.targetObjectPaths[index] = newTargetPath;
                        EstablishMirrorConnection(currentSelectedObject, newTargetObj, config.mirrorAxis);
                    }
                    else
                    {
                        // 如果是删除，断开连接
                        if (targetObj != null)
                        {
                            BreakMirrorConnection(currentSelectedObject, targetObj);
                        }
                    }
                }
                return EditorGUIUtility.singleLineHeight;
            };

            targetList.OnAdd = (list) =>
            {
                config.targetObjectPaths.Add("");
                SaveConfigs();
            };

            targetList.OnRemove = (list) =>
            {
                // OnRemove不需要做额外操作，因为OnRemoveBefore已经处理了断开连接
                SaveConfigs();
            };

            targetList.OnRemoveBefore = (index) =>
            {
                if (index >= 0 && index < config.targetObjectPaths.Count)
                {
                    var targetPath = config.targetObjectPaths[index];
                    var targetObj = FindObjectByPath(targetPath);
                    if (targetObj != null)
                    {
                        BreakMirrorConnection(currentSelectedObject, targetObj);
                    }
                }
            };
        }

        private static void DrawMirrorWindow(int id)
        {
            if (currentSelectedObject == null) return;

            // 根据主题选择合适的图标
            string settingsIcon = EditorGUIUtility.isProSkin ? "d_Settings" : "Settings";
            string closeIcon = EditorGUIUtility.isProSkin ? "d_winbtn_win_close" : "winbtn_win_close";

            float iconSize = EditorGUIUtility.singleLineHeight;
            float padding = 4;

            // 在标题栏绘制按钮
            Rect settingsRect = new Rect(
                windowRect.width - iconSize * 2 - padding * 2,
                padding,
                iconSize,
                iconSize
            );

            Rect closeRect = new Rect(
                windowRect.width - iconSize - padding,
                padding,
                iconSize,
                iconSize
            );

            GUIContent settingsContent = EditorGUIUtility.IconContent(settingsIcon);
            if (GUI.Button(settingsRect, settingsContent, EditorStyles.iconButton))
            {
                var drawer = new MirrorToolSettingsDrawer();
                ModalEditorWindow.ShowUtility(drawer);
            }

            GUIContent closeContent = EditorGUIUtility.IconContent(closeIcon);
            if (GUI.Button(closeRect, closeContent, EditorStyles.iconButton))
            {
                isWindowVisible = false;
            }

            EditorUI.VerticalEGL(() =>
            {
                EditorUI.VerticalEGL(new GUIStyle("Badge"), () =>
                {
                    string sourcePath = GetObjectPath(currentSelectedObject);
                    if (!mirrorConfigs.ContainsKey(sourcePath))
                    {
                        mirrorConfigs[sourcePath] = new MirrorConfig();
                        InitializeTargetList(mirrorConfigs[sourcePath]);
                    }

                    MirrorConfig config = mirrorConfigs[sourcePath];

                    // 轴向选择
                    EditorUI.HorizontalEGL(() =>
                    {
                        EditorGUILayout.LabelField("Axis", GUILayout.Width(45));

                        int selectedAxis = 0;
                        if (Vector3.Dot(config.mirrorAxis.normalized, Vector3.up) > 0.99f)
                            selectedAxis = 1;
                        else if (Vector3.Dot(config.mirrorAxis.normalized, Vector3.forward) > 0.99f)
                            selectedAxis = 2;

                        int newSelection = GUILayout.SelectionGrid(selectedAxis, new string[] { "X", "Y", "Z" }, 3, EditorStyles.miniButton);

                        if (newSelection != selectedAxis)
                        {
                            switch (newSelection)
                            {
                                case 0:
                                    config.mirrorAxis = Vector3.right;
                                    break;
                                case 1:
                                    config.mirrorAxis = Vector3.up;
                                    break;
                                case 2:
                                    config.mirrorAxis = Vector3.forward;
                                    break;
                            }
                        }
                    });

                    EditorUI.DrawCheckChanged(
                        () => config.mirrorAxis = EditorGUILayout.Vector3Field("Direction", config.mirrorAxis),
                        () =>
                        {
                            foreach (var targetPath in config.targetObjectPaths)
                            {
                                var targetObj = FindObjectByPath(targetPath);
                                if (targetObj != null)
                                {
                                    EstablishMirrorConnection(currentSelectedObject, targetObj, config.mirrorAxis);
                                }
                            }
                        }
                    );

                    EditorGUILayout.Space(5);

                    targetList?.DoLayout("Target Objects", new Vector2(0, 150), false, false, true, (objects) =>
                    {
                        foreach (var obj in objects)
                        {
                            if (obj is GameObject gameObj)
                            {
                                string targetPath = GetObjectPath(gameObj);
                                if (!config.targetObjectPaths.Contains(targetPath))
                                {
                                    config.targetObjectPaths.Add(targetPath);
                                    EstablishMirrorConnection(currentSelectedObject, gameObj, config.mirrorAxis);
                                }
                            }
                        }
                        SaveConfigs();
                    });
                });
            });

            GUI.DragWindow();
        }

        private static void ApplyMirror(GameObject source, GameObject target, Vector3 axis)
        {
            if (source == null || target == null) return;

            Vector3 sourcePos = source.transform.position;
            Vector3 mirrorPos = Vector3.Reflect(sourcePos, axis.normalized);
            target.transform.position = mirrorPos;

            Quaternion sourceRot = source.transform.rotation;
            Vector3 forward = Vector3.Reflect(sourceRot * Vector3.forward, axis.normalized);
            Vector3 up = Vector3.Reflect(sourceRot * Vector3.up, axis.normalized);
            target.transform.rotation = Quaternion.LookRotation(forward, up);

            target.transform.localScale = source.transform.localScale;
        }

        private static void SaveConfigs()
        {
            try
            {
                CleanEmptyConfigs();
                string json = JsonUtility.ToJson(new SerializableDict(mirrorConfigs), true);
                System.IO.File.WriteAllText(configPath, json);
            }
            catch (System.Exception e)
            {
                YuebyLogger.LogError($"Failed to save Mirror Tool config: {e.Message}");
            }
        }

        private static void LoadConfigs()
        {
            try
            {
                if (System.IO.File.Exists(configPath))
                {
                    string json = System.IO.File.ReadAllText(configPath);
                    SerializableDict serializableDict = JsonUtility.FromJson<SerializableDict>(json);
                    mirrorConfigs = serializableDict.ToDictionary();
                }
                else
                {
                    mirrorConfigs = new Dictionary<string, MirrorConfig>();
                }
            }
            catch (System.Exception e)
            {
                YuebyLogger.LogError($"Failed to load Mirror Tool config: {e.Message}");
                mirrorConfigs = new Dictionary<string, MirrorConfig>();
            }
        }

        public static string GetObjectPath(GameObject obj)
        {
            if (obj == null) return "";
            string path = obj.name;
            Transform parent = obj.transform.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            return path;
        }

        public static GameObject FindObjectByPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            return GameObject.Find(path);
        }

        [System.Serializable]
        private class SerializableDict
        {
            public List<string> keys = new List<string>();
            public List<MirrorConfig> values = new List<MirrorConfig>();

            public SerializableDict(Dictionary<string, MirrorConfig> dict)
            {
                foreach (var kvp in dict)
                {
                    keys.Add(kvp.Key);
                    values.Add(kvp.Value);
                }
            }

            public Dictionary<string, MirrorConfig> ToDictionary()
            {
                Dictionary<string, MirrorConfig> dict = new Dictionary<string, MirrorConfig>();
                for (int i = 0; i < keys.Count; i++)
                {
                    dict[keys[i]] = values[i];
                }
                return dict;
            }
        }

        private static ReorderableListDroppable targetList;

        public static void ClearAllData()
        {
            mirrorConfigs.Clear();
            SaveConfigs();
            SceneView.RepaintAll();
            YuebyLogger.LogInfo("Mirror Tool data cleared.");
        }

        private static void CleanEmptyConfigs()
        {
            var keysToRemove = mirrorConfigs.Where(kvp => kvp.Value.targetObjectPaths.Count == 0)
                                          .Select(kvp => kvp.Key)
                                          .ToList();
            
            foreach (var key in keysToRemove)
            {
                mirrorConfigs.Remove(key);
            }
        }
    }
}