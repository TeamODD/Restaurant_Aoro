using TMPro;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    public TextMeshProUGUI infoText;

    void Start()
    {
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        if (gm == null || infoText == null) return;

        string text = $"Money: {gm.money}\n" +
                      $"Date: {gm.year}-{gm.month:D2}-{gm.day:D2}\n" +
                      $"Triggers:\n";

        foreach (var trigger in gm.triggers)
            text += $"- {trigger.Key}: {(trigger.Value ? "ON" : "OFF")}\n";

        text += $"Items:\n";
        foreach (var item in gm.itemInventory)
            text += $"- {item.Key}: {item.Value}\n";

        infoText.text = text;
    }

    public void UpdateDisplay()
    {
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        if (gm == null || infoText == null) return;

        string text = $"Money: {gm.money}\n";
        text += $"Date: {gm.year}-{gm.month:D2}-{gm.day:D2}\n";
        foreach (var trigger in gm.triggers)
            text += $"- {trigger.Key}: {(trigger.Value ? "ON" : "OFF")}\n";
        foreach (var item in gm.itemInventory)
            text += $"- {item.Key}: {item.Value}\n";

        infoText.text = text;
    }
}
