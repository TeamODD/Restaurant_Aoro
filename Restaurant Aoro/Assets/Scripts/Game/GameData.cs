using System.Collections.Generic;
using UnityEngine;

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
}
