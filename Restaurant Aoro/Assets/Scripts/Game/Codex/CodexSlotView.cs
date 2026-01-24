using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodexSlotView : MonoBehaviour
{
    [Header("UI")]
    public Button button;
    public Image icon;
    public TMP_Text nameText;

    [Header("Locked Visual")]
    public Sprite lockedSprite;

    private string _id;
    private bool _unlocked;

    public void Bind(string id, Sprite unlockedSprite, string unlockedName, bool unlocked, System.Action<string> onClick)
    {
        _id = id;
        _unlocked = unlocked;

        if (unlocked)
        {
            icon.sprite = unlockedSprite;
            if (nameText) nameText.text = unlockedName;

            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(_id));
        }
        else
        {
            icon.sprite = lockedSprite;
            if (nameText) nameText.text = "???";

            button.onClick.RemoveAllListeners();
            button.interactable = false;
        }
    }
}
