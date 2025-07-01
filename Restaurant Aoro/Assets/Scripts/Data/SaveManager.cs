using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public string currentSaveFileName = "";

    private string saveDirectory => Path.Combine(Application.persistentDataPath, "Save");

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public List<string> GetAllSaveFiles()
    {
        if (!Directory.Exists(saveDirectory))
            Directory.CreateDirectory(saveDirectory);

        var files = Directory.GetFiles(saveDirectory, "save_*.json");
        return new List<string>(files);
    }

    public string CreateNewSaveFileName()
    {
        int index = 1;
        while (true)
        {
            string fileName = $"save_{index:D4}.json";
            string path = Path.Combine(saveDirectory, fileName);
            if (!File.Exists(path))
                return fileName;

            index++;
        }
    }

    public void CreateNewSave()
    {
        GameData data = new GameData
        {
            money = 10000,
            year = 2050,
            month = 1,
            day = 1,
            bgmVolume = 0.7f,
            seVolume = 0.7f,
            triggers = new Dictionary<string, bool>
        {
            { "hasKey", false },
            { "isBossDefeated", false }
        },
            itemInventory = new Dictionary<string, int>
        {
            { "Potion", 2 },
            { "Sword", 0 }
        }
        };

        string fileName = SaveManager.Instance.currentSaveFileName;
        string path = SaveManager.Instance.GetFullPath(fileName);
        string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(path, json);

        Debug.Log($"[New Save] 초기 세이브 파일 생성: {path}");
    }

    public string GetFullPath(string fileName)
    {
        return Path.Combine(saveDirectory, fileName);
    }
}
