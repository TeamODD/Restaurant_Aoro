using Game.Cook;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class ItemSlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public Item item_;

    public void Initialize(Item item)
    {
        itemImage.sprite = item.ItemSprite;
        itemName.text = item.ItemName;
        item_ = item;

        GetComponent<InventoryItemDrag>().Init(item);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!eventData.dragging)
            CookManager.instance.AddIngredientToCookTile(gameObject);
    }
}