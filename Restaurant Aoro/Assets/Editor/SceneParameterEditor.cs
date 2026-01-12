using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneParameterEditor : EditorWindow
{
    private enum SceneTab { Game, Title, LoadGame }
    private SceneTab currentTab = SceneTab.Game;

    // 공통 - 사운드
    private float bgmVolume = 0.5f;
    private float seVolume = 0.5f;

    // Game Scene 전용
    private int money = 1000;
    private int year = 2025, month = 1, day = 1;
    private Dictionary<string, bool> triggers = new() {
        { "isBossDefeated", false },
        { "hasKey", false }
    };
    private Dictionary<string, int> itemInventory = new() {
        { "Potion", 3 }, { "Key", 1 }, { "Sword", 0 }
    };
    private string[] itemTypes = { "Potion", "Key", "Sword" };

    [MenuItem("MyTools/Scene Parameter Editor")]
    public static void ShowWindow()
    {
        GetWindow<SceneParameterEditor>("Scene Parameters");
    }

    private void OnGUI()
    {
        currentTab = (SceneTab)GUILayout.Toolbar((int)currentTab, new[] { "Game", "Title", "LoadGame" });

        EditorGUILayout.Space(10);
        switch (currentTab)
        {
            case SceneTab.Game:
                DrawGameTab();
                break;
            case SceneTab.Title:
            case SceneTab.LoadGame:
                DrawSoundTab();
                break;
        }

        EditorGUILayout.Space(15);
        if (GUILayout.Button("Apply to Runtime"))
        {
            ApplySettingsToRuntime();
        }
    }

    private void DrawSoundTab()
    {
        GUILayout.Label("Sound Settings", EditorStyles.boldLabel);
        bgmVolume = EditorGUILayout.Slider("BGM Volume", bgmVolume, 0f, 1f);
        seVolume = EditorGUILayout.Slider("SE Volume", seVolume, 0f, 1f);
    }

    private void DrawGameTab()
    {
        GUILayout.Label("Money", EditorStyles.boldLabel);
        money = EditorGUILayout.IntField("Money", money);

        GUILayout.Space(5);
        GUILayout.Label("Date", EditorStyles.boldLabel);
        year = EditorGUILayout.IntField("Year", year);
        month = EditorGUILayout.IntSlider("Month", month, 1, 12);
        day = EditorGUILayout.IntSlider("Day", day, 1, 31);

        GUILayout.Space(5);
        GUILayout.Label("Triggers", EditorStyles.boldLabel);
        var keys = new List<string>(triggers.Keys);
        foreach (var key in keys)
            triggers[key] = EditorGUILayout.Toggle(key, triggers[key]);

        GUILayout.Space(5);
        GUILayout.Label("Items", EditorStyles.boldLabel);
        foreach (string item in itemTypes)
            itemInventory[item] = EditorGUILayout.IntField(item, itemInventory[item]);

        GUILayout.Space(5);
        DrawSoundTab(); // Game 씬에도 사운드 조절 포함 가능
    }

    private void ApplySettingsToRuntime()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("플레이 모드 중에만 적용할 수 있습니다.");
            return;
        }

        // GameManager 같은 런타임 클래스에서 적용
        var gm = GameObject.FindObjectOfType<GameManager>();
        if (gm != null)
        {
            /*if (currentTab == SceneTab.Game)
            {
                gm.SetMoney(money);
                gm.SetDate(year, month, day);
                gm.SetTriggers(triggers);
                gm.SetItems(itemInventory);
            }

            gm.SetSound(bgmVolume, seVolume);*/
            Debug.Log("설정 적용 완료");
        }
        else
        {
            Debug.LogWarning("GameManager를 찾을 수 없습니다.");
        }

        var init = GameObject.FindObjectOfType<GameInit>();
        if (init != null)
        {
            init.UpdateDisplay(); // 텍스트 즉시 갱신
        }
    }
}
