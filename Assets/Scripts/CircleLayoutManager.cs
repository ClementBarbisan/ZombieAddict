using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CircleLayoutManager : MonoBehaviour
{
    [Header("Ellipse")]
    public float radiusX = 300f;
    public float radiusY = 150f;
    public float startAngleOffset = 180f; // 90 = first item at top

    [Header("Spawn")]
    public float scaleDuration = 0.35f;
    public Ease scaleEase = Ease.OutBack;

    [Header("Reposition")]
    public float repositionDuration = 0.45f;
    public Ease repositionEase = Ease.OutCubic;

    private readonly List<RectTransform> _items = new();

    void Start()
    {
        // Collect children already present in the hierarchy at startup
        foreach (Transform child in transform)
        {
            var rt = child.GetComponent<RectTransform>();
            if (rt != null) _items.Add(rt);
        }

        RefreshLayout(animateAll: true, newItem: null);
    }

    /// <summary>Call this to register and animate a newly instantiated item.</summary>
    public void AddItem(RectTransform item)
    {
        item.SetParent(transform, worldPositionStays: false);
        item.anchoredPosition = Vector2.zero;
        item.localScale = Vector3.zero;

        _items.Add(item);

        // Reposition all existing items, then scale the new one in
        RefreshLayout(animateAll: true, newItem: item);

        item.DOScale(Vector3.one, scaleDuration)
            .SetEase(scaleEase);
    }

public void StartGame()
{
    foreach (var item in _items)
    {
        item.DOScale(Vector3.zero, scaleDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => item.gameObject.SetActive(false));
    }
}

    void RefreshLayout(bool animateAll, RectTransform newItem)
    {
        int count = _items.Count;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            RectTransform rt = _items[i];
            Vector2 target = GetPositionOnCircle(i, count);

            // New item snaps to position immediately (scale tween handles its reveal)
            if (rt == newItem)
            {
                rt.anchoredPosition = target;
                continue;
            }

            if (animateAll)
                rt.DOAnchorPos(target, repositionDuration).SetEase(repositionEase);
            else
                rt.anchoredPosition = target;
        }
    }

    Vector2 GetPositionOnCircle(int index, int total)
{
    float angleDeg = startAngleOffset + (360f / total) * index;
    float angleRad = angleDeg * Mathf.Deg2Rad;
    return new Vector2(Mathf.Cos(angleRad) * radiusX, Mathf.Sin(angleRad) * radiusY);
}

    /// <summary>Remove an item and reflow the rest.</summary>
    public void RemoveItem(RectTransform item)
    {
        if (!_items.Remove(item)) return;
        Destroy(item.gameObject);
        RefreshLayout(animateAll: true, newItem: null);
    }
}