using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeCodexManager : MonoBehaviour
{
    public static RecipeCodexManager Instance;
    public static event Action OnCodexChanged;

    private Dictionary<string, RecipeCodexEntry> entries = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Dictionary<string, RecipeCodexEntry> GetAll() => new(entries);

    public void LoadFrom(Dictionary<string, RecipeCodexEntry> saved)
    {
        entries = saved != null ? new(saved) : new();
        OnCodexChanged?.Invoke();
    }

    public void RegisterRecipe(List<Item> usedIngredients, Item resultItem)
    {
        if (usedIngredients == null || usedIngredients.Count == 0 || resultItem == null)
            return;

        string entryId = MakeEntryId(usedIngredients, resultItem);

        if (entries.ContainsKey(entryId))
            return;

        var entry = new RecipeCodexEntry
        {
            unlocked = true,
            entryId = entryId,
            result = new RecipeResultSnapshot
            {
                itemId = resultItem.ItemID,
                itemName = resultItem.ItemName,
                itemGrade = resultItem.ItemGrade,
                itemType = resultItem.ItemType,
                mainCategory = resultItem.ItemMainCategory,
                subCategory = resultItem.ItemSubCategory
            },
            ingredients = usedIngredients
                .Where(x => x != null)
                .OrderBy(x => x.ItemID)
                .Select(x => new RecipeIngredientSnapshot
                {
                    itemId = x.ItemID,
                    itemName = x.ItemName,
                    itemGrade = x.ItemGrade,
                    itemType = x.ItemType,
                    mainCategory = x.ItemMainCategory,
                    subCategory = x.ItemSubCategory
                })
                .ToList()
        };

        entries[entryId] = entry;
        OnCodexChanged?.Invoke();
    }

    private string MakeEntryId(List<Item> ingredients, Item resultItem)
    {
        var ingredientKey = ingredients
            .Where(x => x != null)
            .Select(x => $"{x.ItemID}_{x.ItemGrade}")
            .OrderBy(x => x)
            .ToList();

        return string.Join("_", ingredientKey) + "->" + $"{resultItem.ItemID}_{resultItem.ItemGrade}";
    }
}