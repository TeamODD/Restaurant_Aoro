using System.Collections.Generic;
using UnityEngine;

public class CustomerCodexManager : MonoBehaviour
{
    public static CustomerCodexManager Instance;

    private Dictionary<string, CustomerCodexEntry> entries = new();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public Dictionary<string, CustomerCodexEntry> GetAll() => new(entries);

    public void LoadFrom(Dictionary<string, CustomerCodexEntry> saved)
    {
        entries = saved != null ? new(saved) : new();
    }

    public void MarkSeen(string customerId)
    {
        var e = GetOrCreate(customerId);
        e.seen = true;
    }

    public void Unlock(string customerId)
    {
        var e = GetOrCreate(customerId);
        e.seen = true;
        e.unlocked = true;
    }

    public void AddResult(string customerId, ResultType type)
    {
        var e = GetOrCreate(customerId);
        e.visitCount++;

        string key = type.ToString();
        if (e.resultCounts.ContainsKey(key)) e.resultCounts[key]++;
        else e.resultCounts[key] = 1;
    }

    private CustomerCodexEntry GetOrCreate(string id)
    {
        if (!entries.TryGetValue(id, out var e))
        {
            e = new CustomerCodexEntry();
            entries[id] = e;
        }
        return e;
    }
}
