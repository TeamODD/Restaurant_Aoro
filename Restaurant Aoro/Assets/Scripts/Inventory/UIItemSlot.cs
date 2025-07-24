using UnityEngine;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour
{
    public ItemData item;
    public Image icon;
    public Button button;

    private void Start()
    {
        icon.sprite = item.icon;
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        CardMaterialManager.Instance.AssignToSelectedCard(item, this);
    }

    public void SetInteractable(bool value)
    {
        button.interactable = value;
    }
}
