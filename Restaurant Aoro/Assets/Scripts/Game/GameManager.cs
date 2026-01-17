using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Runtime State")]
    public int money;
    public int reputationCustomer;
    public int reputationYoukai;
    public int hour, minute;

    public int year, month, day;
    public Dictionary<string, bool> triggers = new();
    public Dictionary<string, int> itemInventory = new();

    public float bgmVolume;
    public float seVolume;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    // 저장 버튼/자동저장 시 호출
    public void SaveGame()
    {
        CaptureFromScene();

        string path = SaveManager.Instance.GetFullPath(SaveManager.Instance.currentSaveFileName);
        var data = new GameData
        {
            money = money,
            reputationCustomer = reputationCustomer,
            reputationYoukai = reputationYoukai,
            hour = hour,
            minute = minute,
            year = year,
            month = month,
            day = day,
            bgmVolume = bgmVolume,
            seVolume = seVolume,
            triggers = new(triggers),
            itemInventory = new(itemInventory),
        };
        data.customerCodex = CustomerCodexManager.Instance.GetAll();
        data.itemCodex = ItemCodexManager.Instance.GetAll();

        string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(path, json);
        Debug.Log($"[Save] 저장됨: {path}");
    }

    public void LoadGame()
    {
        string path = SaveManager.Instance.GetFullPath(SaveManager.Instance.currentSaveFileName);
        if (!File.Exists(path))
        {
            Debug.LogWarning("[Load] 세이브 파일이 없습니다.");
            return;
        }

        string json = File.ReadAllText(path);
        GameData data = JsonConvert.DeserializeObject<GameData>(json);

        money = data.money;
        reputationCustomer = data.reputationCustomer;
        reputationYoukai = data.reputationYoukai;
        hour = data.hour;
        minute = data.minute;

        year = data.year; month = data.month; day = data.day;
        bgmVolume = data.bgmVolume; seVolume = data.seVolume;
        triggers = data.triggers ?? new();
        itemInventory = data.itemInventory ?? new();

        CustomerCodexManager.Instance.LoadFrom(data.customerCodex);
        data.customerCodex ??= new Dictionary<string, CustomerCodexEntry>();
        ItemCodexManager.Instance.LoadFrom(data.itemCodex);
        data.itemCodex ??= new Dictionary<string, ItemCodexEntry>();

        ApplyToScene(data);

        Debug.Log("[Load] 로드 완료");
    }

    private void CaptureFromScene()
    {
        // 돈
        var rm = FindObjectOfType<RestaurantManager>();
        if (rm != null)
        {
            money = rm.CurrentMoney;          // <- RestaurantManager에 getter 필요
            // reputationCustomer / reputationYoukai 는 ReputationState 구조에 맞춰 채우기
            // reputationCustomer = rm.ReputationState.CustomerRepLevel;
            // reputationYoukai = rm.ReputationState.YoukaiRepLevel;
        }

        // 시간
        var gt = FindObjectOfType<GameTime>();
        if (gt != null)
        {
            hour = gt.Hour;                  // <- GameTime에 getter 필요
            minute = gt.Minute;
        }

        // 인벤토리(데이터 저장소 필요)
        var inv = InventoryManager.instance;
        if (inv != null)
        {
            itemInventory = new Dictionary<string, int>(inv.GetAllItemsAsDict()); // <- 이 함수 필요
        }
    }

    private void ApplyToScene(GameData data)
    {
        // 돈 반영
        var rm = FindObjectOfType<RestaurantManager>();
        if (rm != null)
        {
            rm.SetMoney(data.money);         // <- RestaurantManager에 SetMoney 필요
            // rm.ReputationState.SetRepLevels(...)
        }

        // 시간 반영
        var gt = FindObjectOfType<GameTime>();
        if (gt != null)
        {
            gt.SetTime(data.hour, data.minute);  // <- GameTime에 SetTime 필요
        }

        // 인벤토리 UI 재구성
        var inv = InventoryManager.instance;
        if (inv != null)
        {
            inv.LoadFromDict(data.itemInventory);
        }
    }

    public void OnMainButtonClicked()
    {
        SceneManager.LoadScene("Title");
    }
}
