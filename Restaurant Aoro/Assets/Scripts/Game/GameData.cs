using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int money;
    public int year, month, day;
    public float bgmVolume, seVolume;
    public Dictionary<string, bool> triggers;
    public Dictionary<string, int> itemInventory;
}
