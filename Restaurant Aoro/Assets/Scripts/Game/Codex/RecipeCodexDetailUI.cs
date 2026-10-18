
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeCodexDetailUI : MonoBehaviour
{
    [Header("Result")]
    [SerializeField] private Image resultImage;
    [SerializeField] private TextMeshProUGUI resultNameText;
    [SerializeField] private TextMeshProUGUI resultGradeText;

    [Header("Ingredients")]
    [SerializeField] private Transform ingredientContainer;

    [Header("Slot Prefabs")]
    [SerializeField] private GameObject Prefab_3s;
    [SerializeField] private GameObject Prefab_4s;
    [SerializeField] private GameObject Prefab_5s;
    [SerializeField] private GameObject Prefab_6s;

    [Header("Buttons")]
    [SerializeField] private Button cookButton;

    private RecipeCodexEntry currentRecipe;

    public void Open(RecipeCodexEntry entry)
    {
        if (entry == null || entry.result == null)
            return;

        currentRecipe = entry;
        gameObject.SetActive(true);

        Refresh();
    }

    private void Refresh()
    {
        Item resultItem = ItemDatabase.Instance.GetItem(
            currentRecipe.result.itemId
        );

        if (resultItem == null)
            return;

        resultImage.sprite = resultItem.ItemSprite;
        resultNameText.text = resultItem.ItemName;
        resultGradeText.text = resultItem.ItemGrade.ToString();

        RefreshIngredients();
    }

    private void RefreshIngredients()
    {
        Debug.Log($"[RecipeDetail] 재료 수: {currentRecipe.ingredients.Count}");

        foreach (Transform child in ingredientContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var ingredient in currentRecipe.ingredients)
        {
            Debug.Log(
                $"[RecipeDetail] 재료 ID: {ingredient.itemId}, 등급: {ingredient.itemGrade}"
            );

            Item item = ItemDatabase.Instance.GetItem(ingredient.itemId);

            if (item == null)
            {
                Debug.LogWarning($"[RecipeDetail] 아이템을 찾을 수 없음: {ingredient.itemId}");
                continue;
            }

            GameObject prefab = GetIngredientPrefab(ingredient.itemGrade);

            if (prefab == null)
            {
                Debug.LogWarning($"[RecipeDetail] 프리팹 없음: {ingredient.itemGrade}");
                continue;
            }

            GameObject slotObject = Instantiate(prefab, ingredientContainer);

            ItemSlotUI slot = slotObject.GetComponentInChildren<ItemSlotUI>();

            if (slot == null)
            {
                Debug.LogWarning("[RecipeDetail] ItemSlotUI 컴포넌트 없음");
                Destroy(slotObject);
                continue;
            }

            slot.Initialize(item, 1);

            InventoryItemDrag drag = slot.GetComponent<InventoryItemDrag>();

            if (drag != null)
                drag.enabled = false;

            slot.enabled = false;

            Debug.Log($"[RecipeDetail] 슬롯 생성 완료: {item.ItemName}");
        }
    }
    private GameObject GetIngredientPrefab(ItemGrade grade)
    {
        return grade switch
        {
            ItemGrade.Common => Prefab_3s,
            ItemGrade.Fine => Prefab_4s,
            ItemGrade.Premium => Prefab_5s,
            ItemGrade.Exquisite => Prefab_6s,
            _ => null
        };
    }
}
