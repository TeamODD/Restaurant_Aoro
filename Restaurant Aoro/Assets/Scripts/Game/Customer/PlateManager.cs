using UnityEngine;

public class PlateManager : MonoBehaviour
{
    public static PlateManager instance;

    [SerializeField] private InventoryManager inventoryManager; // 인벤토리 갱신용(필요 시)

    private GameObject itemOnHold;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void AddIngredientToPlateTile(GameObject slotObj)
    {
        var slot = slotObj.GetComponent<ItemSlotUI>();
        if (slot == null || slot.item_ == null) return;

        if (slot.item_.ItemType != ItemType.Food) return;

        itemOnHold = slotObj;
    }

    public void IngredientAddedToPlateTile(PlateTile plateTile)
    {
        if (itemOnHold == null || plateTile == null) { ClearHold(); return; }

        var slot = itemOnHold.GetComponent<ItemSlotUI>();
        if (slot == null || slot.item_ == null) { ClearHold(); return; }

        bool added = plateTile.AddItem(slot.item_);
        if (added)
        {
            Destroy(itemOnHold.transform.parent.gameObject);
        }

        ClearHold();
    }

    private void ClearHold()
    {
        itemOnHold = null;
    }
}
