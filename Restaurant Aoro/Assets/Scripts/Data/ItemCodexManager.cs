using System.Collections.Generic;
using UnityEngine;

public class ItemCodexManager : MonoBehaviour
{
    public static ItemCodexManager Instance;

    private Dictionary<string, ItemCodexEntry> entries = new();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public Dictionary<string, ItemCodexEntry> GetAll() => new(entries);

    public void LoadFrom(Dictionary<string, ItemCodexEntry> saved)
    {
        entries = saved != null ? new(saved) : new();
    }

    public void MarkSeen(string itemId)
    {
        var e = GetOrCreate(itemId);
        e.seen = true;
    }

    public void Unlock(string itemId)
    {
        var e = GetOrCreate(itemId);
        e.seen = true;
        e.unlocked = true;
    }

    private ItemCodexEntry GetOrCreate(string id)
    {
        if (!entries.TryGetValue(id, out var e))
        {
            e = new ItemCodexEntry();
            entries[id] = e;
        }
        return e;
    }
}
