using System;
using System.Collections.Generic;
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
        switch (item.ItemType)
        {
            case ItemType.Food:
                // FoodPanel (slot1Prefab)
                InstantiateSlot(slot1Prefab, foodPanelContent, item);

                // FoodInventory (slot2Prefab)
                InstantiateSlot(slot2Prefab, foodInventoryContent, item);
                break;
            case ItemType.Ingredient:
                // IngredientPanel (slot1Prefab)
                InstantiateSlot(slot1Prefab, ingredientPanelContent, item);

                // IngredientInventory (slot2Prefab)
                InstantiateSlot(slot2Prefab, ingredientInventoryContent, item);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    public void RebuildFromSaved(Dictionary<string, int> itemCounts)
    {
        ClearAll();

        if (itemCounts == null) return;

        foreach (var kv in itemCounts)
        {
            string itemId = kv.Key;
            int count = kv.Value;

            Item item = ItemDatabase.Instance.GetItem(itemId);
            if (item == null)
            {
                Debug.LogWarning($"[InventoryUI] ItemID를 찾을 수 없음: {itemId}");
                continue;
            }

            for (int i = 0; i < count; i++)
                AddItemToInventory(item);
        }
    }

    private void ClearAll()
    {
        ClearChildren(foodPanelContent);
        ClearChildren(ingredientPanelContent);
        ClearChildren(foodInventoryContent);
        ClearChildren(ingredientInventoryContent);
    }

    private void ClearChildren(Transform parent)
    {
        if (parent == null) return;
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }
    private void InstantiateSlot(GameObject prefab, Transform parent, Item item)
    {
        GameObject slotGO = Instantiate(prefab, parent);
        var slotUI = slotGO.transform.GetChild(0).GetComponent<ItemSlotUI>();

        slotUI.Initialize(item);
    }

}
