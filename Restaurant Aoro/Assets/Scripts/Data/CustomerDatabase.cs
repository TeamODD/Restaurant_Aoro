using System.Collections.Generic;
using UnityEngine;

public class CustomerDatabase : MonoBehaviour
{
    public static CustomerDatabase Instance;

    private Dictionary<string, Customer> byId = new();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        LoadAll();
    }

    private void LoadAll()
    {
        byId.Clear();
        var customers = Resources.LoadAll<Customer>("Customers");

        foreach (var c in customers)
        {
            if (c == null || string.IsNullOrEmpty(c.CustomerID)) continue;

            if (!byId.ContainsKey(c.CustomerID))
                byId.Add(c.CustomerID, c);
            else
                Debug.LogWarning($"[CustomerDB] Duplicate CustomerID: {c.CustomerID}");
        }

        Debug.Log($"[CustomerDB] Loaded: {byId.Count}");
    }

    public Customer Get(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        byId.TryGetValue(id, out var c);
        return c;
    }

    public IEnumerable<Customer> GetAll() => byId.Values;
}