using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using JetBrains.Annotations;

public class SelectionUtil
{
    private static void UpdateSelection(ICollection<GameObject> newSelection, ICollection<string> failedObjects, string failMessage)
    {
        if (newSelection.Count > 0)
        {
            var objectsToSelect = newSelection.ToArray();
            EditorApplication.delayCall += () =>
            {
                Selection.objects = objectsToSelect;
            };

            if (failedObjects.Count > 0)
            {
                Debug.LogWarning($"以下对象{failMessage}：\n{string.Join("\n", failedObjects)}");
            }
        }
        else
        {
            Debug.LogWarning($"所选对象都{failMessage}");
        }
    }

    [MenuItem("GameObject/YuebyTools/Move Selection Down &.")] // Alt+. (>)
    public static void MoveSelectionDown()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        List<GameObject> newSelection = new List<GameObject>();
        List<string> noChildObjects = new List<string>();

        foreach (GameObject obj in selectedObjects)
        {
            if (obj.transform.childCount > 0)
            {
                Transform firstChild = obj.transform.GetChild(0);
                newSelection.Add(firstChild.gameObject);
            }
            else
            {
                noChildObjects.Add(obj.name);
            }
        }

        UpdateSelection(newSelection, noChildObjects, "没有子对象");
    }

    [MenuItem("GameObject/YuebyTools/Expand Selection Down #&.")] // Shift+Alt+. (>)
    public static void ExpandSelectionDown()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        List<GameObject> newSelection = new List<GameObject>();
        List<string> noChildObjects = new List<string>();

        // 保留原有选择
        newSelection.AddRange(selectedObjects);

        foreach (GameObject obj in selectedObjects)
        {
            if (obj.transform.childCount > 0)
            {
                Transform firstChild = obj.transform.GetChild(0);
                newSelection.Add(firstChild.gameObject);
            }
            else
            {
                noChildObjects.Add(obj.name);
            }
        }

        UpdateSelection(newSelection, noChildObjects, "没有子对象");
    }

    [MenuItem("GameObject/YuebyTools/Move Selection Up &,")] // Alt+, (<)
    public static void MoveSelectionUp()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        HashSet<GameObject> newSelection = new HashSet<GameObject>();
        List<string> noParentObjects = new List<string>();

        foreach (GameObject obj in selectedObjects)
        {
            if (obj.transform.parent != null)
            {
                newSelection.Add(obj.transform.parent.gameObject);
            }
            else
            {
                noParentObjects.Add(obj.name);
            }
        }

        UpdateSelection(newSelection, noParentObjects, "没有父对象");
    }

    [MenuItem("GameObject/YuebyTools/Shrink Selection Up #&,")] // Shift+Alt+, (<)
    public static void ShrinkSelectionUp()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        HashSet<GameObject> newSelection = new HashSet<GameObject>();
        List<string> noParentObjects = new List<string>();

        // 找出最深层级的对象
        HashSet<GameObject> deepestObjects = new HashSet<GameObject>(selectedObjects);
        foreach (GameObject obj in selectedObjects)
        {
            Transform parent = obj.transform.parent;
            while (parent != null)
            {
                if (deepestObjects.Contains(parent.gameObject))
                {
                    // 如果父对象在选择中，说明当前对象是更深的层级
                    deepestObjects.Remove(parent.gameObject);
                }
                parent = parent.parent;
            }
        }

        // 移除最深层级的对象，保留其父对象
        foreach (GameObject obj in selectedObjects)
        {
            if (!deepestObjects.Contains(obj))
            {
                newSelection.Add(obj);
            }
            else if (obj.transform.parent != null)
            {
                newSelection.Add(obj.transform.parent.gameObject);
            }
            else
            {
                noParentObjects.Add(obj.name);
            }
        }

        UpdateSelection(newSelection, noParentObjects, "没有父对象");
    }

    [MenuItem("GameObject/YuebyTools/Move Selection Down &.", true)]
    private static bool ValidateMoveSelectionDown()
    {
        return Selection.gameObjects.Length > 0;
    }

    [MenuItem("GameObject/YuebyTools/Move Selection Up &,", true)]
    private static bool ValidateMoveSelectionUp()
    {
        return Selection.gameObjects.Length > 0;
    }

    [MenuItem("GameObject/YuebyTools/Expand Selection Down #&.", true)]
    private static bool ValidateExpandSelectionDown()
    {
        return Selection.gameObjects.Length > 0;
    }

    [MenuItem("GameObject/YuebyTools/Shrink Selection Up #&,", true)]
    private static bool ValidateShrinkSelectionUp()
    {
        return Selection.gameObjects.Length > 0;
    }
}
