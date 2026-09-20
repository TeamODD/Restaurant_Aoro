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
    public Dictionary<string, CustomerCodexEntry> customerCodex = new();
    public Dictionary<string, ItemCodexEntry> itemCodex = new();

    public float bgmVolume;
    public float seVolume;

    private GameData loadedData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Map")
        {
            ApplyRuntimeToMapScene();
            Debug.Log("[GameManager] Runtime State → Map 적용 완료");
        }
        else if (scene.name == "Restaurant 1")
        {
            ApplyRuntimeToRestaurantScene();
            Debug.Log("[GameManager] Runtime State → Restaurant 적용 완료");
        }
    }

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
        Debug.Log($"[Save]: {path}");
    }

    public bool LoadGameData()
    {
        string path = SaveManager.Instance.GetFullPath(
            SaveManager.Instance.currentSaveFileName
        );

        if (!File.Exists(path))
        {
            Debug.LogWarning("[Load] 세이브 파일이 없습니다.");
            return false;
        }

        string json = File.ReadAllText(path);

        loadedData = JsonConvert.DeserializeObject<GameData>(json);

        if (loadedData == null)
        {
            Debug.LogWarning("[Load] 세이브 데이터를 읽지 못했습니다.");
            return false;
        }

        loadedData.triggers ??= new Dictionary<string, bool>();
        loadedData.itemInventory ??= new Dictionary<string, int>();
        loadedData.customerCodex ??= new Dictionary<string, CustomerCodexEntry>();
        loadedData.itemCodex ??= new Dictionary<string, ItemCodexEntry>();

        money = loadedData.money;

        reputationCustomer = loadedData.reputationCustomer;
        reputationYoukai = loadedData.reputationYoukai;

        hour = loadedData.hour;
        minute = loadedData.minute;

        year = loadedData.year;
        month = loadedData.month;
        day = loadedData.day;

        bgmVolume = loadedData.bgmVolume;
        seVolume = loadedData.seVolume;

        triggers = new Dictionary<string, bool>(loadedData.triggers);
        itemInventory = new Dictionary<string, int>(loadedData.itemInventory);

        customerCodex = new Dictionary<string, CustomerCodexEntry>(loadedData.customerCodex);
        itemCodex = new Dictionary<string, ItemCodexEntry>(loadedData.itemCodex);
        Debug.Log("[Load] 데이터 로드 완료");

        return true;
    }

    private void CaptureFromScene()
    {
        // 돈
        var rm = FindObjectOfType<RestaurantManager>();

        if (rm != null)
        {
            money = rm.CurrentMoney;
        }

        // 평판
        var reputationState = FindObjectOfType<ReputationState>();

        if (reputationState != null)
        {
            reputationCustomer = reputationState.CustomerReputation;
            reputationYoukai = reputationState.YoukaiReputation;
        }

        // 시간
        var gt = FindObjectOfType<GameTime>();

        if (gt != null)
        {
            hour = gt.Hour;
            minute = gt.Minute;
        }

        // 인벤토리
        var inv = InventoryManager.instance;

        if (inv != null)
        {
            itemInventory = new Dictionary<string, int>(
                inv.GetAllItemsAsDict()
            );
        }
    }

    private void ApplyRuntimeToRestaurantScene()
    {
        var rm = FindObjectOfType<RestaurantManager>();

        if (rm != null)
        {
            rm.SetMoney(money);
        }

        var gt = FindObjectOfType<GameTime>();

        if (gt != null)
        {
            gt.SetTime(hour, minute);
        }

        var reputationState = FindObjectOfType<ReputationState>();

        if (reputationState != null)
        {
            reputationState.SetReputation(
                reputationCustomer,
                reputationYoukai
            );
        }

        var inv = InventoryManager.instance;

        if (inv != null)
        {
            inv.LoadFromDict(itemInventory);
        }

        if (CustomerCodexManager.Instance != null)
        {
            CustomerCodexManager.Instance.LoadFrom(
                customerCodex
            );
        }

        if (ItemCodexManager.Instance != null)
        {
            ItemCodexManager.Instance.LoadFrom(
                itemCodex
            );
        }
    }
    private void ApplyRuntimeToMapScene()
    {
        var mm = FindObjectOfType<MapManager>();

        if (mm != null)
        {
            mm.SetMoney(money);
        }
        /*
        var gt = FindObjectOfType<GameTime>();

        if (gt != null)
        {
            gt.SetTime(hour, minute);
        }
        */
        // Map 인벤토리 구현 후 추가
    }

    public void OnMainButtonClicked()
    {
        SceneManager.LoadScene("Title");
    }
}
