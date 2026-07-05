using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodexSlotView : MonoBehaviour
{
    [Header("UI")]
    public Button button;
    public Image icon;
    public Image background;
    public Image background_locked;
    public TMP_Text nameText;

    [Header("Locked Visual")]
    public GameObject lockOverlay;

    private string _id;
    private bool _unlocked;

    public void Bind(string id, Sprite itemSprite, string itemName, bool unlocked, System.Action<string> onClick)
    {
        _id = id;
        _unlocked = unlocked;

        if (background != null)
            background.gameObject.SetActive(unlocked);

        if (background_locked != null)
            background_locked.gameObject.SetActive(!unlocked);

        if (icon != null)
            icon.gameObject.SetActive(unlocked);

        if (nameText != null)
            nameText.gameObject.SetActive(unlocked);

        if (lockOverlay != null)
            lockOverlay.SetActive(!unlocked);

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = unlocked;
        }

        if (unlocked)
        {
            if (icon != null)
                icon.sprite = itemSprite;

            if (nameText != null)
                nameText.text = itemName;

            if (button != null)
                button.onClick.AddListener(() => onClick?.Invoke(_id));
        }
    }

    public void BindEmpty()
    {
        _id = null;
        _unlocked = false;

        if (background != null)
            background.gameObject.SetActive(false);

        if (background_locked != null)
            background_locked.gameObject.SetActive(true);

        if (icon != null)
            icon.gameObject.SetActive(false);

        if (nameText != null)
        {
            nameText.text = "";
            nameText.gameObject.SetActive(false);
        }

        if (lockOverlay != null)
            lockOverlay.SetActive(false);

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = false;
        }
    }
}
