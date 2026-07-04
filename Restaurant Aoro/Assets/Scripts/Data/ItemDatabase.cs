using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    private Dictionary<string, Item> byId = new Dictionary<string, Item>();

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        LoadAllItems();
    }

    private void LoadAllItems()
    {
        byId.Clear();

        // Resources/Items �������� Item ScriptableObject ���� �ε�
        Item[] items = Resources.LoadAll<Item>("Items");

        foreach (var item in items)
        {
            if (item == null) continue;
            if (string.IsNullOrEmpty(item.ItemID)) continue;

            if (!byId.ContainsKey(item.ItemID))
                byId.Add(item.ItemID, item);
            else
                Debug.LogWarning($"[ItemDatabase] �ߺ� ItemID �߰�: {item.ItemID} ({item.name})");
        }

        Debug.Log($"[ItemDatabase] Loaded Items: {byId.Count}");
    }

    public Item GetItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return null;
        byId.TryGetValue(itemId, out var item);
        return item;
    }

    public IEnumerable<Item> GetAllItems()
    {
        return byId.Values;
    }
}
