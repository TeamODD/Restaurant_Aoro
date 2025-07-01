using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int money;
    public int year, month, day;
    public Dictionary<string, bool> triggers = new();
    public Dictionary<string, int> itemInventory = new();

    public float bgmVolume;
    public float seVolume;

    public void SetMoney(int val) => money = val;
    public void SetDate(int y, int m, int d) { year = y; month = m; day = d; }
    public void SetTriggers(Dictionary<string, bool> trig) => triggers = new(trig);
    public void SetItems(Dictionary<string, int> items) => itemInventory = new(items);
    public void SetSound(float bgm, float se) { bgmVolume = bgm; seVolume = se; }

    private string savePath => Application.dataPath + "/Save/save.json";

    void Start()
    {
        LoadGame();
    }

    public void SaveGame()
    {
        string path = SaveManager.Instance.GetFullPath(SaveManager.Instance.currentSaveFileName);
        GameData data = new()
        {
            money = this.money,
            year = this.year,
            month = this.month,
            day = this.day,
            bgmVolume = this.bgmVolume,
            seVolume = this.seVolume,
            triggers = this.triggers,
            itemInventory = this.itemInventory
        };

        string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(path, json);
        Debug.Log($"[Save] 저장됨: {path}");
    }

    public void LoadGame()
    {
        string path = SaveManager.Instance.GetFullPath(SaveManager.Instance.currentSaveFileName);
        if (!File.Exists(path))
        {
            Debug.LogWarning("세이브 파일이 없습니다.");
            return;
        }

        string json = File.ReadAllText(path);
        GameData data = JsonConvert.DeserializeObject<GameData>(json);

        this.money = data.money;
        this.year = data.year;
        this.month = data.month;
        this.day = data.day;
        this.bgmVolume = data.bgmVolume;
        this.seVolume = data.seVolume;
        this.triggers = data.triggers;
        this.itemInventory = data.itemInventory;

        Debug.Log("[Load] 로드 완료");
    }

    public void OnMainButtonClicked()
    {
        SceneManager.LoadScene("Title");
    }
}
