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
        if (scene.name != "Restaurant 1") //후에 방으로 바꿀 예정
            return;

        if (loadedData == null)
            return;

        ApplyToScene(loadedData);

        loadedData = null;

        Debug.Log("[Load] 씬 데이터 적용 완료");
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
        Debug.Log($"[Save] �����: {path}");
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

    private void ApplyToScene(GameData data)
    {
        var rm = FindObjectOfType<RestaurantManager>();

        if (rm != null)
        {
            rm.SetMoney(data.money);
        }

        var gt = FindObjectOfType<GameTime>();

        if (gt != null)
        {
            gt.SetTime(data.hour, data.minute);
        }

        var reputationState = FindObjectOfType<ReputationState>();

        if (reputationState != null)
        {
            reputationState.SetReputation(
                data.reputationCustomer,
                data.reputationYoukai
            );
        }

        var inv = InventoryManager.instance;

        if (inv != null)
        {
            inv.LoadFromDict(data.itemInventory);
        }

        if (CustomerCodexManager.Instance != null)
        {
            CustomerCodexManager.Instance.LoadFrom(
                data.customerCodex
            );
        }

        if (ItemCodexManager.Instance != null)
        {
            ItemCodexManager.Instance.LoadFrom(
                data.itemCodex
            );
        }
    }

    public void OnMainButtonClicked()
    {
        SceneManager.LoadScene("Title");
    }
}
