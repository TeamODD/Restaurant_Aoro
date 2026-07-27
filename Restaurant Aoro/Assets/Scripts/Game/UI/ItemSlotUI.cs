using Game.Cook;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ItemSlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public Item item_;
    public TextMeshProUGUI quantity;

    private int itemCount;
    public void Initialize(Item item, int count)
    {
        if (item == null) return;

        itemImage.sprite = item.ItemSprite;
        itemName.text = item.ItemName;
        item_ = item;

        SetQuantity(count);

        GetComponent<InventoryItemDrag>().Init(item);
    }
    public void SetQuantity(int count)
    {
        itemCount = count;

        if (quantity == null)
            return;

        if (itemCount <= 1)
        {
            quantity.text = string.Empty;
        }
        else
        {
            quantity.text = $"x{itemCount}";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.dragging) return;

        if (item_.ItemType == ItemType.Ingredient) CookManager.instance.IngredientAddedToCookTile(this);
    }
}