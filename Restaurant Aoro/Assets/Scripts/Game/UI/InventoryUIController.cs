using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [Header("Slot Prefabs")]
    public GameObject slot1Prefab;
    public GameObject slot2Prefab;

    [Header("Inventory Panels")]
    public Transform foodPanelContent;
    public Transform ingredientPanelContent;

    [Header("Independent Inventories")]
    public Transform foodInventoryContent;
    public Transform ingredientInventoryContent;

    public void AddItemToInventory(Item item)
    {
        if (item.ItemType == "Food")
        {
            // FoodPanel (slot1Prefab)
            InstantiateSlot(slot1Prefab, foodPanelContent, item);

            // FoodInventory (slot2Prefab)
            InstantiateSlot(slot2Prefab, foodInventoryContent, item);
        }
        else if (item.ItemType == "Ingredient")
        {
            // IngredientPanel (slot1Prefab)
            InstantiateSlot(slot1Prefab, ingredientPanelContent, item);

            // IngredientInventory (slot2Prefab)
            InstantiateSlot(slot2Prefab, ingredientInventoryContent, item);
        }
    }

    private void InstantiateSlot(GameObject prefab, Transform parent, Item item)
    {
        GameObject slotGO = Instantiate(prefab, parent);
        var slotUI = slotGO.transform.GetChild(0).GetComponent<ItemSlotUI>();

        slotUI.Initialize(item);
    }
}
