using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomerCodexManager : MonoBehaviour
{
    public static CustomerCodexManager Instance;
    public static event Action OnCodexChanged;

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
        OnCodexChanged?.Invoke();
    }

    /*public void Unlock(string customerId)
    {
        var e = GetOrCreate(customerId);
        e.seen = true;
        e.unlocked = true;
        OnCodexChanged?.Invoke();
    }*/

    /*public void AddResult(string customerId, ResultType type)
    {
        var e = GetOrCreate(customerId);
        e.visitCount++;

        string key = type.ToString();
        if (e.resultCounts.ContainsKey(key)) e.resultCounts[key]++;
        else e.resultCounts[key] = 1;
        OnCodexChanged?.Invoke();
    }*/

    private CustomerCodexEntry GetOrCreate(string id)
    {
        if (!entries.TryGetValue(id, out var e))
        {
            e = new CustomerCodexEntry();
            entries[id] = e;
        }
        return e;
    }
    public void UnlockEntranceInfo(string customerId)
    {
        var e = GetOrCreate(customerId);

        e.seen = true;
        e.mainIllustrationUnlocked = true;
        e.entranceIllustrationUnlocked = true;
        e.basicDescriptionUnlocked = true;
        Debug.Log($"[CustomerCodex] Entrance unlocked: {customerId}");

        OnCodexChanged?.Invoke();
    }
    public void UnlockRightInfo(string customerId)
    {
        var entry = GetOrCreate(customerId);

        entry.rightIllustrationUnlocked = true;

        OnCodexChanged?.Invoke();
    }

    public void UnlockSeatedInfo(string customerId)
    {
        var e = GetOrCreate(customerId);

        e.seatedIllustrationUnlocked = true;

        OnCodexChanged?.Invoke();
    }

    public void AddResult(string customerId, ResultType type)
    {
        var e = GetOrCreate(customerId);

        e.visitCount++;

        switch (type)
        {
            case ResultType.Perfect:
                e.perfectCount++;
                e.perfectIllustrationUnlocked = true;
                break;

            case ResultType.Excellent:
                e.excellentCount++;
                e.excellentIllustrationUnlocked = true;
                break;

            case ResultType.Success:
                e.successCount++;
                e.successIllustrationUnlocked = true;
                break;

            case ResultType.Fail:
            case ResultType.Late:
            case ResultType.WrongOrder:
                e.failCount++;
                e.failIllustrationUnlocked = true;
                break;
        }

        int totalResultCount =
            e.perfectCount +
            e.excellentCount +
            e.successCount +
            e.failCount;

        if (totalResultCount >= 3)
            e.detailDescriptionUnlocked = true;

        OnCodexChanged?.Invoke();
    }
}
