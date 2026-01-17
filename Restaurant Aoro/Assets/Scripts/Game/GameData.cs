using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomerCodexEntry
{
    public bool seen;
    public bool unlocked;
    
    public int affinity;
    public int visitCount;

    public Dictionary<string, int> resultCounts = new();
}
[System.Serializable]
public class ItemCodexEntry
{
    public bool seen;      
    public bool unlocked;  
}

[System.Serializable]
public class GameData
{
    public int money;
    public int reputationCustomer;
    public int reputationYoukai;

    public int hour;
    public int minute;
    public int year, month, day;

    public float bgmVolume, seVolume;

    public Dictionary<string, bool> triggers = new();
    public Dictionary<string, int> itemInventory = new();

    public Dictionary<string, CustomerCodexEntry> customerCodex = new();
    public Dictionary<string, ItemCodexEntry> itemCodex = new();
}
