using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [Header("Slot Prefabs")]
    public GameObject slot1Prefab;
    public GameObject slot2Prefab;
    public GameObject Prefab_3s;
    public GameObject Prefab_4s;
    public GameObject Prefab_5s;
    public GameObject Prefab_6s;

    [Header("Inventory Panels")]
    public Transform foodPanelContent;
    public Transform ingredientPanelContent;

    [Header("Independent Inventories")]
    public Transform foodInventoryContent;
    public Transform ingredientInventoryContent;

    public void AddItemToInventory(Item item, int count)
    {
        if (item == null)
            return;

        switch (item.ItemType)
        {
            case ItemType.Food:
                UpdateOrCreateSlot(
                    slot1Prefab,
                    foodPanelContent,
                    item,
                    count
                );

                UpdateOrCreateSlot(
                    slot2Prefab,
                    foodInventoryContent,
                    item,
                    count
                );
                break;

            case ItemType.Ingredient:
                UpdateOrCreateSlot(
                    slot1Prefab,
                    ingredientPanelContent,
                    item,
                    count
                );

                UpdateOrCreateSlot(
                    slot2Prefab,
                    ingredientInventoryContent,
                    item,
                    count
                );
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateOrCreateSlot(
        GameObject prefab,
        Transform parent,
        Item item,
        int count)
    {
        if (prefab == null || parent == null || item == null)
            return;

        ItemSlotUI existingSlot = FindSlot(parent, item.ItemID);

        if (existingSlot != null)
        {
            existingSlot.SetQuantity(count);
            return;
        }

        InstantiateSlot(prefab, parent, item, count);
    }

    private ItemSlotUI FindSlot(Transform parent, string itemId)
    {
        ItemSlotUI[] slots = parent.GetComponentsInChildren<ItemSlotUI>(true);

        foreach (ItemSlotUI slot in slots)
        {
            if (slot.item_ == null)
                continue;

            if (slot.item_.ItemID == itemId)
                return slot;
        }

        return null;
    }

    private void InstantiateSlot(
        GameObject prefab,
        Transform parent,
        Item item,
        int count)
    {
        GameObject slotGO = Instantiate(prefab, parent);

        ItemSlotUI slotUI =
            slotGO.GetComponentInChildren<ItemSlotUI>();

        if (slotUI == null)
        {
            Debug.LogError(
                $"[InventoryUI] ItemSlotUI를 찾을 수 없습니다: {slotGO.name}"
            );

            Destroy(slotGO);
            return;
        }

        slotUI.Initialize(item, count);
    }

    public void RebuildFromSaved(Dictionary<string, int> itemCounts)
    {
        ClearAll();

        if (itemCounts == null)
            return;

        foreach (KeyValuePair<string, int> itemData in itemCounts)
        {
            string itemId = itemData.Key;
            int count = itemData.Value;

            Item item = ItemDatabase.Instance.GetItem(itemId);

            if (item == null)
            {
                Debug.LogWarning(
                    $"[InventoryUI] ItemID를 찾을 수 없습니다: {itemId}"
                );

                continue;
            }

            // 개수만큼 반복 생성하지 않고 슬롯 하나만 생성
            AddItemToInventory(item, count);
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
        if (parent == null)
            return;

        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }
}
