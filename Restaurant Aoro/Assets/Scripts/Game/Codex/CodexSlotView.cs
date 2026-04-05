using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodexSlotView : MonoBehaviour
{
    [Header("UI")]
    public Button button;
    public Image icon;
    //public TMP_Text nameText;

    [Header("Locked Visual")]
    public Sprite lockedSprite;
    public GameObject lockOverlay;

    private string _id;
    private bool _unlocked;

    public void Bind(string id, Sprite unlockedSprite, string unlockedName, bool unlocked, System.Action<string> onClick)
    {
        _id = id;
        _unlocked = unlocked;

        if (unlocked)
        {
            icon.sprite = unlockedSprite;
            //if (nameText) nameText.text = unlockedName;
            //if (lockOverlay) lockOverlay.SetActive(!unlocked);

            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(_id));
        }
        else
        {
            icon.sprite = lockedSprite;
            //if (nameText) nameText.text = "???";

            button.onClick.RemoveAllListeners();
            button.interactable = false;
        }
    }

    public void BindEmpty(Sprite emptySprite = null)
    {
        if (icon != null)
            icon.sprite = emptySprite != null ? emptySprite : lockedSprite;

        /*if (nameText != null)
            nameText.text = "";*/

        if (lockOverlay != null)
            lockOverlay.SetActive(false);

        button.onClick.RemoveAllListeners();
        button.interactable = false;
    }
}
