using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Threading.Tasks;

public static class UIHelpers
{

    /// <summary>
    /// Immedeatly calculate height of elemtent, without using Canvac Layout Update. 
    /// Attention: Not universal. Depth is only one element!
    /// </summary>
    /// <returns>The minimum height.</returns>
    /// <param name="listContent">List content.</param>
    public static float CalculateMinHeight(this RectTransform listContent)
    {
        float height = 0;

        int childCount = 0;
        foreach (Transform child in listContent.transform) if (child.gameObject.activeSelf)
            {
                height += child.GetComponent<RectTransform>().sizeDelta.y;
                childCount++;
            }

        VerticalLayoutGroup layoutGroup;
        if (layoutGroup = listContent.GetComponent<VerticalLayoutGroup>())
        {
            height += layoutGroup.spacing * (childCount - 1);
        }

        return height;

    }

    public static Rect Expand(this Rect rectBase, float heightAdd = 0f, float? widthAdd = null)
    {
        if (!widthAdd.HasValue)
            widthAdd = heightAdd;

        if (heightAdd == 0f && widthAdd.Value == 0f)
            return rectBase;

        var rect = new Rect(rectBase.x - rectBase.width * widthAdd.Value * 0.5f,   // Expand
                            rectBase.y - rectBase.height * heightAdd * 0.5f,
                            rectBase.width * (1 + widthAdd.Value),
                            rectBase.height * (1 + heightAdd));

        return rect;
    }

    public static bool IsVisibleAt(this RectTransform rt, RectTransform viewTransform, float expandHeightMnf = 0f, float expandWidthMnf = 0f)
    {
        var rect = viewTransform.GetWorldRect().Expand(expandHeightMnf, expandWidthMnf);

        return rect.Overlaps(rt.GetWorldRect(), true);

    }

    public static Rect GetWorldRect(this RectTransform rt, Vector2? scale = null)
    {

        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        Vector3 topLeft = corners[1];
        Vector3 bottomRight = corners[3];


        Vector2 scaledSize = bottomRight - topLeft;

        return new Rect(topLeft, scaledSize);
    }


    public static void ScrollTo(this Transform target, Vector3? offsetTarget = null)
    {
        Canvas.ForceUpdateCanvases();

        var scrollRect = target.GetComponentInParent<ScrollRect>();
        if (scrollRect)
        {
            var contentPanel = scrollRect.content;

            contentPanel.anchoredPosition =
                (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(target.position + offsetTarget??Vector3.zero);
        }
    }

    public static void VerticalScrollTo(this Transform target, Vector3? offsetTarget = null)
    {
        Canvas.ForceUpdateCanvases();

        var scrollRect = target.GetComponentInParent<ScrollRect>();
        if (scrollRect)
        {
            var contentPanel = scrollRect.content;
            var newPos = new Vector2(contentPanel.anchoredPosition.x, 
                ((Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(target.position + offsetTarget ?? Vector3.zero)).y);

            contentPanel.anchoredPosition = newPos;
        }
    }

    public static void VerticalScrollTo(this Transform transform, float padding = 0, float time = 0, AnimationCurve curve = null, bool localZfixed = false)
    {

        var scrollParent = transform.GetComponentInParent<ScrollRect>();
        if (scrollParent)
        {
            var topView = scrollParent.GetComponent<RectTransform>().GetWorldRect().yMin - padding;
            var topElement = transform.GetComponent<RectTransform>().GetWorldRect().yMin;

            var delta = topElement - topView;
            var from = scrollParent.content.transform.position.y;
            var to = from - delta;

            VerticalScrollProccess(scrollParent, from, to, time, curve, localZfixed);

            Debug.Log($"Scroll delta {delta} (from: {topElement}, to: {topView} )");
        }

    }

    private static async void VerticalScrollProccess(ScrollRect scrollParent, float from, float to, float time, AnimationCurve curve = null, bool localZfixed = false)
    {
        Func<bool> checkBreak = () => Input.GetMouseButton(0);

        if (curve == null)
            curve = AnimationCurve.Linear(0, 0, 1, 1);

        scrollParent.velocity = Vector2.zero;

        var transform = scrollParent.content.transform;
        Vector3 position = transform.position;
        float proccess = 0;
        if (time <= 0)
        {
            proccess = 1;
            time = 1; // can not divide Zero
        }

        do
        {
            proccess += Time.deltaTime / time;
            if (proccess > 1) proccess = 1f;

            position.y = Mathf.LerpUnclamped(from, to, curve.Evaluate(proccess));
            transform.position = position;

            if (localZfixed)
            {
                transform.localPosition = transform.localPosition.SetZ(0);
            }

            await Task.Delay(1); //yield return null; // wait next frame

        } while (proccess < 1 && !checkBreak());

    }

    public static void ForEach<T>(this T[] array, Action<T> action)
    {
        Array.ForEach(array, action);
    }

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (T item in enumerable)
        {
            action.Invoke(item);
        }
    }

    public static void ForEach<T>(this T[] array, Action<T, int> action)
    {
        int index = 0;
        Array.ForEach(array, el => action.Invoke(el, index++));
    }

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
    {
        int index = 0;
        foreach (T item in enumerable)
        {
            action.Invoke(item, index++);
        }
    }

    public static void Repeat(this int count, Action<int> action)
    {
        for (int i = 0; i < count; i++)
        {
            action.Invoke(i);
        }
    }

    public static Rect SetHeight(this Rect rect, float height)
    {
        return new Rect(rect.x, rect.y, rect.width, height);
    }

    public static Rect LeftOffset(this Rect rect, float offset)
    {
        return new Rect(rect.x + offset, rect.y, rect.width - offset, rect.height);
    }

    public static Rect RightOffset(this Rect rect, float offset)
    {
        return new Rect(rect.x, rect.y, rect.width - offset, rect.height);
    }

    /// <summary>
    /// Splits rect by horizontal. Width every parts will be associate to parts size. 
    /// If usage absolute mode, use 0 for expand size to remaining space
    /// </summary>
    /// <returns>Array with rects inside initial rect</returns>
    /// <param name="rect">Rect.</param>
    /// <param name="parts">associate to parts sizes.</param>
    /// <param name="absolute">If set to <c>true</c> absolute.</param>
    /// <param name="relativePartMin">Part minimum size for remaining space</param>
    public static Rect[] SplitHorizontal(this Rect rect, float[] parts, bool absolute = false, float relativePartMin = 10f)
    {
        if (absolute)
        {
            int relativeCount = parts.Count(v => v <= 0);
            float fixedLength = parts.Sum(v => v > 0 ? v : 0);

            float relativeLength = Mathf.Max(relativePartMin, rect.width - fixedLength);

            parts = parts.Select(v => v > 0 ? v : (relativeLength / relativeCount)).ToArray();

        }
        else
        {

            float fullLength = parts.Sum(v => v);

            // Transmit to absolute values
            parts = parts.Select(v => rect.width * (v / fullLength)).ToArray();
        }


        float offset = 0;
        Rect[] rects = new Rect[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            rects[i] = new Rect(rect.x + offset, rect.y, parts[i], rect.height);
            offset += parts[i];
        }

        return rects;
    }

    public static TVal GetOrDefault<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey key, TVal def = default(TVal))
    {
        if (dict.ContainsKey(key))
            return dict[key];

        return def;
    }

    public static Vector3IntSerializable ToSerializable(this Vector3Int vector) => new Vector3IntSerializable(vector);
}


[Serializable]
public struct Vector3IntSerializable
{
    public int x;
    public int y;
    public int z;

    public Vector3Int ValueVector3Int => new Vector3Int(x, y, z);

    public Vector3IntSerializable(int x, int y, int z = 0)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3IntSerializable(Vector3Int vector) : this(vector.x, vector.y, vector.z)
    {
    }
}