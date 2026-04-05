using UnityEngine;
using UnityEngine.InputSystem;

public class CircleLayoutTester : MonoBehaviour
{
    public CircleLayoutManager manager;
    public RectTransform itemPrefab;

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            SpawnItem();

        if (Mouse.current.rightButton.wasPressedThisFrame)
            RemoveLast();

        if (Mouse.current.middleButton.wasPressedThisFrame)
            manager.StartGame();
    }

    void SpawnItem()
    {
        if (itemPrefab == null || manager == null) return;
        var item = Instantiate(itemPrefab);
        manager.AddItem(item);
    }

    void RemoveLast()
    {
        var field = typeof(CircleLayoutManager)
            .GetField("_items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var list = field?.GetValue(manager) as System.Collections.Generic.List<UnityEngine.RectTransform>;
        if (list == null || list.Count == 0) return;
        manager.RemoveItem(list[^1]);
    }
}