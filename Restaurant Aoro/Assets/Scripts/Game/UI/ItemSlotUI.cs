using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
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
}
